using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchService
{
    Task<WatchResponseContract> CreateWatch(string ownerIdString, WatchRequestContract contract);
    Task<WatchResponseContract?> GetWatchById(string ownerIdString, Guid guid);
    Task<IEnumerable<WatchResponseContract>> GetAll(string ownerIdString);
    Task<IEnumerable<WatchResponseContract>> GetWatchesByBrand(string ownerIdString, string brand);
    Task<WatchResponseContract> UpdateWatch(string ownerIdString, Guid watchId, WatchRequestContract contract);
    Task DeleteWatch(string ownerIdString, Guid watchId);
    Task<IEnumerable<string>> GetWatchBrands();
}
