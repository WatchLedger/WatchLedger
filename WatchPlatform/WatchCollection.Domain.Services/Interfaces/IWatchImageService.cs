using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchImageService
{
    Task<WatchImageResponseContract> UploadImageAsync(Guid watchId, string fileName, string contentType, long fileSize, bool isPrimary, Stream imageStream);
    Task<IEnumerable<WatchImageResponseContract>> GetAllImagesByWatchIdAsync(Guid watchId);
    Task<IEnumerable<string>> GetFilenamesByWatchIdAsync(Guid watchId);
    Task DeleteImageAsync(Guid watchId, Guid imageId);
    Task DeleteBlobsAsync(string filename);
    Task<WatchImageResponseContract> SetMainImageAsync(Guid watchId, Guid imageId);
}
