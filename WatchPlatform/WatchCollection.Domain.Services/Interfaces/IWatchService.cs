using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchService
{
    Task<WatchResponseContract> CreateWatch(WatchRequestContract contract);
    Task<WatchResponseContract?> GetWatchById(Guid guid);
    Task<IEnumerable<WatchResponseContract>> GetAll();
    Task<WatchResponseContract> UpdateWatch(Guid watchId, WatchRequestContract contract);
    Task DeleteWatch(Guid watchId);
}
