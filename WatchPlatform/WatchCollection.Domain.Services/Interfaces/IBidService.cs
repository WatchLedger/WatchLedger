using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IBidService
{
    Task<BidResponseContract> AddBidAsync(Guid advertisementId, BidRequestContract bidRequestContract);
    Task<IEnumerable<BidResponseContract>> GetBidsByAdvertisementIdAsync(Guid advertisementId);
    Task DeleteBidAsync(Guid bidId);
}
