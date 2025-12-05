//using Microsoft.Azure.Cosmos;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace WatchValuation.Storage;

public class BrandsCacheRepository(IConfiguration _configuration) : IBrandsCacheRepository
{
    public Task<CachedBrands?> GetCachedBrandsAsync()
    {
        var constring = _configuration["watchplatformcache-connectionstring"];
        System.Console.WriteLine(constring);
        throw new NotImplementedException();    }

    public Task SetCachedBrandsAsync(CachedBrands cachedBrands)
    {
        throw new NotImplementedException();
    }
}
