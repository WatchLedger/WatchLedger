using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IBidRepository
{
    Task<List<Bid>> GetAllBidsByAdvertisementId(Guid advertisementId);
    Task<Bid> AddBidAsync(Bid bid);
    Task RemoveBidAsync(Guid bidId);
    Task<Bid?> GetHighestBidForAdvertisementAsync(Guid advertisementId);
}
