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

public class AdvertisementService(IAdvertisementRepository _repository, IWatchValuationHttpClient _watchValuationClient, IWatchService _watchService) : IAdvertisementService
{
    public async Task<AdvertisementResponseContract> CreateAdvertisement(string sellerIdString, AdvertisementRequestContract contract)
    {
        var watch = await _watchService.GetWatchById( sellerIdString,contract.WatchId);
        if (watch is null)
            throw new WatchNotFoundException(contract.WatchId, "Watch not found for the provided WatchId.");
        if(watch.OwnerUserId.ToString() != sellerIdString)
            throw new UnauthorizedAccessException("User is not authorized to create an advertisement for this watch.");
        if(contract.Status is AdvertisementStatus.Sold || contract.Status is AdvertisementStatus.Expired)
            throw new InvalidAdvertisementStatusException("Cannot create an advertisement with status Sold or Expired.");

        var model = contract.AsModel();
        var advertisementId = Guid.NewGuid();
        var sellerUserId = Guid.TryParse(sellerIdString, out var userIdGuid) ? userIdGuid : throw new Exception("Invalid User ID format.");
        model.AdvertisementId = advertisementId;
        model.SellerUserId = sellerUserId;
        model.ViewCount = 0;

        if(model.Status is AdvertisementStatus.Active)
        {
            model.PublishedAt = DateTime.UtcNow;
            model.ExpiresAt = model.PublishedAt.Value.AddDays(30); 
        }

        var entity = model.AsEntity();
        var createdEntity =  await _repository.CreateAdvertisementAsync(entity);

        return createdEntity.AsModel().AsContract();
    }

    public async Task DeleteAdvertisement(string sellerIdString, bool isAdmin, Guid advertisementId)
    {
        var advertisement = await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found");
        if (advertisement.SellerUserId.ToString() != sellerIdString && !isAdmin){
            throw new UnauthorizedAccessException("User is not authorized to delete this advertisement.");
        }
        await _repository.DeleteAdvertisementAsync(advertisementId);
    }

    public async Task<AdvertisementResponseContract?> GetAdvertisementById(string? userId, Guid advertisementId)
    {
        var entity =  await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (entity is null)
            return null;
        
        if (userId is not null && entity.SellerUserId.ToString() != userId){
            entity.ViewCount += 1;
            await _repository.UpdateViewCountAsync(entity.AdvertisementId, entity.ViewCount);
        }

        return entity.AsModel().AsContract();
    }

    public async Task<IEnumerable<AdvertisementResponseContract>> GetAllAdvertisements(int pageNumber = 1, int pageSize = 10, string? watchBrand = null)
    {
        // ensure pagination parameters are valid
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var entities = await _repository.GetAllAdvertisementsAsync(pageNumber, pageSize, watchBrand);
        return entities.Select(e => e.AsModel().AsContract());
    }

    public async Task<IEnumerable<AdvertisementResponseContract>> GetAdvertisementsBySellerId(Guid sellerId, int pageNumber = 1, int pageSize = 10, string? watchBrand = null)
    {
        // ensure pagination parameters are valid
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var entities = await _repository.GetAdvertisementsBySellerIdAsync(sellerId, pageNumber, pageSize, watchBrand);
        return entities.Select(e => e.AsModel().AsContract());
    }

    public async Task<IEnumerable<AdvertisementResponseContract>> GetAdvertisementsByOwnerId(string ownerIdString, int pageNumber = 1, int pageSize = 10, string? watchBrand = null)
    {
        // ensure pagination parameters are valid
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var entities = await _repository.GetAdvertisementsBySellerIdAsync(Guid.Parse(ownerIdString), pageNumber, pageSize, watchBrand, true);
        return entities.Select(e => e.AsModel().AsContract());
    }

    public async Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, string sellerIdString, bool isAdmin, AdvertisementUpdateRequestContract contract)
    {
        if (contract.Status is AdvertisementStatus.Expired || contract.Status is AdvertisementStatus.Draft)
            throw new InvalidAdvertisementStatusException("Cannot update an advertisement to status Expired or Draft");

        var advertisement = await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (advertisement is null)
            throw new AdvertisementNotFoundExceptions(advertisementId, "Advertisement not found");

        if(advertisement.SellerUserId.ToString() != sellerIdString && !isAdmin)
        {
            throw new UnauthorizedAccessException("User is not authorized to update this advertisement.");
        }

        if (advertisement.AsModel().Status is AdvertisementStatus.Sold)
            throw new InvalidAdvertisementStatusException("Cannot update an advertisement that is marked as Sold.");
            
        
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
        model.AdvertisementId = advertisement.AdvertisementId;
        model.SellerUserId    = advertisement.SellerUserId;
        if(contract.Status is AdvertisementStatus.Active && advertisement.AsModel().Status is not AdvertisementStatus.Active)
        {
            model.PublishedAt = DateTime.UtcNow;
            model.ExpiresAt = model.PublishedAt.Value.AddDays(30);
        }
        else
        {
            model.PublishedAt = advertisement.PublishedAt;
            model.ExpiresAt   = advertisement.ExpiresAt;
        }

        if(contract.Status is AdvertisementStatus.Sold)
        {
            model.SoldAt = DateTime.UtcNow;
        }
        else
        {
            model.SoldAt = advertisement.SoldAt;
        }

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
