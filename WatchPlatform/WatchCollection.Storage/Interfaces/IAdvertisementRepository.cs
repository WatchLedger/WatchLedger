using System;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Interfaces;

public interface IAdvertisementRepository
{
    Task<Guid> CreateAdvertisementAsync(Advertisement advertisement);
    Task<Advertisement> GetAdvertisementByIdAsync(Guid advertisementId);
}
