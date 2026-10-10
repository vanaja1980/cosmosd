# Grapgql Products API

A minimal ASP.NET Core GraphQL API that reads products from Azure Cosmos DB for NoSQL.

## Architecture

- `Domain` contains the product model and has no infrastructure dependencies.
- `Application` contains the product repository port and the use case for listing products.
- `Infrastructure` implements the repository port with Cosmos DB, decorates it with Redis
  cache-aside behavior, owns service configuration, and provisions the development database and
  container through a hosted service.
- `GraphQL` and `Api` are HTTP adapters; both delegate product listing to the application use case.

Dependencies point inward: the Cosmos implementation depends on the application port, while the
domain remains independent of both the web API and Cosmos DB.

## API documentation

The GraphQL endpoint is available at `/graphql`. OpenAPI describes the REST API, not GraphQL
schemas, so the app also exposes `GET /api/products` for the same product-listing use case.
In Development, open `/scalar/v1` for interactive API documentation or `/openapi/v1.json` for the
OpenAPI document. A ready-to-run request is in [Products.http](./Products.http).

## Configuration

Only the Cosmos account endpoint and key are required for Cosmos DB. The Development configuration
defaults to database `store` and container `products`, and creates them automatically on startup
with `/category` as the partition key. Override the names in configuration if you want different
names.

The product list is cached in Redis for one minute in all environments. Development uses the
`localhost:6379` connection string from `appsettings.Development.json`.

Install and start Docker Desktop, then run this from the project directory to start Redis:

```powershell
docker compose up -d redis
```

Stop Redis when finished with `docker compose down`. Configure `Redis:ConnectionString` or set
`Redis__ConnectionString` to use a different Redis server. Keep secured connection strings outside
source control. Redis must be available before starting the app; cache errors are surfaced rather
than silently ignored.

The cache is read-through: the first product-list request loads Cosmos DB and stores the list in
Redis; subsequent requests use that value until its one-minute absolute expiry. As this
app only reads products, no write-side invalidation is needed yet. When product mutations are added,
they should invalidate this cache key after successful writes.

Set the Cosmos endpoint and key as environment variables before starting the application:

| Variable | Description |
| --- | --- |
| `CosmosDb__Endpoint` | Cosmos DB account endpoint |
| `CosmosDb__Key` | Cosmos DB account key |
| `Redis__ConnectionString` | Optional override for the Redis connection string (`localhost:6379` in Development) |

For local development without Azure, run the Azure Cosmos DB Emulator. Keep the key out of source
control; use .NET User Secrets. This project already has a `UserSecretsId`, so do not run
`dotnet user-secrets init` again. Start the emulator first, then set its actual key (the example
below is a placeholder, not a usable key):

```powershell
dotnet user-secrets set "CosmosDb:Endpoint" "https://localhost:8081/"
dotnet user-secrets set "CosmosDb:Key" "<paste-the-key-shown-by-your-emulator>"
dotnet run
```

Replace the key placeholder with the emulator's actual primary key. Do not include angle brackets
or paste the whole connection string: the Cosmos SDK expects the base64-encoded key itself.

Run these commands from the directory containing `grapgql.csproj`. The emulator must be installed
and running at that endpoint. If startup still fails, check the first error printed by `dotnet run`;
missing settings, an unreachable emulator, an invalid key, and an untrusted local TLS certificate
need different fixes.

The application creates the configured database and container in Development. In other environments,
`ProvisionOnStartup` defaults to `false`; provision the database and container separately, or
explicitly enable startup provisioning for a development/test deployment. Product documents use
camel-case JSON properties:

```json
{
  "id": "product-1",
  "name": "Desk lamp",
  "description": "Adjustable LED desk lamp",
  "price": 39.99,
  "category": "Lighting"
}
```

The default container uses `/category` as its partition key. The query reads across all partitions.

## Run

```powershell
$env:CosmosDb__Endpoint = "https://your-account.documents.azure.com:443/"
$env:CosmosDb__Key = "<your-account-key>"
dotnet run
```

Open the URL printed by `dotnet run` with `/graphql` appended, then run:

```graphql
{
  products {
    id
    name
    description
    price
    category
  }
}
```
