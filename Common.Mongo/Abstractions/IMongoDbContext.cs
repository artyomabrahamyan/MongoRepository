using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Common.Mongo.Abstractions
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<TEntity> GetCollection<TEntity>(string partitionKey = null);

        Task DropCollectionAsync<TDocument>(
            string partitionKey = null,
            CancellationToken cancellationToken = default);
    }
}
