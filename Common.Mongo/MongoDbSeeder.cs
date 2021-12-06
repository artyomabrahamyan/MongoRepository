using System.Threading;
using System.Threading.Tasks;
using Common.Mongo.Abstractions;

namespace Common.Mongo
{
    public abstract class MongoDbSeeder : IMongoDbSeeder
    {
        protected readonly IMongoDbContext MongoContext;

        public MongoDbSeeder(IMongoDbContext mongoContext)
        {
            MongoContext = mongoContext;
        }

        public abstract Task SeedAsync(CancellationToken cancellationToken = default);
    }
}