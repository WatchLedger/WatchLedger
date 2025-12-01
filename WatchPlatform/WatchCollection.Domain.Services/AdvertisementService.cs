using System;
using Azure.Core.Pipeline;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;
using WatchCollection.Domain.Services.Mapping;

namespace WatchCollection.Domain.Services;

public class AdvertisementService(IAdvertisementRepository _repository) : IAdvertisementService
{
    public async Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract contract)
    {
        var model = contract.AsModel();
        var advertisementId = Guid.NewGuid();
        model.AdvertisementId = advertisementId;
        model.Status = "Active";
        model.PublishedAt = DateTimeOffset.UtcNow;
        var entity = model.AsEntity();
        var createdEntity =  await _repository.CreateAdvertisementAsync(entity);

        return createdEntity.AsModel().AsContract();
    }

    public Task DeleteAdvertisement(Guid advertisementId)
    {
        throw new NotImplementedException();
    }

    public async Task<AdvertisementResponseContract?> GetAdvertisementById(Guid advertisementId)
    {
        var entity =  await _repository.GetAdvertisementByIdAsync(advertisementId);
        if (entity is null)
            return null;
        return entity.AsModel().AsContract();
    }

    public Task<IEnumerable<AdvertisementResponseContract>> GetAdvertisementsByWatchId(Guid watchId)
    {
        throw new NotImplementedException();
    }

    public Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, AdvertisementRequestContract request)
    {
        throw new NotImplementedException();
    }
}
