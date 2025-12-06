using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace WatchValuation.Storage;

public class ValuationCacheRepository(IConfiguration _configuration) : IValuationCacheRepository
{
    public async Task<CachedValuation?> GetCachedValuationAsync(string watchReference)
    {
        try
        {
            var container = GetCosmosContainer(_configuration);
            var response = await container.ReadItemAsync<CachedValuation>(
                partitionKey: new PartitionKey(watchReference),
                id: watchReference);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task SetCachedValuationAsync(CachedValuation cachedValuation)
    {
        try
        {
            var container = GetCosmosContainer(_configuration);
            await container.UpsertItemAsync(
                item: cachedValuation,
                partitionKey: new PartitionKey(cachedValuation.id));
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
        }
    }

    private static Container GetCosmosContainer(IConfiguration _configuration)
    {
        var constring = _configuration["watchplatformcache-connectionstring"];
        var client = new CosmosClient(constring);
        var database = client.GetDatabase("watchplatform");
        var container = database.GetContainer("WatchPlatformCache");

        if (container is null)
            throw new Exception("Failed to get Cosmos DB container.");
        
        return container;
    }
}
