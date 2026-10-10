using Grapgql.Domain.Products;

namespace Grapgql.Application.Products;

public sealed class GetProductsHandler(IProductRepository productRepository)
{
    public Task<IReadOnlyList<Product>> HandleAsync(CancellationToken cancellationToken) =>
        productRepository.GetAllAsync(cancellationToken);
}
