using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IWatchRepository
{
    Task<Watch> Create(Watch watch);
    Task<Watch?> GetWatchById(Guid watchId);
    Task<IEnumerable<Watch>> GetAll();
    Task<IEnumerable<Watch>> GetWatchesByBrand(string brand);
    Task<Watch> UpdateWatch(Guid watchId, Watch watch);
    Task DeleteWatch(Guid watchId);
}
