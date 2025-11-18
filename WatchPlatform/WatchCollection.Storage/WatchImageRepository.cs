using System;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WatchCollection.Storage;

public class WatchImageRepository(WatchServiceDbContext _dbContext) : IWatchImageRepository
{
    public async Task<WatchImage> AddWatchImageAsync(WatchImage watchImage)
    {
        await _dbContext.WatchImages.AddAsync(watchImage);
        await _dbContext.SaveChangesAsync();
        return watchImage;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<WatchImage>> GetAllImagesByWatchIdAsync(Guid watchId)
    {
        return await _dbContext.WatchImages
            .Where(wi => wi.WatchId == watchId)
            .ToListAsync();
    }
}
