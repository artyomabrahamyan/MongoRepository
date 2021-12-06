using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Mongo.Abstractions;
using Common.Mongo.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Common.Mongo
{
    /// <summary>
    /// Provides generic functionality for interaction with mongo db.
    /// Inspired by https://github.com/alexandre-spieser/mongodb-generic-repository.
    /// </summary>
    /// <typeparam name="TEntity">Specifies the type of entity to store.</typeparam>
    /// <typeparam name="TId">Specifies the tyep of identifier.</typeparam>
    public abstract class MongoRepository<TEntity, TId> : IMongoRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
        where TId : IEquatable<TId>
    {
        private readonly IMongoDbContext _mongoDbContext;

        protected MongoRepository(IMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Exposes mongo collection as <see cref="IMongoQueryable"/> for the given type.
        /// </summary>
        /// <param name="partitionKey">Optional value for the partition key.</param>
        /// <returns>Mongo queryable.</returns>
        public IMongoQueryable<TEntity> AsMongoQueryable(string partitionKey = null)
            => HandlePartitioned(partitionKey).AsQueryable();

        public Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            string partitionKey = null,
            CancellationToken cancellationToken = default)
            => HandlePartitioned(partitionKey)
                .Find(predicate)
                .SingleOrDefaultAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            string partitionKey = null,
            CancellationToken cancellationToken = default)
            => await HandlePartitioned(partitionKey)
                .Find(predicate)
                .ToListAsync(cancellationToken);

        public async Task<PagedResultEntity<TEntity>> GetPagedByAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = default,
            Expression<Func<TEntity, bool>> totalRowsPredicate = default,
            SortDefinition<TEntity> sort = default,
            string partitionKey = null,
            CancellationToken cancellationToken = default)
        {
            totalRowsPredicate ??= x => true;
            predicate ??= x => true;

            var pagedResult = new PagedResultEntity<TEntity>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRowsCount = await AsMongoQueryable(partitionKey).CountAsync(totalRowsPredicate, cancellationToken),
                RowCount = await AsMongoQueryable(partitionKey).CountAsync(predicate, cancellationToken),
            };

            var pageCount = (double)pagedResult.RowCount / pageSize;

            if (pagedResult.RowCount == 0)
            {
                return pagedResult;
            }

            if (pageCount > 0 && pageNumber > pageCount)
            {
                pagedResult.RowCount = default;
                return pagedResult;
            }

            pagedResult.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (pageNumber - 1) * pageSize;

            var result = HandlePartitioned(partitionKey).Find(predicate);
            if (sort != default)
            {
                result = result.Sort(sort);
            }

            result = result
                .Skip(skip)
                .Limit(pageSize);

            pagedResult.Results = await result.ToListAsync(cancellationToken);

            return pagedResult;
        }

        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedDate = DateTime.UtcNow;

            return HandlePartitioned(entity)
                 .InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        public Task<ReplaceOneResult> UpdateAsync(
           Expression<Func<TEntity, bool>> predicate,
           TEntity entity,
           bool isUpsert = false,
           CancellationToken cancellationToken = default)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            return HandlePartitioned(entity)
              .ReplaceOneAsync(
               new ExpressionFilterDefinition<TEntity>(predicate),
               entity,
               new ReplaceOptions { IsUpsert = isUpsert },
               cancellationToken: cancellationToken);
        }

        public Task<UpdateResult> UpdateManyAsync(
           FilterDefinition<TEntity> filter,
           UpdateDefinition<TEntity> updateDefinition,
           bool isUpsert = false,
           string partitionKey = null,
           CancellationToken cancellationToken = default)
        {
            updateDefinition.Set(x => x.UpdatedDate, DateTime.UtcNow);

            return
                HandlePartitioned(partitionKey)
                .UpdateManyAsync(
              filter, updateDefinition, new UpdateOptions { IsUpsert = isUpsert }, cancellationToken: cancellationToken);
        }

        public Task InsertManyAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }

            return HandlePartitioned(entities?.First())
               .InsertManyAsync(entities, cancellationToken: cancellationToken);
        }

        public Task<BulkWriteResult<TEntity>> BulkUpsertAsync(
            IEnumerable<WriteModel<TEntity>> requests,
            string partitionKey = null,
            CancellationToken cancellationToken = default)
        {
            return HandlePartitioned(partitionKey)
               .BulkWriteAsync(requests, cancellationToken: cancellationToken);
        }

        public Task DeleteAsync(
            TId id,
            string partitionKey = null,
            CancellationToken cancellationToken = default)
            => HandlePartitioned(partitionKey)
              .DeleteOneAsync(e => e.Id.Equals(id), cancellationToken);

        public Task DropAsync(
            string partitionKey = null,
            CancellationToken cancellationToken = default)
            => _mongoDbContext.DropCollectionAsync<TEntity>(partitionKey, cancellationToken);

        #region Index Management

        public Task<string> CreateAscendingIndexAsync(
            Expression<Func<TEntity, object>> field,
            CreateIndexOptions options = null,
            string partitionKey = null)
        {
            var collection = HandlePartitioned(partitionKey);
            var indexKey = Builders<TEntity>.IndexKeys;

            return collection.Indexes
                .CreateOneAsync(
                    new CreateIndexModel<TEntity>(indexKey.Ascending(field), options));
        }

        public Task<string> CreateDescendingIndexAsync(
            Expression<Func<TEntity, object>> field,
            CreateIndexOptions options = null,
            string partitionKey = null)
        {
            var collection = HandlePartitioned(partitionKey);

            var indexKey = Builders<TEntity>.IndexKeys;
            return collection.Indexes
                .CreateOneAsync(
                    new CreateIndexModel<TEntity>(indexKey.Descending(field), options));
        }

        public Task<string> CreateCombinedTextIndexAsync(
            IEnumerable<Expression<Func<TEntity, object>>> fields,
            CreateIndexOptions options = null,
            string partitionKey = null)
        {
            var collection = HandlePartitioned(partitionKey);

            var indexDefinitions = fields.Select(
                field => Builders<TEntity>.IndexKeys.Text(field)).ToList();

            return collection.Indexes
                .CreateOneAsync(new CreateIndexModel<TEntity>(
                    Builders<TEntity>.IndexKeys.Combine(indexDefinitions),
                    options));
        }

        #endregion

        /// <summary>
        /// Handles collection partitioning.
        /// </summary>
        /// <param name="entity">Entity provided.</param>
        /// <returns>Mongo Collection for the particular entity.</returns>
        private IMongoCollection<TEntity> HandlePartitioned(TEntity entity)
        {
            string partitionKey = null;

            if (entity is PartitionedDocument<TId> partitionedDocument)
            {
                partitionKey = partitionedDocument?.PartitionKey;
            }

            return HandlePartitioned(partitionKey);
        }

        /// <summary>
        /// Handles collection partitioning.
        /// </summary>
        /// <param name="partitionKey">Optional valu for partition key.</param>
        /// <returns>Mongo Collection for the particular entity.</returns>
        private IMongoCollection<TEntity> HandlePartitioned(string partitionKey = null) =>
                    _mongoDbContext.GetCollection<TEntity>(partitionKey);
    }
}