using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IAdvertisementService
{
    Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract request);
    Task<AdvertisementResponseContract?> GetAdvertisementById(Guid advertisementId);
}
