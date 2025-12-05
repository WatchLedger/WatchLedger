using Microsoft.Azure.Cosmos;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Extensions.Configuration;

namespace WatchValuation.Storage;

public class BrandsCacheRepository(IConfiguration _configuration) : IBrandsCacheRepository
{
    public async Task<CachedBrands?> GetCachedBrandsAsync()
    {
        try
        {
            var container = GetCosmosContainer(_configuration);
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
            var container = GetCosmosContainer(_configuration);
            await container.UpsertItemAsync(
                item: cachedBrands,
                partitionKey: new PartitionKey("BrandsCache"));
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
