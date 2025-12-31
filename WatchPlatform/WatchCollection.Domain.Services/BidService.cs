using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Domain.Services;

public class BidService(IBidRepository _bidRepository, IAdvertisementRepository _adverisementRepository) : IBidService
{
    public async Task<BidResponseContract> AddBidAsync(Guid advertisementId, BidRequestContract bidRequestContract)
    {
        var advertisement = await _adverisementRepository.GetAdvertisementByIdAsync(advertisementId);
        if(advertisement is null)
            throw new AdvertisementNotFoundExceptions();

        var model = bidRequestContract.AsModel();
        model.BidId = Guid.NewGuid();
        model.AdvertisementId = advertisementId;
        model.BidderId = Guid.NewGuid(); // In a real scenario, this would come from the authenticated user context

        var entity = model.AsEntity();
        var createdEntity = await _bidRepository.AddBidAsync(entity);

        return createdEntity.AsModel().AsContract();
    }

    public async Task DeleteBidAsync(Guid bidId)
    {
        try
        {
            await _bidRepository.RemoveBidAsync(bidId);
        } catch (EntityNotFoundException)
        {
            throw;
        } 
    }

    public async Task<IEnumerable<BidResponseContract>> GetBidsByAdvertisementIdAsync(Guid advertisementId)
    {
        var advertisement = await _adverisementRepository.GetAdvertisementByIdAsync(advertisementId);
        if(advertisement is null)
            throw new AdvertisementNotFoundExceptions();

        var bids = await _bidRepository.GetAllBidsByAdvertisementId(advertisementId);
        return bids.Select(b => b.AsModel().AsContract());
    }
}
