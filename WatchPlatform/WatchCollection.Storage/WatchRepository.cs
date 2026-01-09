using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class WatchRepository(WatchServiceDbContext _context) : IWatchRepository
{
    public async Task<Watch> Create(Watch watch)
    {
        var addedWatch = watch;
        await _context.Watches.AddAsync(watch);
        await _context.SaveChangesAsync();
        return addedWatch;
    }

    public async Task<Watch?> GetWatchById(Guid watchId)
    {
        // Fetch watch and attach up to 10 images: 1 primary + 9 newest non-primary
        var watch = await _context.Watches
            .SingleOrDefaultAsync(w => w.WatchId == watchId);

        if (watch is null)
            return null;

        var images = await _context.WatchImages
            .Where(img => img.WatchId == watchId && img.IsPrimary)
            .OrderByDescending(img => img.UploadedAt)
            .Take(1)
            .Concat(
                _context.WatchImages
                    .Where(img => img.WatchId == watchId && !img.IsPrimary)
                    .OrderByDescending(img => img.UploadedAt)
                    .Take(9))
            .ToListAsync();

        watch.WatchImages = images;
        return watch;
    }

    public async Task<IEnumerable<Watch>> GetAll(Guid ownerId)
    {
        // first get all watches for the owner
        var watches = await _context.Watches
            .Where(w => w.OwnerUserId == ownerId)
            .ToListAsync();

        if (watches.Count == 0)
            return watches;

        var watchIds = watches.Select(w => w.WatchId).ToList();

        // Fetch primary images
        var primaryImages = await _context.WatchImages
            .Where(img => watchIds.Contains(img.WatchId) && img.IsPrimary)
            .OrderByDescending(img => img.UploadedAt)
            .GroupBy(img => img.WatchId)
            .Select(g => g.First())
            .ToListAsync();

        // Map primary images to watches
        var primaryByWatch = primaryImages.ToDictionary(img => img.WatchId);
        // Attach primary images to watches
        foreach (var watch in watches)
        {
            watch.WatchImages = primaryByWatch.TryGetValue(watch.WatchId, out var image)
                ? new List<WatchImage> { image }
                : new List<WatchImage>();
        }

        return watches;
    }

    public async Task<IEnumerable<Watch>> GetWatchesByBrand(Guid ownerId, string brand)
    {
        var watches = await _context.Watches
            .Where(w => w.OwnerUserId == ownerId && w.Brand.ToLower().Contains(brand.ToLower()))
            .ToListAsync();

        if (watches.Count == 0)
            return watches;

        var watchIds = watches.Select(w => w.WatchId).ToList();

        var primaryImages = await _context.WatchImages
            .Where(img => watchIds.Contains(img.WatchId) && img.IsPrimary)
            .OrderByDescending(img => img.UploadedAt)
            .GroupBy(img => img.WatchId)
            .Select(g => g.First())
            .ToListAsync();

        var primaryByWatch = primaryImages.ToDictionary(img => img.WatchId);

        foreach (var watch in watches)
        {
            watch.WatchImages = primaryByWatch.TryGetValue(watch.WatchId, out var image)
                ? new List<WatchImage> { image }
                : new List<WatchImage>();
        }

        return watches;
    }

    public async Task<Watch> UpdateWatch(Guid watchId, Watch watch)
    {
        var existingWatch = await _context
            .FindAsync<Watch>(watchId) ??
            throw new WatchNotFoundException(watchId, "Watch not found.");

        var entry = _context.Entry(existingWatch);
        entry.CurrentValues.SetValues(watch);
        
        // Ensure CreatedAt and UpdatedAt are not modified manually
        entry.Property(e => e.CreatedAt).IsModified = false;
        entry.Property(e => e.UpdatedAt).IsModified = false;
        
        await _context.SaveChangesAsync();
        return existingWatch;
    }

    public async Task DeleteWatch(Guid watchId)
    {
        var existingWatch = await _context
            .FindAsync<Watch>(watchId) ??
            throw new WatchNotFoundException(watchId, "Watch not found.");

        _context.Watches.Remove(existingWatch);
        await _context.SaveChangesAsync();
    }
}
