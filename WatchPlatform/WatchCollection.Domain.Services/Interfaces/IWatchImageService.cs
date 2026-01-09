using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchImageService
{
    Task<WatchImageResponseContract> UploadImageAsync(string ownerIdString, Guid watchId, string fileName, string contentType, long fileSize, bool isPrimary, Stream imageStream);
    Task<IEnumerable<WatchImageResponseContract>> GetAllImagesByWatchIdAsync(Guid watchId);
    Task<IEnumerable<string>> GetFilenamesByWatchIdAsync(Guid watchId);
    Task DeleteImageAsync(string ownerIdString, Guid watchId, Guid imageId);
    Task DeleteBlobsAsync(string filename);
    Task<WatchImageResponseContract> SetMainImageAsync(string ownerIdString, Guid watchId, Guid imageId);
}
