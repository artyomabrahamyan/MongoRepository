using System;
using Common.Mongo;
using Common.Mongo.Abstractions;

namespace Mongo.ManualTests.Mongo
{
    /// <summary>
    /// Repository for testing purposes.
    /// </summary>
    public class TestMongoRepository : MongoRepository<TestModel, Guid>
    {
        public TestMongoRepository(IMongoDbContext mongoDbContext)
            : base(mongoDbContext)
        {
        }
    }
}
