using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IAdvertisementService
{
    Task<AdvertisementResponseContract> CreateAdvertisement(string bidderIdString, AdvertisementRequestContract contract);
    Task<AdvertisementResponseContract?> GetAdvertisementById(string? userId, Guid advertisementId);
    Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, string sellerIdString, bool isAdmin, AdvertisementUpdateRequestContract request);
    Task DeleteAdvertisement(string sellerIdString, bool isAdmin, Guid advertisementId);
    Task<IEnumerable<AdvertisementResponseContract>> GetAllAdvertisements();
    Task<decimal> GetWatchValuation(string referenceNumber);
}
