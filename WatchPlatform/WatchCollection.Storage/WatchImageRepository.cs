using System;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Interfaces;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Storage;

public class WatchImageRepository(WatchServiceDbContext _dbContext) : IWatchImageRepository
{
    public async Task<WatchImage> AddWatchImageAsync(WatchImage watchImage)
    {
        await _dbContext.WatchImages.AddAsync(watchImage);
        await _dbContext.SaveChangesAsync();
        return watchImage;
    }


    public async Task<List<WatchImage>> GetAllImagesByWatchIdAsync(Guid watchId)
    {
        return await _dbContext.WatchImages
            .Where(wi => wi.WatchId == watchId)
            .ToListAsync();
    }

    public async Task<string> DeleteWatchImageDataAsync(Guid watchId, Guid imageId)
    {
        var watchImage = await _dbContext.WatchImages
            .FirstOrDefaultAsync(wi => wi.WatchId == watchId && wi.ImageId == imageId) ??
            throw new WatchImageNotFoundException(imageId, "Watch image not found");

        _dbContext.WatchImages.Remove(watchImage);
        await _dbContext.SaveChangesAsync();

        return watchImage.FileName;
    }

    public async Task<List<string>> DeleteImagesByWatchIdAsync(Guid watchId)
    {
        var watchImages = _dbContext.WatchImages
            .Where(wi => wi.WatchId == watchId)
            .ToList();

        _dbContext.WatchImages.RemoveRange(watchImages);
        await _dbContext.SaveChangesAsync();

        return watchImages.Select(wi => wi.FileName).ToList();
    }

    public async Task<WatchImage> SetMainImageAsync(Guid watchId, Guid imageId)
    {
        var images = await _dbContext.WatchImages
            .Where(wi => wi.WatchId == watchId)
            .ToListAsync();

        if (!images.Any(img => img.ImageId == imageId))
            throw new WatchImageNotFoundException(imageId, "Watch image not found");

        foreach (var img in images)
        {
            img.IsPrimary = img.ImageId == imageId;
        }

        await _dbContext.SaveChangesAsync();

        return images.First(img => img.ImageId == imageId);
    }

    public async Task<IEnumerable<string>> GetFilenamesByWatchIdAsync(Guid watchId)
    {
        var fileNames = await _dbContext.WatchImages
            .Where(wi => wi.WatchId == watchId)
            .Select(wi => wi.FileName)
            .ToListAsync();

        return fileNames;
    }
}
