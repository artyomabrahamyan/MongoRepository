using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Mongo.Models
{
    public abstract class BaseEntity<TId> : IIdentifiable<TId>
        where TId : IEquatable<TId>
    {
        protected BaseEntity()
        {
        }

        protected BaseEntity(TId id)
        {
            Id = id;
        }

        [BsonId]
        [BsonIgnoreIfDefault]
        public TId Id { get; protected set; }

        public DateTime CreatedDate { get; internal protected set; }

        public DateTime UpdatedDate { get; internal protected set; }
    }
}