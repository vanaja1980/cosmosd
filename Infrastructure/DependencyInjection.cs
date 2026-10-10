using Grapgql.Application.Products;
using Grapgql.Infrastructure.Caching;
using Grapgql.Infrastructure.Persistence.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Grapgql.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRedisProductCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["Redis:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure Redis:ConnectionString or the Redis__ConnectionString environment variable.");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "grapgql:";
        });

        return services;
    }

    public static IServiceCollection AddCosmosInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<CosmosDbOptions>()
            .Bind(configuration.GetSection(CosmosDbOptions.SectionName))
            .Validate(options => options.IsValid, CosmosDbOptions.ValidationMessage)
            .ValidateOnStart();

        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
            return new CosmosClient(
                options.Endpoint,
                options.Key,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                });
        });

        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
            return serviceProvider.GetRequiredService<CosmosClient>()
                .GetContainer(options.DatabaseName, options.ContainerName);
        });
        services.AddScoped<CosmosProductRepository>();
        services.AddScoped<IProductRepository, RedisCachedProductRepository>();
        services.AddHostedService<CosmosDbInitializer>();

        return services;
    }
}
