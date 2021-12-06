using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Mongo.Abstractions;
using Common.Mongo.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Common.Mongo
{
    public class MongoDbInitializer : IMongoDbInitializer
    {
        private static bool _initialized;
        private readonly bool _seed;
        private readonly IMongoDbSeeder _seeder;

        public MongoDbInitializer(
            IMongoDbSeeder seeder,
            IOptions<MongoDbOptions> options)
        {
            _seeder = seeder;
            _seed = options.Value.Seed;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }

            RegisterConventions();
            _initialized = true;

            if (_seed)
            {
                await _seeder.SeedAsync();
            }
        }

        private static void RegisterConventions()
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            ConventionRegistry.Register(
                nameof(MongoDbConventions),
                new MongoDbConventions(),
                x => true);
        }

        private class MongoDbConventions : IConventionPack
        {
            public IEnumerable<IConvention> Conventions => new List<IConvention>
            {
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention(),
            };
        }
    }
}