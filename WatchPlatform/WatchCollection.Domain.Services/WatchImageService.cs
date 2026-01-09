using System;
using System.Linq;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Domain.Services.Validators;
using WatchCollection.Domain.Services.Exceptions;

namespace WatchCollection.Domain.Services;

public class WatchImageService(IWatchImageRepository _watchImageRepository, IBlobStorageService _blobStorageService, IWatchRepository _watchRepository) : IWatchImageService
{

    public async Task<WatchImageResponseContract> UploadImageAsync(string ownerIdString, Guid watchId, string fileName, string contentType, long fileSize, bool isPrimary, Stream imageStream)
    {
        FileValidator.ValidateImageFile(fileName, contentType, fileSize);

        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");
        
        if (watch.OwnerUserId.ToString() != ownerIdString)
            throw new UnauthorizedAccessException("You do not have permission to upload images for this watch.");

        var existingImages = await _watchImageRepository.GetAllImagesByWatchIdAsync(watchId);
        
        if (existingImages.Count >= 25)
            throw new DomainInvalidOperationException("Maximum of 25 images allowed per watch.");
        
        var shouldBePrimary = isPrimary || existingImages.Count == 0; // The very first image will always be primary
        
        // If this should be primary, flip the old primary to false first
        if (shouldBePrimary && existingImages.Any())
        {
            var oldPrimary = existingImages.FirstOrDefault(img => img.IsPrimary);
            if (oldPrimary is not null)
                await _watchImageRepository.UpdatePrimaryImageAsync(watchId, oldPrimary.ImageId, false);
        }
        
        var updatedFileName = Guid.NewGuid().ToString() + "_" + fileName;
        var imageUrl = await _blobStorageService.UploadImageAsync(updatedFileName, imageStream);

        var model = new WatchImageModel{
            ImageId = Guid.NewGuid(),
            WatchId = watchId,
            BlobUrl = imageUrl,
            FileName = updatedFileName,
            FileSize = fileSize,
            ContentType = contentType,
            IsPrimary = shouldBePrimary,
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

    public async Task DeleteImageAsync(string ownerIdString, Guid watchId, Guid imageId)
    {
        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");
        
        if (watch.OwnerUserId.ToString() != ownerIdString)
            throw new UnauthorizedAccessException("You do not have permission to delete images for this watch.");

        // First check if the deleted image is primary
        var images = await _watchImageRepository.GetAllImagesByWatchIdAsync(watchId);
        var target = images.FirstOrDefault(i => i.ImageId == imageId);
        if (target is null)
        {
            _ = await _watchImageRepository.DeleteWatchImageDataAsync(watchId, imageId);
            return;
        }

        var wasPrimary = target.IsPrimary;

        var blobUrl = await _watchImageRepository.DeleteWatchImageDataAsync(watchId, imageId);
        await _blobStorageService.DeleteImageAsync(blobUrl);

        // Auto set a new primary if the deleted one was primary
        if (wasPrimary)
        {
            var remaining = images.Where(i => i.ImageId != imageId).ToList();
            if (remaining.Count > 0)
            {
                var candidate = remaining
                    .OrderByDescending(i => i.UploadedAt)
                    .First();
                await _watchImageRepository.UpdatePrimaryImageAsync(watchId, candidate.ImageId, true);
            }
        }
    }

    public async Task<WatchImageResponseContract> SetMainImageAsync(string ownerIdString, Guid watchId, Guid imageId)
    {
        var watch = await _watchRepository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId, "Watch not found");

        if (watch.OwnerUserId.ToString() != ownerIdString)
            throw new UnauthorizedAccessException("You do not have permission to set the main image for this watch.");

        // ensure the current primary is set to false and then set the new one to true
        var images = await _watchImageRepository.GetAllImagesByWatchIdAsync(watchId);
        var currentPrimary = images.FirstOrDefault(i => i.IsPrimary);
        if (currentPrimary is not null && currentPrimary.ImageId != imageId)
        {
            await _watchImageRepository.UpdatePrimaryImageAsync(watchId, currentPrimary.ImageId, false);
        }

        var updatedEntity = await _watchImageRepository.UpdatePrimaryImageAsync(watchId, imageId, true);
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
