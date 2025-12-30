using System;
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
        var advertisement =  _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions();
        _context.Advertisements.Remove(advertisement);
        await _context.SaveChangesAsync();
    }

    public async Task<Advertisement> GetAdvertisementByIdAsync(Guid advertisementId)
    {
        var advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.AdvertisementId == advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions();
        return advertisement;
    }

    public async Task<IEnumerable<Advertisement>> GetAllAdvertisementsAsync()
    {   
        var advertisements =  await _context.Advertisements.ToListAsync();
        return advertisements;
    }

    public async Task<Advertisement> UpdateAdvertisementAsync(Advertisement advertisement)
    {
        var existingAdvertisement =  _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == advertisement.AdvertisementId);
        if (existingAdvertisement is null)
            throw new AdvertisementNotFoundExceptions();
        
        var entry = _context.Entry(existingAdvertisement);
        entry.CurrentValues.SetValues(advertisement);

        entry.Property(e => e.CreatedAt).IsModified = false;
        entry.Property(e => e.UpdatedAt).IsModified = false;

        await _context.SaveChangesAsync();
        return existingAdvertisement;
    }
}
