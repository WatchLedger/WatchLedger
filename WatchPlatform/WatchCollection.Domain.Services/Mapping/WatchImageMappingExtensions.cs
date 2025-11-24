using System;
using WatchCollection.Domain.Model;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Exceptions;
namespace WatchCollection.Domain.Services.Mapping;

internal static class WatchImageMappingExtensions
{
    public static WatchImage AsEntity(this WatchImageModel model)
    {
        return new WatchImage
        {
            WatchId = model.WatchId,
            BlobUrl = model.BlobUrl,
            FileName = model.FileName,
            FileSize = model.FileSize,
            ContentType = model.ContentType,
            IsPrimary = model.IsPrimary,
            UploadedAt = model.UploadedAt
        };
    }

    public static WatchImageModel AsModel(this WatchImage entity)
    {
        return new WatchImageModel
        {
            ImageId = entity.ImageId,
            WatchId = entity.WatchId,
            BlobUrl = entity.BlobUrl,
            FileName = entity.FileName,
            FileSize = entity.FileSize,
            ContentType = entity.ContentType,
            IsPrimary = entity.IsPrimary,
            UploadedAt = entity.UploadedAt
        };
    }

    public static WatchImageResponseContract AsResponseContract(this WatchImageModel model)
    {
        return new WatchImageResponseContract
        {
            ImageId = model.ImageId ?? throw new MappingException() ,
            WatchId = model.WatchId,
            BlobUrl = model.BlobUrl,
            FileName = model.FileName,
            FileSize = model.FileSize,
            ContentType = model.ContentType,
            IsPrimary = model.IsPrimary,
            UploadedAt = model.UploadedAt
        };
    }
}
