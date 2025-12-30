using System;
using Microsoft.EntityFrameworkCore.Storage;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Domain.Services.Mapping;

internal static class BidMappingExtensions
{
    public static BidsModel AsModel(this BidRequestContract contract)
    {
        return new BidsModel
        {
            Amount = contract.Amount  != default
                ? contract.Amount
                : throw new MappingException("Amount is required")
        };
    }

    public static BidResponseContract AsContract(this BidsModel model)
    {
        return new BidResponseContract
        {
            BidId = model.BidId ?? throw new MappingException("BidId is required"),
            AdvertisementId = model.AdvertisementId ?? throw new MappingException("AdvertisementId is required"),
            BidderId = model.BidderId ?? throw new MappingException("BidderId is required"),
            Amount = model.Amount != default
                ? model.Amount
                : throw new MappingException("Amount is required"),
            CreatedAt = model.CreatedAt ?? throw new MappingException("CreatedAt is required")
        };
    }

    public static Bid AsEntity(this BidsModel model)
    {
        return new Bid
        {
            BidId = model.BidId ?? throw new MappingException(),
            AdvertisementId = model.AdvertisementId ?? throw new MappingException(),
            BidderId = model.BidderId ?? throw new MappingException(),
            Amount = model.Amount != default
                ? model.Amount
                : throw new MappingException("Amount is required"),
        };
    }

    public static BidsModel AsModel(this Bid entity)
    {
        return new BidsModel
        {
            BidId = entity.BidId,
            AdvertisementId = entity.AdvertisementId,
            BidderId = entity.BidderId,
            CreatedAt = entity.CreatedAt != default
                ? entity.CreatedAt
                : throw new MappingException("CreatedAt is required"),
            Amount = entity.Amount != default
                ? entity.Amount
                : throw new MappingException("Amount is required")
        };
    }
}
