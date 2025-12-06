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

    public Task DeleteAdvertisementAsync(Guid advertisementId)
    {
        throw new NotImplementedException();
    }

    public async Task<Advertisement> GetAdvertisementByIdAsync(Guid advertisementId)
    {
        var advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.AdvertisementId == advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions();
        return advertisement;
    }

    public Task<IEnumerable<Advertisement>> GetAllAdvertisementsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Advertisement> UpdateAdvertisementAsync(Advertisement advertisement)
    {
        throw new NotImplementedException();
    }
}
