using Grapgql.Api.GraphQL;
using Grapgql.Api.Products;
using Grapgql.Application.Products;
using Grapgql.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddRedisProductCaching(builder.Configuration);
builder.Services.AddCosmosInfrastructure(builder.Configuration);
builder.Services
    .AddGraphQLServer()
    .AddQueryType<ProductQueries>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapGraphQL();
app.MapProductEndpoints();
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.Run();
