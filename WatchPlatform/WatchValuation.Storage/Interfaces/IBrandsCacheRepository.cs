using System;
using WatchValuation.Storage.Records;

namespace WatchValuation.Storage.Interfaces;

public interface IBrandsCacheRepository
{
    Task<CachedBrands?> GetCachedBrandsAsync();
    Task SetCachedBrandsAsync(CachedBrands cachedBrands);
}
