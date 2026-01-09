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
    public async Task<AdvertisementResponseContract> CreateAdvertisement(string bidderIdString, AdvertisementRequestContract contract)
    {
        if(contract.Status is AdvertisementStatus.Sold || contract.Status is AdvertisementStatus.Expired)
            throw new InvalidAdvertisementStatusException("Cannot create an advertisement with status Sold or Expired.");

        var model = contract.AsModel();
        var advertisementId = Guid.NewGuid();
        var sellerUserId = Guid.TryParse(bidderIdString, out var userIdGuid) ? userIdGuid : throw new Exception("Invalid User ID format.");
        model.AdvertisementId = advertisementId;
        model.SellerUserId = sellerUserId;
        model.ViewCount = 0;
        var entity = model.AsEntity();
        var createdEntity =  await _repository.CreateAdvertisementAsync(entity);

        return createdEntity.AsModel().AsContract();
    }

    public async Task DeleteAdvertisement(string sellerIdString, Guid advertisementId)
    {
        var advertisement = await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found");
        if (advertisement.SellerUserId.ToString() != sellerIdString){
            // TODO: write override logic for admin users
            // TODO: write custom exception for unauthorized access
            throw new UnauthorizedAccessException("User is not authorized to delete this advertisement.");
        }
        await _repository.DeleteAdvertisementAsync(advertisementId);
    }

    public async Task<AdvertisementResponseContract?> GetAdvertisementById(string sellerIdString,Guid advertisementId)
    {
        var entity =  await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (entity is null)
            return null;
        
        if (entity.SellerUserId.ToString() != sellerIdString){
            entity.ViewCount += 1;
            // TODO: Ensure a update view count exists only updating that field
            await _repository.UpdateAdvertisementAsync(entity);
        }

        return entity.AsModel().AsContract();
    }

    public async Task<IEnumerable<AdvertisementResponseContract>> GetAllAdvertisements()
    {
        var entities = await _repository.GetAllAdvertisementsAsync();
        return entities.Select(e => e.AsModel().AsContract());
    }

    public async Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, string sellerIdString, AdvertisementUpdateRequestContract contract)
    {
        if (contract.Status is AdvertisementStatus.Expired || contract.Status is AdvertisementStatus.Draft)
            throw new InvalidAdvertisementStatusException("Cannot update an advertisement to status Expired or Draft");

        var advertisement = await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found");

        if(advertisement.SellerUserId.ToString() != sellerIdString)
        {
            // TODO: write override logic for admin users
            throw new UnauthorizedAccessException("User is not authorized to update this advertisement.");
        }
        
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
