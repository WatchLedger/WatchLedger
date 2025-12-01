using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Domain.Services;

public class AdvertisementService(IAdvertisementRepository _repository) : IAdvertisementService
{
    public Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract contract)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAdvertisement(Guid advertisementId)
    {
        throw new NotImplementedException();
    }

    public Task<AdvertisementResponseContract?> GetAdvertisementById(Guid advertisementId)
    {
        throw new NotImplementedException();
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
