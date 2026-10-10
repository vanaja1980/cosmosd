using Grapgql.Application.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Grapgql.Api.Products;

public static class ProductEndpoints
{
    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/api/products", GetProductsAsync)
            .WithName("GetProducts")
            .WithSummary("Get all products")
            .WithDescription("Returns all products stored in the configured Cosmos DB container.")
            .Produces<ProductListResponse>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<Ok<ProductListResponse>> GetProductsAsync(
        GetProductsHandler handler,
        CancellationToken cancellationToken)
    {
        var products = await handler.HandleAsync(cancellationToken);
        var response = new ProductListResponse(
            products.Select(ProductResponse.FromDomain).ToArray());

        return TypedResults.Ok(response);
    }
}
