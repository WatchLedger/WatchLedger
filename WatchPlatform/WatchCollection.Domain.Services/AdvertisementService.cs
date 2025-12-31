using System;
using Azure.Core.Pipeline;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Shared.Enums;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Domain.Services;

public class AdvertisementService(IAdvertisementRepository _repository, IWatchValuationHttpClient _watchValuationClient) : IAdvertisementService
{
    public async Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract contract)
    {
        if(contract.Status is AdvertisementStatus.Sold || contract.Status is AdvertisementStatus.Expired)
            throw new InvalidAdvertisementStatusException("Cannot create an advertisement with status Sold or Expired.");

        var model = contract.AsModel();
        var advertisementId = Guid.NewGuid();
        var sellerUserId = Guid.NewGuid(); // This should be retrieved from the authenticated user's context in a real application.
        model.AdvertisementId = advertisementId;
        model.SellerUserId = sellerUserId;
        model.ViewCount = 0;
        var entity = model.AsEntity();
        var createdEntity =  await _repository.CreateAdvertisementAsync(entity);

        return createdEntity.AsModel().AsContract();
    }

    public async Task DeleteAdvertisement(Guid advertisementId)
    {
        await _repository.DeleteAdvertisementAsync(advertisementId);
    }

    public async Task<AdvertisementResponseContract?> GetAdvertisementById(Guid advertisementId)
    {
        var entity =  await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (entity is null)
            return null;
        return entity.AsModel().AsContract();
    }

    public async Task<IEnumerable<AdvertisementResponseContract>> GetAllAdvertisements()
    {
        var entities = await _repository.GetAllAdvertisementsAsync();
        return entities.Select(e => e.AsModel().AsContract());
    }

    public async Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, AdvertisementUpdateRequestContract contract)
    {
        if (contract.Status is AdvertisementStatus.Expired || contract.Status is AdvertisementStatus.Draft)
            throw new InvalidAdvertisementStatusException("Cannot update an advertisement to status Expired or Draft");
        var advertisement = await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found");
        
        var contractWithWatchId = new AdvertisementRequestContract
        {
            WatchId = advertisement.WatchId,
            Title = contract.Title,
            Description = contract.Description,
            AskingPrice = contract.AskingPrice,
            Status = contract.Status,
            AllowBids = contract.AllowBids
        };
        var model = contractWithWatchId.AsModel();
        model.AdvertisementId = advertisementId;
        model.SellerUserId = advertisement.SellerUserId; // This should be retrieved from the authenticated user's context in a real application.

        var entity = model.AsEntity();
        
        var updatedEntity =  await _repository.UpdateAdvertisementAsync(entity);
        return updatedEntity.AsModel().AsContract();
    }

    public async Task<decimal> GetWatchValuation(string referenceNumber)
    {
        var valuation = await _watchValuationClient.GetWatchValuationAsync(referenceNumber);
        if(valuation == default)
            throw new ValuationUnavailableException("No valuation available for the provided reference number.");
        return valuation;
    }
}
