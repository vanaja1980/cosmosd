namespace Grapgql.Infrastructure.Persistence.Cosmos;

public sealed class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";
    public const string ValidationMessage =
        "CosmosDb must define an HTTPS Endpoint and Key.";

    public string Endpoint { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = "store";
    public string ContainerName { get; init; } = "products";
    public bool ProvisionOnStartup { get; init; }

    public bool IsValid =>
        Uri.TryCreate(Endpoint, UriKind.Absolute, out var endpoint) &&
        endpoint.Scheme == Uri.UriSchemeHttps &&
        !string.IsNullOrWhiteSpace(Key);
}
