using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Domain.Services.Validators;

namespace WatchCollection.Domain.Services;

public class WatchImageService(IWatchImageRepository _watchImageRepository, IBlobStorageService _blobStorageService, IWatchRepository _watchRepository) : IWatchImageService
{

    public async Task<WatchImageResponseContract> UploadImageAsync(Guid watchId, string fileName, string contentType, long fileSize, bool isPrimary, Stream imageStream)
    {
        FileValidator.ValidateImageFile(fileName, contentType, fileSize);

        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");

        var updatedFileName = Guid.NewGuid().ToString() + "_" + fileName; // Ensures unique filenames
        var imageUrl = await _blobStorageService.UploadImageAsync(updatedFileName, imageStream);

        var model = new WatchImageModel{
            ImageId = Guid.NewGuid(),
            WatchId = watchId,
            BlobUrl = imageUrl,
            FileName = updatedFileName,
            FileSize = fileSize,
            ContentType = contentType,
            IsPrimary = isPrimary,
        };

        var entity = model.AsEntity();
        var addedEntity = await _watchImageRepository.AddWatchImageAsync(entity);

        return addedEntity.AsModel().AsContract();
    }

    public async Task<IEnumerable<WatchImageResponseContract>> GetAllImagesByWatchIdAsync(Guid watchId)
    {
        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");

        var entities =  await _watchImageRepository.GetAllImagesByWatchIdAsync(watchId);
        return entities.Select(e => e.AsModel().AsContract()).ToList();
    }

    public async Task DeleteImageAsync(Guid watchId, Guid imageId)
    {
        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");

        var blobUrl = await _watchImageRepository.DeleteWatchImageDataAsync(watchId, imageId);
        await _blobStorageService.DeleteImageAsync(blobUrl);
    }

    public async Task<WatchImageResponseContract> SetMainImageAsync(Guid watchId, Guid imageId)
    {
        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");

        var updatedEntity = await _watchImageRepository.SetMainImageAsync(watchId, imageId);
        return updatedEntity.AsModel().AsContract();
    }

    public async Task<IEnumerable<string>> GetFilenamesByWatchIdAsync(Guid watchId)
    {
        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
        throw new WatchNotFoundException(watchId);

        var fileNames = await _watchImageRepository.GetFilenamesByWatchIdAsync(watchId);
        return fileNames;
    }

    public async Task DeleteBlobsAsync(string filename)
    {
        await _blobStorageService.DeleteImageAsync(filename);
    }
}
