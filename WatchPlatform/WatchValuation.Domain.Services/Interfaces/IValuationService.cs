using System;
using WatchValuation.Api.Contracts;

namespace WatchValuation.Domain.Services.Interfaces;

public interface IValuationService
{
    Task<ValuationResponseContract> GetWatchValuationFromCacheAsync(string referenceNumber);
}
