using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IWatchImageRepository
{
    Task<WatchImage> AddWatchImageAsync(WatchImage watchImage);
    Task<List<WatchImage>> GetAllImagesByWatchIdAsync(Guid watchId);
    Task DeleteAsync(Guid id);
}
