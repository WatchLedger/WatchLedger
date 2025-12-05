using System;
using WatchValuation.Storage.Records;

namespace WatchValuation.Storage.Interfaces;

public interface IValuationCacheRepository
{
    Task<CachedValuation?> GetCachedValuationAsync(string watchReference);
    Task SetCachedValuationAsync(CachedValuation cachedValuation);
}
