using Grapgql.Domain.Products;

namespace Grapgql.Api.Products;

/// <summary>A product exposed by the HTTP API.</summary>
public sealed record ProductResponse(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string Category)
{
    public static ProductResponse FromDomain(Product product) =>
        new(product.Id, product.Name, product.Description, product.Price, product.Category);
}
