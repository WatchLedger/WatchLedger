using System;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Extensions.Configuration;

namespace WatchValuation.Storage;

public class ValuationCacheRepository(IConfiguration _configuration) : IValuationCacheRepository
{
    public Task<CachedValuation?> GetCachedValuationAsync(string watchReference)
    {
        var constring = _configuration["watchplatformcache-connectionstring"];
        System.Console.WriteLine(constring);
        throw new NotImplementedException();
    }

    public Task SetCachedValuationAsync(CachedValuation cachedValuation)
    {
        throw new NotImplementedException();
    }
}
