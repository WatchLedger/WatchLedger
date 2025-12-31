using System;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Model;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Shared.Enums;
using WatchCollection.Shared.Extensions;

namespace WatchCollection.Domain.Services.Mapping;

internal static class WatchMappingExtensions
{
    // seperate method to wrap exception translation
    private static WatchCondition SafeParseWatchCondition(string condition)
    {
        try
        {
            return condition.ParseWatchCondition();
        }
        catch (InvalidOperationException ex)
        {
            throw new MappingException($"Invalid watch condition: {ex.Message}");
        }
    }

    public static WatchModel AsModel(this WatchRequestContract contract)
    {
        return new WatchModel
        {
            OwnerId = Guid.Empty, // temp
            Brand = contract.Brand ?? throw new MappingException("Brand is required."),
            Model = contract.Model ?? throw new MappingException("Model is required."),
            ReferenceNumber = contract.ReferenceNumber,
            SerialNumber = contract.SerialNumber,
            YearOfProduction = contract.YearOfProduction,
            Condition = contract.Condition,
            Description = contract.Description,
            PurchasePrice = contract.PurchasePrice,
            PurchaseDate = contract.PurchaseDate,
        };
    }

    public static Watch AsEntity(this WatchModel model)
    {
        return new Watch
        {
            WatchId = model.WatchId ?? throw new MappingException("WatchId is required."),
            OwnerUserId = model.OwnerId is Guid ownerId && ownerId != Guid.Empty
                ? ownerId
                : throw new MappingException("OwnerId is required."),
            Brand = model.Brand ?? throw new MappingException("Brand is required."),
            Model = model.Model ?? throw new MappingException("Model is required."),
            ReferenceNumber = model.ReferenceNumber,
            SerialNumber = model.SerialNumber,
            YearOfProduction = model.YearOfProduction,
            Description = model.Description,
            Condition = model.Condition.ToString(),
            PurchasePrice = model.PurchasePrice,
            PurchaseDate = model.PurchaseDate,
        };
    }

    public static WatchResponseContract AsContract(this WatchModel model)
    {
        return new WatchResponseContract
        {
            WatchId = model.WatchId ?? throw new MappingException("WatchId is required."),
            OwnerUserId = model.OwnerId is Guid ownerId && ownerId != Guid.Empty
                ? ownerId
                : throw new MappingException("OwnerId is required."),
            Brand = model.Brand ?? throw new MappingException("Brand is required."),
            Model = model.Model ?? throw new MappingException("Model is required."),
            ReferenceNumber = model.ReferenceNumber,
            SerialNumber = model.SerialNumber,
            YearOfProduction = model.YearOfProduction,
            Condition = model.Condition,
            Description = model.Description,
            PurchasePrice = model.PurchasePrice,
            PurchaseDate = model.PurchaseDate,
            CreatedAt = model.CreatedAt ?? throw new MappingException("CreatedAt is required."),
            UpdatedAt = model.UpdatedAt ?? throw new MappingException("UpdatedAt is required.")
        };
    }

    public static WatchModel AsModel(this Watch entity)
    {
        return new WatchModel
        {
            WatchId = entity.WatchId,
            OwnerId = entity.OwnerUserId,
            Brand = entity.Brand ?? throw new MappingException("Brand is required."),
            Model = entity.Model ?? throw new MappingException("Model is required."),
            ReferenceNumber = entity.ReferenceNumber,
            SerialNumber = entity.SerialNumber,
            YearOfProduction = entity.YearOfProduction,
            Condition = SafeParseWatchCondition(entity.Condition),
            Description = entity.Description,
            PurchasePrice = entity.PurchasePrice,
            PurchaseDate = entity.PurchaseDate,
            CreatedAt = entity.CreatedAt != default
                ? entity.CreatedAt
                : throw new MappingException("CreatedAt is required."), 
            UpdatedAt = entity.UpdatedAt != default
                ? entity.UpdatedAt
                : throw new MappingException("UpdatedAt is required.")
        };
    }
}
