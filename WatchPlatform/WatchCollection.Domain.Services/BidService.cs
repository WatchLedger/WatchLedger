using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;

namespace WatchCollection.Domain.Services;

public class BidService : IBidService
{
    public async Task<BidResponseContract> AddBidAsync(Guid advertisementId, BidRequestContract bidRequestContract)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteBidAsync(Guid bidId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<BidResponseContract>> GetBidsByAdvertisementIdAsync(Guid advertisementId)
    {
        throw new NotImplementedException();
    }
}
