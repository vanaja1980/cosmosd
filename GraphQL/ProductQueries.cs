using Grapgql.Application.Products;
using Grapgql.Domain.Products;
using HotChocolate;

namespace Grapgql.Api.GraphQL;

public sealed class ProductQueries
{
    public Task<IReadOnlyList<Product>> Products(
        [Service] GetProductsHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(cancellationToken);
}
