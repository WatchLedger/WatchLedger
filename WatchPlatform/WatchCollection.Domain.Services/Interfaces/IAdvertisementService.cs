using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IAdvertisementService
{
    Task<AdvertisementResponseContract> CreateAdvertisement(string bidderIdString, AdvertisementRequestContract contract);
    Task<AdvertisementResponseContract?> GetAdvertisementById(string? userId, Guid advertisementId);
    Task<AdvertisementResponseContract> UpdateAdvertisement(Guid advertisementId, string sellerIdString, bool isAdmin, AdvertisementUpdateRequestContract request);
    Task DeleteAdvertisement(string sellerIdString, bool isAdmin, Guid advertisementId);
    Task<IEnumerable<AdvertisementResponseContract>> GetAllAdvertisements(int pageNumber, int pageSize, string? watchBrand);
    Task<IEnumerable<AdvertisementResponseContract>> GetAdvertisementsBySellerId(Guid sellerId, int pageNumber, int pageSize, string? watchBrand);
    Task<IEnumerable<AdvertisementResponseContract>> GetAdvertisementsByOwnerId(string ownerIdString, int pageNumber, int pageSize, string? watchBrand);
    Task<decimal> GetWatchValuation(string referenceNumber);
}
