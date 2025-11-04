using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IWatchRepository
{
    Watch Create(Watch watch);
    Watch? GetWatchById(Guid guid);
    IEnumerable<Watch> GetAll();
}
