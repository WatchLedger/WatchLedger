using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchImageService
{
    Task<WatchImageResponseContract> UploadImageAsync(Guid watchId, string fileName, string contentType, long fileSize, WatchImageRequestContract contract, Stream imageStream);
    Task<List<WatchImageResponseContract>> GetAllImagesByWatchIdAsync(Guid watchId);
    Task DeleteImageAsync(Guid watchId, Guid imageId);
}
