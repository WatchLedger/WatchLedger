using System;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class AdvertisementRepository(WatchServiceDbContext _context) : IAdvertisementRepository
{
    public async Task<Guid> CreateAdvertisementAsync(Advertisement advertisement)
    {
        var created = await _context.Advertisements.AddAsync(advertisement);
        await _context.SaveChangesAsync();
        return created.Entity.AdvertisementId;
    }

    public async Task<Advertisement> GetAdvertisementByIdAsync(Guid advertisementId)
    {
        var advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.AdvertisementId == advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions();
        return advertisement;
    }
}
