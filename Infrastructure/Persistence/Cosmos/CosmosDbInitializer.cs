using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Grapgql.Infrastructure.Persistence.Cosmos;

public sealed class CosmosDbInitializer(
    CosmosClient cosmosClient,
    IOptions<CosmosDbOptions> options,
    ILogger<CosmosDbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (!settings.ProvisionOnStartup)
        {
            return;
        }

        logger.LogInformation(
            "Ensuring Cosmos DB database {DatabaseName} and container {ContainerName} exist.",
            settings.DatabaseName,
            settings.ContainerName);

        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(
            settings.DatabaseName,
            cancellationToken: cancellationToken);

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties(settings.ContainerName, "/category"),
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
