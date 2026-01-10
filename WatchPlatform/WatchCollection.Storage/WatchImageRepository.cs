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

    public async Task<WatchImage> UpdatePrimaryImageAsync(Guid watchId, Guid imageId, bool isPrimary)
    {
        var image = await _dbContext.WatchImages
            .FirstOrDefaultAsync(wi => wi.WatchId == watchId && wi.ImageId == imageId) ??
            throw new WatchImageNotFoundException(imageId, "Watch image not found");
        
        image.IsPrimary = isPrimary;
        _dbContext.WatchImages.Update(image);
        await _dbContext.SaveChangesAsync();
        return image;
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
