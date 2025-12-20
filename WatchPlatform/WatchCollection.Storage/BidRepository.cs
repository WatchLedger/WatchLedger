using System;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class BidRepository : IBidRepository
{
    public Task<Bid> AddBidAsync(Bid bid)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Bid>> GetAllBidsByAdvertisementId(Guid advertisementId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveBidAsync(Bid bid)
    {
        throw new NotImplementedException();
    }
}
