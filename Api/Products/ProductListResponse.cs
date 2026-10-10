namespace Grapgql.Api.Products;

/// <summary>A list of products returned by the HTTP API.</summary>
public sealed record ProductListResponse(IReadOnlyList<ProductResponse> Products);
