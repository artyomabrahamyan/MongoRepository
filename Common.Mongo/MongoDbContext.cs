using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Mongo.Abstractions;
using Common.Mongo.Attributes;
using Common.Mongo.Helpers.Pluralization;
using MongoDB.Driver;

namespace Common.Mongo
{
    /// <summary>
    /// Handles the main interaction with mongo database and collections.
    /// </summary>
    internal class MongoDbContext : IMongoDbContext
    {
        public MongoDbContext(IMongoDatabase database)
        {
            Database = database;
        }

        /// <summary>
        /// The <see cref="IMongoDatabase"/> from the official MongoDB.Driver.
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Returns a collection for a document type and partition key if available.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The optional value of the partition key.</param>
        public IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null) =>
        Database.GetCollection<TDocument>(GetCollectionName<TDocument>(partitionKey));

        /// <summary>
        /// Removes a collection from database.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The optional value of the partition key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task DropCollectionAsync<TDocument>(
            string partitionKey = null,
            CancellationToken cancellationToken = default) =>
                Database.DropCollectionAsync(GetCollectionName<TDocument>(partitionKey), cancellationToken);

        /// <summary>
        /// Extracts the CollectionName attribute from the entity type, if any.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The name of the collection in which the TDocument is stored.</returns>
        private static string GetAttributeCollectionName<TDocument>() =>
                                     (typeof(TDocument)
                                     .GetTypeInfo()
                                     .GetCustomAttributes(typeof(CollectionNameAttribute))
                                     .FirstOrDefault() as CollectionNameAttribute)?.Name;

        /// <summary>
        /// Gets Collection name from the document type provided.
        /// </summary>
        /// <typeparam name="TDocument">Type of the document.</typeparam>
        /// <param name="partitionKey">Partition key for the collection.</param>
        /// <returns>A collection name with the partition key if available.</returns>
        private static string GetCollectionName<TDocument>(string partitionKey)
        {
            var collectionName = GetAttributeCollectionName<TDocument>()
                                 ?? FormatName<TDocument>();

            return string.IsNullOrEmpty(partitionKey)
                ? collectionName
                : $"{collectionName}_{partitionKey}";
        }

        private static string FormatName<TDocument>() =>
            typeof(TDocument).Name.Pluralize().Underscore().ToLower();
    }
}
