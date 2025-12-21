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
            AdvertisementId = contract.AdvertisementId,
            BidderId = contract.BidderId,
            Amount = contract.Amount
        };
    }

    public static BidResponseContract AsContract(this BidsModel model)
    {
        return new BidResponseContract
        {
            BidId = model.BidId ?? throw new MappingException(),
            AdvertisementId = model.AdvertisementId,
            BidderId = model.BidderId,
            CreatedAt = model.CreatedAt,
            Amount = model.Amount
        };
    }

    public static Bid AsEntity(this BidsModel model)
    {
        return new Bid
        {
            BidId = model.BidId ?? throw new MappingException(),
            AdvertisementId = model.AdvertisementId,
            BidderId = model.BidderId,
            CreatedAt = model.CreatedAt,
            Amount = model.Amount
        };
    }

    public static BidsModel AsModel(this Bid entity)
    {
        return new BidsModel
        {
            BidId = entity.BidId,
            AdvertisementId = entity.AdvertisementId,
            BidderId = entity.BidderId,
            CreatedAt = entity.CreatedAt,
            Amount = entity.Amount
        };
    }
}
