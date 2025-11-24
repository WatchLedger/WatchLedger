using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Domain.Services;

public class WatchImageService(IWatchImageRepository _watchImageRepository, IBlobStorageService _blobStorageService, IWatchService _watchService) : IWatchImageService
{

    public async Task<WatchImageResponseContract> UploadImageAsync(Guid watchId, string fileName, string contentType, long fileSize, WatchImageRequestContract contract, Stream imageStream)
    {
        FileValidator.ValidateImageFile(fileName, contentType, fileSize);


        var watch = await _watchService.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException();

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
        var watch = await _watchService.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException();

        var entities =  await _watchImageRepository.GetAllImagesByWatchIdAsync(watchId);
        return entities.Select(e => e.AsModel().AsResponseContract()).ToList();
    }

    public async Task DeleteImageAsync(Guid watchId, Guid imageId)
    {
        var watch = await _watchService.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException();

        var blobUrl = await _watchImageRepository.DeleteWatchImageDataAsync(watchId, imageId);
        await _blobStorageService.DeleteImageAsync(blobUrl);
    }

    public async Task DeleteImagesByWatchIdAsync(Guid watchId)
    {
        var watch = await _watchService.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException();

        var fileNames = await _watchImageRepository.DeleteImagesByWatchIdAsync(watchId);
        foreach (var filename in fileNames)
        {
            await _blobStorageService.DeleteImageAsync(filename);
        }
    }

    public async Task<WatchImageResponseContract> SetMainImageAsync(Guid watchId, Guid imageId)
    {
        var watch = await _watchService.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException();

        var updatedEntity = await _watchImageRepository.SetMainImageAsync(watchId, imageId);
        return updatedEntity.AsModel().AsResponseContract();
    }
}

public class FileValidator
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public static void ValidateImageFile(string fileName, string contentType, long fileSize)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type cannot be empty.");

            if (fileSize <= 0)
                throw new ArgumentException("File size must be greater than 0.");

            if (fileSize > MaxFileSizeBytes)
                throw new ArgumentException($"File size exceeds maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");

            if (!AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
                throw new ArgumentException($"Content type '{contentType}' is not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}");
        }
    }
