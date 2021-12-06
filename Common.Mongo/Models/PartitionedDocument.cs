using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Mongo.Models
{
    /// <summary>
    /// This class represents a document that can be inserted in a collection that can be partitioned.
    /// The partition key allows for the creation of different collections having the same document schema.
    /// This can be useful if you are planning to build a Software as a Service (SaaS) Platform, or if you want to reduce indexing.
    /// You could for example insert Logs in different collections based on the week and year they where created, or their Log category/source.
    /// </summary>
    /// <typeparam name="TId">Type of the Identifier.</typeparam>
    public abstract class PartitionedDocument<TId> : BaseEntity<TId>
        where TId : IEquatable<TId>
    {
        public PartitionedDocument(string partitionKey)
        {
            PartitionKey = partitionKey;
        }

        public PartitionedDocument()
        {
        }

        /// <summary>
        /// The name of the property used for partitioning the collection
        /// This will not be inserted into the collection.
        /// This partition key will be prepended to the collection name to create a new collection.
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public string PartitionKey { get; set; }
    }
}
