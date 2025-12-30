using System;
using System.Net;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IWatchImageRepository
{
    Task<WatchImage> AddWatchImageAsync(WatchImage watchImage);
    Task<List<WatchImage>> GetAllImagesByWatchIdAsync(Guid watchId);
    Task<string> DeleteWatchImageDataAsync(Guid watchId, Guid imageId); // Returns the filename of the deleted image to facilitate blob deletion
    Task<List<string>> DeleteImagesByWatchIdAsync(Guid watchId);
    Task<WatchImage> SetMainImageAsync(Guid watchId, Guid imageId);
    Task<IEnumerable<string>> GetFilenamesByWatchIdAsync(Guid watchId);
}
