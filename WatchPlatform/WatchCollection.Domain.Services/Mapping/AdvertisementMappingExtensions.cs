using System;
using System.Linq.Expressions;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Shared.Enums;
using WatchCollection.Shared.Extensions;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Domain.Services.Mapping;

internal static class AdvertisementMappingExtensions
{
    // seperate method to wrap exception translation
    private static AdvertisementStatus SafeParseAdvertisementStatus(string status)
    {
        try
        {
            return status.ParseAdvertisementStatus();
        }
        catch (InvalidOperationException ex)
        {
            throw new MappingException($"Invalid advertisement status: {ex.Message}");
        }
    }

    public static AdvertisementModel AsModel(this AdvertisementRequestContract contract)
    {
        return new AdvertisementModel
        {
            WatchId = contract.WatchId is Guid watchId && watchId != Guid.Empty // ensure WatchId is not empty
                ? watchId
                : throw new MappingException("WatchId is required"),
            Title = contract.Title ?? throw new MappingException(),
            Status = contract.Status,
            Description = contract.Description,
            AskingPrice = contract.AskingPrice != default // ensure AskingPrice is greater than zero
                ? contract.AskingPrice
                : throw new MappingException("AskingPrice must be greater than zero."),
            AllowBids = contract.AllowBids
        };
    } 

    public static AdvertisementResponseContract AsContract(this AdvertisementModel model)
    {
        return new AdvertisementResponseContract
        {
            AdvertisementId = model.AdvertisementId ?? throw new MappingException(),
            WatchId = model.WatchId is Guid watchId && watchId != Guid.Empty // ensure WatchId is not empty
                ? watchId
                : throw new MappingException("WatchId is required"),
            SellerUserId = model.SellerUserId is Guid sellerUserId && sellerUserId != Guid.Empty // ensure SellerUserId is not empty
                ? sellerUserId
                : throw new MappingException("SellerUserId is required"),
            Title = model.Title ?? throw new MappingException(),
            Description = model.Description,
            AskingPrice = model.AskingPrice != default // ensure AskingPrice is greater than zero
                ? model.AskingPrice
                : throw new MappingException("AskingPrice must be greater than zero."),
            Status = model.Status,
            ViewCount = model.ViewCount,
            PublishedAt = model.PublishedAt,
            ExpiresAt = model.ExpiresAt,
            SoldAt = model.SoldAt,
            CreatedAt = model.CreatedAt ?? throw new MappingException("CreatedAt is required"),
            UpdatedAt = model.UpdatedAt ?? throw new MappingException("UpdatedAt is required"),
            AllowBids = model.AllowBids
        };
    }  

    public static Advertisement AsEntity(this AdvertisementModel model)
    {
        return new Advertisement
        {
            AdvertisementId = model.AdvertisementId ?? throw new MappingException(),
            WatchId = model.WatchId is Guid watchId && watchId != Guid.Empty // ensure WatchId is not empty
                ? watchId
                : throw new MappingException("WatchId is required"),
            SellerUserId = model.SellerUserId is Guid sellerUserId && sellerUserId != Guid.Empty // ensure SellerUserId is not empty
                ? sellerUserId
                : throw new MappingException("SellerUserId is required"),
            Title = model.Title ?? throw new MappingException("Title is required"),
            Description = model.Description,
            AskingPrice = model.AskingPrice != default // ensure AskingPrice is greater than zero
                ? model.AskingPrice
                : throw new MappingException("AskingPrice must be greater than zero."),
            Status = model.Status.ToString(),
            PublishedAt = model.PublishedAt,
            ExpiresAt = model.ExpiresAt,
            SoldAt = model.SoldAt,
            AllowBids = model.AllowBids
        };
    }

    public static AdvertisementModel AsModel(this Advertisement entity)
    {
        return new AdvertisementModel
        {
            AdvertisementId = entity.AdvertisementId,
            WatchId = entity.WatchId,
            SellerUserId = entity.SellerUserId,
            Title = entity.Title ?? throw new MappingException("Title is required"),
            Description = entity.Description,
            AskingPrice = entity.AskingPrice,
            Status = SafeParseAdvertisementStatus(entity.Status),
            ViewCount = entity.ViewCount,
            PublishedAt = entity.PublishedAt,
            ExpiresAt = entity.ExpiresAt,
            SoldAt = entity.SoldAt,
            CreatedAt = entity.CreatedAt != default // ensure CreatedAt is not default
                ? entity.CreatedAt
                : throw new MappingException("CreatedAt is required"),
            UpdatedAt = entity.UpdatedAt != default // ensure UpdatedAt is not default
                ? entity.UpdatedAt
                : throw new MappingException("UpdatedAt is required"),
            AllowBids = entity.AllowBids
        };
    }
}
