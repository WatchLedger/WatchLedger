using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Mapping;

namespace WatchCollection.Domain.Services;

public class WatchImageService(IWatchImageRepository _watchImageRepository, IBlobStorageService _blobStorageService) : IWatchImageService
{

    public async Task<WatchImageResponseContract> UploadImageAsync(Guid watchId, string fileName, string contentType, long fileSize, WatchImageRequestContract contract, Stream imageStream)
    {
        var updatedFileName = Guid.NewGuid().ToString() + "_" + fileName; // Ensures unique filenames
        var imageUrl = await _blobStorageService.UploadImageAsync(updatedFileName, imageStream);

        var model = new WatchImageModel{
            WatchId = watchId,
            BlobUrl = imageUrl,
            FileName = updatedFileName,
            FileSize = fileSize,
            ContentType = contentType,
            IsPrimary = contract.IsPrimary,
            UploadedAt = DateTimeOffset.UtcNow
        };

        var entity = model.AsEntity();
        var addedEntity = await _watchImageRepository.AddWatchImageAsync(entity);

        return addedEntity.AsModel().AsResponseContract();
    }

    public async Task<List<WatchImageResponseContract>> GetAllImagesByWatchIdAsync(Guid watchId)
    {
        var entities =  await _watchImageRepository.GetAllImagesByWatchIdAsync(watchId);
        return entities.Select(e => e.AsModel().AsResponseContract()).ToList();
    }

    public Task DeleteImageAsync(Guid imageId)
    {
        throw new NotImplementedException();
    }
}
