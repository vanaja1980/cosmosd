using Grapgql.Application.Products;
using Grapgql.Domain.Products;
using Microsoft.Azure.Cosmos;

namespace Grapgql.Infrastructure.Persistence.Cosmos;

public sealed class CosmosProductRepository(Container container) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var iterator = container.GetItemQueryIterator<Product>(
            new QueryDefinition("SELECT * FROM c"));

        var products = new List<Product>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            products.AddRange(response);
        }

        return products;
    }
}
