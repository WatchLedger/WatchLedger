using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IAdvertisementService
{
    Task<AdvertisementResponseContract> CreateAdvertisement(AdvertisementRequestContract contract);
    Task<AdvertisementResponseContract?> GetAdvertisementById(Guid advertisementId);
    Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, AdvertisementRequestContract request);
    Task DeleteAdvertisement(Guid advertisementId);
    Task<IEnumerable<AdvertisementResponseContract>> GetAdvertisementsByWatchId(Guid watchId);
}
