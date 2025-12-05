//using Microsoft.Azure.Cosmos;
//using Microsoft.Extensions.Options;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;

namespace WatchValuation.Storage;

public class BrandsCacheRepository : IBrandsCacheRepository
{
    public Task<CachedBrands?> GetCachedBrandsAsync()
    {
        throw new NotImplementedException();
    }

    public Task SetCachedBrandsAsync(CachedBrands cachedBrands)
    {
        throw new NotImplementedException();
    }
}
