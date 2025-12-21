using System;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class BidRepository(WatchServiceDbContext _context) : IBidRepository
{
    public async Task<Bid> AddBidAsync(Bid bid)
    {
        var created = await _context.AddAsync(bid);
        await _context.SaveChangesAsync();
        return created.Entity;
    }

    public async Task<List<Bid>> GetAllBidsByAdvertisementId(Guid advertisementId)
    {
        var bids = await _context.Bids.ToListAsync();
        return bids;
    }

    public async Task RemoveBidAsync(Guid bidId)
    {
        var entity = _context.Bids.FirstOrDefault(b => b.BidId == bidId);
        if(entity is null)
            throw new BidNotFoundExceptions();

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
