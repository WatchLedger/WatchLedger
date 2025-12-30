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
            ImageId = model.ImageId is Guid imageId && imageId != Guid.Empty
                ? imageId
                : throw new MappingException("ImageId is required"),
            WatchId = model.WatchId is Guid watchId && watchId != Guid.Empty
                ? watchId
                : throw new MappingException("WatchId is required"),
            BlobUrl = model.BlobUrl ?? throw new MappingException("BlobUrl is required"),
            FileName = model.FileName ?? throw new MappingException("FileName is required"),
            FileSize = model.FileSize,
            ContentType = model.ContentType,
            IsPrimary = model.IsPrimary,
        };
    }

    public static WatchImageModel AsModel(this WatchImage entity)
    {
        return new WatchImageModel
        {
            ImageId = entity.ImageId,
            WatchId = entity.WatchId,
            BlobUrl = entity.BlobUrl ?? throw new MappingException("BlobUrl is required"),
            FileName = entity.FileName ?? throw new MappingException("FileName is required"),
            FileSize = entity.FileSize,
            ContentType = entity.ContentType,
            IsPrimary = entity.IsPrimary,
            UploadedAt = entity.UploadedAt != default
                ? entity.UploadedAt
                : throw new MappingException("UploadedAt is required")
        };
    }

    public static WatchImageResponseContract AsContract(this WatchImageModel model)
    {
        return new WatchImageResponseContract
        {
            ImageId = model.ImageId is Guid imageId && imageId != Guid.Empty
                ? imageId
                : throw new MappingException("ImageId is required"),
            WatchId = model.WatchId is Guid watchId && watchId != Guid.Empty
                ? watchId
                : throw new MappingException("WatchId is required"),
            BlobUrl = model.BlobUrl ?? throw new MappingException("BlobUrl is required"),
            FileName = model.FileName ?? throw new MappingException("FileName is required"),
            FileSize = model.FileSize,
            ContentType = model.ContentType,
            IsPrimary = model.IsPrimary,
            UploadedAt = model.UploadedAt != default // ensure UploadedAt is not default
                ? model.UploadedAt
                : throw new MappingException("UploadedAt is required")
        };
    }

}
