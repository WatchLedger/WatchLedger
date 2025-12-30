using System;
using Azure.Core.Pipeline;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Domain.Services.Exceptions;

namespace WatchCollection.Domain.Services;

public class AdvertisementService(IAdvertisementRepository _repository, IWatchValuationHttpClient _watchValuationClient) : IAdvertisementService
{
    public async Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract contract)
    {
        var model = contract.AsModel();
        var advertisementId = Guid.NewGuid();
        model.AdvertisementId = advertisementId;
        model.Status = "Active";
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

    public async Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, AdvertisementRequestContract request)
    {
        var model = request.AsModel();
        model.AdvertisementId = advertisementId;

        var entity = model.AsEntity();
        
        var updatedEntity =  await _repository.UpdateAdvertisementAsync(entity);
        return updatedEntity.AsModel().AsContract();
    }

    public async Task<decimal> GetWatchValuation(string referenceNumber)
    {
        try
        {
            var valuation = await _watchValuationClient.GetWatchValuationAsync(referenceNumber);
            if(valuation == default)
                throw new ValuationUnavailableException("No valuation available for the provided reference number.");
            return valuation;
        }
        catch(ArgumentException)
        {
            throw;
        }
        catch (Exception)
        {   
            throw;
        }
    }
}
