using Common.Mongo.Abstractions;
using Common.Mongo.Extensions;
using Common.Mongo.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.Threading;
using Mongo.ManualTests.Mongo;

namespace Mongo.ManualTests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var mongoSection = Configuration.GetSection("Mongo");
            services.Configure<MongoDbOptions>(mongoSection);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = nameof(ManualTests), Version = "v1" });
            });

            services.AddMongo<TestMongoSeeder>(mongoSection.Get<MongoDbOptions>());
            services.AddScoped<TestMongoRepository>();
        }

        public void Configure(IApplicationBuilder app, IMongoDbInitializer initializer)
        {
            new JoinableTaskFactory(new JoinableTaskContext())
                .RunAsync(async () =>
                {
                    await initializer.InitializeAsync();
                });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mongo.ManualTests v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
