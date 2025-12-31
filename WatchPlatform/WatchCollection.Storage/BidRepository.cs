using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class BidRepository(WatchServiceDbContext _context) : IBidRepository
{
    /// <summary>
    /// Adds a new bid for the given advertisement and enforces that its amount
    /// is strictly higher than the current highest bid.
    /// Uses a serializable transaction so validation and insertion happen
    /// atomically under concurrency.
    /// </summary>
    public async Task<Bid> AddBidAsync(Bid bid)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);
            
        var highestBid = await _context.Bids
            .Where(b => b.AdvertisementId == bid.AdvertisementId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync();

        if(highestBid is not null && bid.Amount <= highestBid.Amount)
            throw new BidTooLowException($"Bid amount must be higher than the current highest bid of {highestBid.Amount}.");
        
        
        var created = await _context.Bids.AddAsync(bid);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return created.Entity;
    }

    public async Task<List<Bid>> GetAllBidsByAdvertisementId(Guid advertisementId)
    {
        var bids = await _context.Bids
            .Where(b => b.AdvertisementId == advertisementId)
            .ToListAsync();
        return bids;
    }

    public async Task RemoveBidAsync(Guid bidId)
    {
        var entity = _context.Bids
            .FirstOrDefault(b => b.BidId == bidId) ??
            throw new BidNotFoundExceptions(bidId, "Bid not found");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Bid?> GetHighestBidForAdvertisementAsync(Guid advertisementId)
    {
        return await _context.Bids
            .Where(b => b.AdvertisementId == advertisementId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync();
    }
}
