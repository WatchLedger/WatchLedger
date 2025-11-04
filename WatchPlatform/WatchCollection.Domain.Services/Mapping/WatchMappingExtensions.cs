using System;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Exceptions;

namespace WatchCollection.Domain.Services.Mapping;

internal static class WatchMappingExtensions
{
    public static WatchModel AsModel(this WatchRequestContract contract)
    {
        return new WatchModel
        {
            Brand = contract.Brand ?? throw new MappingException(),
            Model = contract.Model ?? throw new MappingException(),
            ReferenceNumber = contract.RefereceNumber,
            SerialNumber = contract.SerialNumber,
            YearOfProduction = contract.YearOfProduction,
            Condition = contract.Condition ?? throw new MappingException(),
            Description = contract.Description,
            PurchasePrice = contract.PurchasePrice,
            PurchaseDate = contract.PurchaseDate,
            IsForSale = contract.IsForSale,
        };
    }

    public static Watch AsEntity(this WatchModel model)
    {
        return new Watch
        {
            WatchId = model.WatchId,
            OwnerUserId = model.OwnerId,
            Brand = model.Brand ?? throw new MappingException(),
            Model = model.Model ?? throw new MappingException(),
            ReferenceNumber = model.ReferenceNumber,
            SerialNumber = model.SerialNumber,
            YearOfProduction = model.YearOfProduction,
            Condition = model.Condition ?? throw new MappingException(),
            Description = model.Description,
            PurchasePrice = model.PurchasePrice,
            PurchaseDate = model.PurchaseDate,
            IsForSale = model.IsForSale,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    public static WatchResponseContract AsContract(this WatchModel model)
    {
        return new WatchResponseContract
        {
            WatchId = model.WatchId ?? throw new MappingException(),
            OwnerId = model.OwnerId ?? throw new MappingException(),
            Brand = model.Brand ?? throw new MappingException(),
            Model = model.Model ?? throw new MappingException(),
            ReferenceNumber = model.ReferenceNumber,
            SerialNumber = model.SerialNumber,
            YearOfProduction = model.YearOfProduction,
            Condition = model.Condition ?? throw new MappingException(),
            Description = model.Description,
            PurchasePrice = model.PurchasePrice,
            PurchaseDate = model.PurchaseDate,
            IsForSale = model.IsForSale,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    public static WatchModel AsModel(this Watch entity)
    {
        return new WatchModel
        {
            WatchId = entity.WatchId,
            OwnerId = entity.OwnerUserId,
            Brand = entity.Brand ?? throw new MappingException(),
            Model = entity.Model ?? throw new MappingException(),
            ReferenceNumber = entity.ReferenceNumber,
            SerialNumber = entity.SerialNumber,
            YearOfProduction = entity.YearOfProduction,
            Condition = entity.Condition ?? throw new MappingException(),
            Description = entity.Description,
            PurchasePrice = entity.PurchasePrice,
            PurchaseDate = entity.PurchaseDate,
            IsForSale = entity.IsForSale,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
