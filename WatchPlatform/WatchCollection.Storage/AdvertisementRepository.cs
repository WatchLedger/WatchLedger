using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class AdvertisementRepository(WatchServiceDbContext _context) : IAdvertisementRepository
{
    public async Task<Advertisement> CreateAdvertisementAsync(Advertisement advertisement)
    {
        var createdAdvertisement = advertisement;
        await  _context.Advertisements.AddAsync(createdAdvertisement);
        await _context.SaveChangesAsync();
        return createdAdvertisement;
    }

    public async Task DeleteAdvertisementAsync(Guid advertisementId)
    {
        var advertisement =  _context.Advertisements
            .FirstOrDefault(a => a.AdvertisementId == advertisementId) ??
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found.");

        _context.Advertisements.Remove(advertisement);
        await _context.SaveChangesAsync();
    }

    public async Task<Advertisement> GetAdvertisementByIdAsync(Guid advertisementId)
    {
        var advertisement = await _context.Advertisements
            .FirstOrDefaultAsync(a => a.AdvertisementId == advertisementId) ??
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found.");

        var watch = await _context.Watches
            .SingleOrDefaultAsync(w => w.WatchId == advertisement.WatchId);

        if (watch is not null)
        {
            var images = await _context.WatchImages
                .Where(img => img.WatchId == advertisement.WatchId && img.IsPrimary)
                .OrderByDescending(img => img.UploadedAt)
                .Take(1)
                .Concat(
                    _context.WatchImages
                        .Where(img => img.WatchId == advertisement.WatchId && !img.IsPrimary)
                        .OrderByDescending(img => img.UploadedAt)
                        .Take(9))
                .ToListAsync();

            watch.WatchImages = images;
            advertisement.Watch = watch;
        }

        return advertisement;
    }

    public async Task<IEnumerable<Advertisement>> GetAllAdvertisementsAsync()
    {
        var advertisements = await _context.Advertisements.ToListAsync();

        if (advertisements.Count == 0)
            return advertisements;

        var watchIds = advertisements.Select(a => a.WatchId).ToList();

        var watches = await _context.Watches
            .Where(w => watchIds.Contains(w.WatchId))
            .ToListAsync();

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

        var watchById = watches.ToDictionary(w => w.WatchId);
        foreach (var advertisement in advertisements)
        {
            if (watchById.TryGetValue(advertisement.WatchId, out var watch))
            {
                advertisement.Watch = watch;
            }
        }

        return advertisements;
    }

    public async Task<Advertisement> UpdateAdvertisementAsync(Advertisement advertisement)
    {
        var existingAdvertisement =  _context.Advertisements
            .FirstOrDefault(a => a.AdvertisementId == advertisement.AdvertisementId) ??
            throw new AdvertisementNotFoundExceptions(advertisement.AdvertisementId, "Advertisement not found.");
        
        var entry = _context.Entry(existingAdvertisement);
        entry.CurrentValues.SetValues(advertisement);

        entry.Property(e => e.CreatedAt).IsModified = false;
        entry.Property(e => e.UpdatedAt).IsModified = false;

        await _context.SaveChangesAsync();
        return existingAdvertisement;
    }

    public async Task UpdateViewCountAsync(Guid advertisementId, int newViewCount)
    {
        var advertisement =  _context.Advertisements
            .FirstOrDefault(a => a.AdvertisementId == advertisementId) ??
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found.");

        advertisement.ViewCount = newViewCount;
        _context.Advertisements.Update(advertisement);
        await _context.SaveChangesAsync();
    }
}
