using System.Threading;
using System.Threading.Tasks;

namespace Common.Mongo.Abstractions
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}