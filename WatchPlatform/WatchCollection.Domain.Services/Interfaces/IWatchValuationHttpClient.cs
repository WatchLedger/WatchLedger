using System;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchValuationHttpClient
{
    Task<decimal> GetWatchValuationAsync(string referenceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetWatchBrandsAsync(CancellationToken cancellationToken = default);
}
