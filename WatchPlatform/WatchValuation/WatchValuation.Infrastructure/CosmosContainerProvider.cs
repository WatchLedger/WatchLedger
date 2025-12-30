using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace WatchValuation.Infrastructure;

public interface ICosmosContainerProvider
{
    Container GetContainer();
}

public class CosmosContainerProvider(IOptions<CosmosDbOptions> options) : ICosmosContainerProvider
{
    private readonly CosmosDbOptions _options = options.Value;
    private CosmosClient? _client;
    private Container? _container;

    public Container GetContainer()
    {
        if (_container is not null)
            return _container;

        _client ??= new CosmosClient(_options.CosmosConnectionString);
        var database = _client.GetDatabase(_options.DatabaseName);
        _container = database.GetContainer(_options.ContainerName)
            ?? throw new InvalidOperationException("Failed to get Cosmos DB container.");

        return _container;
    }
}
