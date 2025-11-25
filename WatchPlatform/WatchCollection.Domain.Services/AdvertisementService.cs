using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Domain.Services;

public class AdvertisementService(IAdvertisementRepository _repository) : IAdvertisementService
{
    public Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract request)
    {
        throw new NotImplementedException();
    }

    public Task<AdvertisementResponseContract?> GetAdvertisementById(Guid advertisementId)
    {
        throw new NotImplementedException();
    }
}
