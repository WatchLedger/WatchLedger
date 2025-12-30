using Microsoft.Azure.Cosmos;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Extensions.Options;
using WatchValuation.Infrastructure;

namespace WatchValuation.Storage;

public class BrandsCacheRepository(ICosmosContainerProvider _containerProvider) : IBrandsCacheRepository
{
    public async Task<CachedBrands?> GetCachedBrandsAsync()
    {
        try
        {
            var container = _containerProvider.GetContainer();
            var response = await container.ReadItemAsync<CachedBrands>(
                partitionKey: new PartitionKey("BrandsCache"),
                id: "BrandsCache");
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task SetCachedBrandsAsync(CachedBrands cachedBrands)
    {
        try
        {
            var container = _containerProvider.GetContainer();
            await container.UpsertItemAsync(
                item: cachedBrands,
                partitionKey: new PartitionKey("BrandsCache"));
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
        }
    }

}
