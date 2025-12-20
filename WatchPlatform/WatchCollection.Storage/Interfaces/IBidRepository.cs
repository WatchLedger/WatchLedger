using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IBidRepository
{
    Task<IEnumerable<Bid>> GetAllBidsByAdvertisementId(Guid advertisementId);
    Task<Bid> AddBidAsync(Bid bid);
    Task RemoveBidAsync(Bid bid);
}
