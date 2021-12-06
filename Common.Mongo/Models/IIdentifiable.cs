using System;

namespace Common.Mongo.Models
{
    public interface IIdentifiable<TId>
        where TId : IEquatable<TId>
    {
        TId Id { get; }
    }
}