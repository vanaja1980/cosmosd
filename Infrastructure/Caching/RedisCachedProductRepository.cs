using System.Text.Json;
using Grapgql.Application.Products;
using Grapgql.Domain.Products;
using Grapgql.Infrastructure.Persistence.Cosmos;
using Microsoft.Extensions.Caching.Distributed;

namespace Grapgql.Infrastructure.Caching;

public sealed class RedisCachedProductRepository(
    CosmosProductRepository cosmosRepository,
    IDistributedCache cache) : IProductRepository
{
    private const string CacheKey = "products:all:v1";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cachedProducts = await cache.GetAsync(CacheKey, cancellationToken);
        if (cachedProducts is not null)
        {
            return JsonSerializer.Deserialize<Product[]>(cachedProducts)
                ?? throw new InvalidDataException("The cached product list could not be deserialized.");
        }

        var products = await cosmosRepository.GetAllAsync(cancellationToken);
        var serializedProducts = JsonSerializer.SerializeToUtf8Bytes(products);
        await cache.SetAsync(
            CacheKey,
            serializedProducts,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            },
            cancellationToken);

        return products;
    }
}
