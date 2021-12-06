using System.Threading.Tasks;

namespace Common.Mongo.Abstractions
{
    public interface IMongoDbInitializer
    {
        Task InitializeAsync();
    }
}