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
            .FirstOrDefaultAsync(wi => wi.WatchId == watchId && wi.ImageId == imageId);

        if (watchImage is null)
            throw new WatchImageNotFoundException();

        _dbContext.WatchImages.Remove(watchImage);
        await _dbContext.SaveChangesAsync();

        return watchImage.FileName;
    }
}
