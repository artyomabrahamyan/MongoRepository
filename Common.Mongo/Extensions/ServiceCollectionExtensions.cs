using Common.Mongo;
using Common.Mongo.Abstractions;
using Common.Mongo.Options;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Common.Mongo.Extensions
{
    /// <summary>
    /// Extension methods for adding MongoDb services to an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers mongo dependancies to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection
        /// to add the service to.</param>
        /// <param name="opts">Mongo db options for registration.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            MongoDbOptions opts)
        {
            return services
                    .AddSingleton<IMongoClient>(new MongoClient(opts.ConnectionString))
                    .AddScoped(sp => sp.GetService<IMongoClient>().GetDatabase(opts.Database))
                    .AddScoped<IMongoDbContext, MongoDbContext>();
        }

        /// <summary>
        /// Registers mongo dependancies with the scoped seeder to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TSeeder">Type of the seeder.</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection
        /// to add the service to.</param>
        /// <param name="opts">Mongo db options for registration.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongo<TSeeder>(
            this IServiceCollection services,
            MongoDbOptions opts)
            where TSeeder : class, IMongoDbSeeder
            => services.AddMongo(opts)
              .AddScoped<IMongoDbInitializer, MongoDbInitializer>()
              .AddScoped<IMongoDbSeeder, TSeeder>();
    }
}