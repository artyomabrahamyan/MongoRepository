using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Mongo.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Common.Mongo.Abstractions
{
    public interface IMongoRepository<TEntity, TId>
        where TEntity : class, IIdentifiable<TId>
        where TId : IEquatable<TId>
    {
        IMongoQueryable<TEntity> AsMongoQueryable(string partitionKey = null);

        Task<string> CreateAscendingIndexAsync(
            Expression<Func<TEntity, object>> field,
            CreateIndexOptions options = null,
            string partitionKey = null);

        Task<string> CreateCombinedTextIndexAsync(
            IEnumerable<Expression<Func<TEntity, object>>> fields,
            CreateIndexOptions options = null,
            string partitionKey = null);

        Task<string> CreateDescendingIndexAsync(
            Expression<Func<TEntity, object>> field,
            CreateIndexOptions options = null,
            string partitionKey = null);

        Task DeleteAsync(TId id, string partitionKey = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            string partitionKey = null,
            CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            string partitionKey = null,
            CancellationToken cancellationToken = default);

        Task<PagedResultEntity<TEntity>> GetPagedByAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, bool>> totalRowsPredicate = null,
            SortDefinition<TEntity> sort = null,
            string partitionKey = null,
            CancellationToken cancellationToken = default);

        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task<ReplaceOneResult> UpdateAsync(
           Expression<Func<TEntity, bool>> predicate,
           TEntity entity,
           bool isUpsert = false,
           CancellationToken cancellationToken = default);

        Task<UpdateResult> UpdateManyAsync(
            FilterDefinition<TEntity> filter,
            UpdateDefinition<TEntity> updateDefinition,
            bool isUpsert = false,
            string partitionKey = null,
            CancellationToken cancellationToken = default);

        Task DropAsync(
            string partitionKey = null,
            CancellationToken cancellationToken = default);

        Task<BulkWriteResult<TEntity>> BulkUpsertAsync(
            IEnumerable<WriteModel<TEntity>> requests,
            string partitionKey = null, CancellationToken
            cancellationToken = default);
    }
}