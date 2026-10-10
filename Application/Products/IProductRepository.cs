using Grapgql.Domain.Products;

namespace Grapgql.Application.Products;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);
}
