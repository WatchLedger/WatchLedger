using System;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;

namespace WatchValuation.Storage;

public class ValuationCacheRepository : IValuationCacheRepository
{
    public Task<CachedValuation?> GetCachedValuationAsync(string watchReference)
    {
        throw new NotImplementedException();
    }

    public Task SetCachedValuationAsync(CachedValuation cachedValuation)
    {
        throw new NotImplementedException();
    }
}
