using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Domain.Services.Mapping;

public static class AdvertisementMappingExtensions
{
    public static AdvertisementModel AsModel(this AdvertisementRequestContract contract)
    {
        return new AdvertisementModel
        {
            WatchId = contract.WatchId,
            SellerUserId = contract.SellerUserId,
            Title = contract.Title,
            Description = contract.Description,
            AskingPrice = contract.AskingPrice,
        };
    } 

    public static AdvertisementResponseContract AsContract(this AdvertisementModel model)
    {
        return new AdvertisementResponseContract
        {
            AdvertisementId = model.AdvertisementId ?? throw new MappingException(),
            WatchId = model.WatchId,
            SellerUserId = model.SellerUserId,
            Title = model.Title,
            Description = model.Description,
            AskingPrice = model.AskingPrice,
            Status = model.Status,
            ViewCount = model.ViewCount,
            PublishedAt = model.PublishedAt,
            ExpiresAt = model.ExpiresAt,
            SoldAt = model.SoldAt,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }  

    public static Advertisement AsEntity(this AdvertisementModel model)
    {
        return new Advertisement
        {
            AdvertisementId = model.AdvertisementId ?? throw new MappingException(),
            WatchId = model.WatchId,
            SellerUserId = model.SellerUserId,
            Title = model.Title,
            Description = model.Description,
            AskingPrice = model.AskingPrice,
            Status = model.Status,
            ViewCount = model.ViewCount,
            PublishedAt = model.PublishedAt,
            ExpiresAt = model.ExpiresAt,
            SoldAt = model.SoldAt,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }

    public static AdvertisementModel AsModel(this Advertisement entity)
    {
        return new AdvertisementModel
        {
            AdvertisementId = entity.AdvertisementId,
            WatchId = entity.WatchId,
            SellerUserId = entity.SellerUserId,
            Title = entity.Title,
            Description = entity.Description,
            AskingPrice = entity.AskingPrice,
            Status = entity.Status,
            ViewCount = entity.ViewCount,
            PublishedAt = entity.PublishedAt?.UtcDateTime,
            ExpiresAt = entity.ExpiresAt?.UtcDateTime,
            SoldAt = entity.SoldAt?.UtcDateTime,
            CreatedAt = entity.CreatedAt?.UtcDateTime ?? throw new MappingException(),
            UpdatedAt = entity.UpdatedAt?.UtcDateTime,
        };
    }
}
