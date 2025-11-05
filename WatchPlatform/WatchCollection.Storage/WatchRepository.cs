using System;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
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
        var watch = await _context.FindAsync<Watch>(watchId);
        return watch;
    }

    public async Task<IEnumerable<Watch>> GetAll()
    {
        var watches = await _context.Watches.ToListAsync();
        return watches;
    }

    public async Task<Watch?> UpdateWatch(Guid watchId, Watch watch)
    {
        var existingWatch = await _context.FindAsync<Watch>(watchId);

        if (existingWatch is null)
            return null;

        existingWatch.Brand = watch.Brand;
        existingWatch.Model = watch.Model;
        existingWatch.ReferenceNumber = watch.ReferenceNumber;
        existingWatch.SerialNumber = watch.SerialNumber;
        existingWatch.YearOfProduction = watch.YearOfProduction;
        existingWatch.Condition = watch.Condition;
        existingWatch.Description = watch.Description;
        existingWatch.PurchasePrice = watch.PurchasePrice;
        existingWatch.PurchaseDate = watch.PurchaseDate;
        existingWatch.IsForSale = watch.IsForSale;
        existingWatch.UpdatedAt = watch.UpdatedAt;

        _context.Entry(existingWatch).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return existingWatch;
    }
}
