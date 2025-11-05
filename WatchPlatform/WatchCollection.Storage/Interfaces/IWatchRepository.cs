using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IWatchRepository
{
    Task<Watch> Create(Watch watch);
    Task<Watch?> GetWatchById(Guid watchId);
    Task<IEnumerable<Watch>> GetAll();
    Task<Watch?> UpdateWatch(Guid watchId, Watch watch);

    void DeleteWatch(Guid watchId);
}
