using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Azure.Cosmos;
using WatchValuation.Infrastructure;

namespace WatchValuation.Storage;

public class ValuationCacheRepository(ICosmosContainerProvider _containerProvider) : IValuationCacheRepository
{
    public async Task<CachedValuation?> GetCachedValuationAsync(string watchReference)
    {
        try
        {
            var container = _containerProvider.GetContainer();
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
            var container = _containerProvider.GetContainer();
            await container.UpsertItemAsync(
                item: cachedValuation,
                partitionKey: new PartitionKey(cachedValuation.id));
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
        }
    }

}
