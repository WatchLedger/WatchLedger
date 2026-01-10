using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IAdvertisementRepository
{
    Task<Advertisement> CreateAdvertisementAsync(Advertisement advertisement);
    Task<Advertisement> GetAdvertisementByIdAsync(Guid advertisementId);
    Task<Advertisement> UpdateAdvertisementAsync(Advertisement advertisement);
    Task DeleteAdvertisementAsync(Guid advertisementId);
    Task<IEnumerable<Advertisement>> GetAllAdvertisementsAsync(int pageNumber, int pageSize, string? watchBrand);
    Task<IEnumerable<Advertisement>> GetAdvertisementsBySellerIdAsync(Guid sellerId, int pageNumber, int pageSize, string? watchBrand, bool? includeInactive = false);
    Task UpdateViewCountAsync(Guid advertisementId, int newViewCount);
}
