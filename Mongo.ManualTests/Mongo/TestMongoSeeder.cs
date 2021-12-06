using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Mongo.Abstractions;

namespace Mongo.ManualTests.Mongo
{
    public class TestMongoSeeder : IMongoDbSeeder
    {
        private TestMongoRepository _repository;

        public TestMongoSeeder(TestMongoRepository repository)
        {
            _repository = repository;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            await _repository.DropAsync(Constants.PartitionKey, cancellationToken);

            var models = Enumerable.Range(1, 4).Select(x => new TestModel
            {
                PartitionKey = Constants.PartitionKey,
                TestCompanyId = x,
                TestStringProp = x.ToString(),
            });

            await _repository.InsertManyAsync(models, cancellationToken);
        }
    }
}
