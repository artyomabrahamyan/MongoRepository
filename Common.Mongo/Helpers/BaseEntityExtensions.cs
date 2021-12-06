using System;
using System.Linq.Expressions;
using Common.Mongo.Models;
using MongoDB.Driver;

namespace Common.Mongo.Helpers
{
    public static class BaseEntityExtensions
    {
        public static ReplaceOneModel<TEntity> ToReplaceOneModel<TEntity, TId>(
            this TEntity entity,
            Expression<Func<TEntity, bool>> predicate)
            where TEntity : BaseEntity<TId>
            where TId : IEquatable<TId>
        {
            entity.UpdatedDate = DateTime.UtcNow;

            return new ReplaceOneModel<TEntity>(
                 new ExpressionFilterDefinition<TEntity>(predicate),
                 entity)
            {
                IsUpsert = true,
            };
        }
    }
}
