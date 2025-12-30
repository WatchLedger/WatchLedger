using System;

namespace WatchCollection.Domain.Model;

public class WatchImageModel
{
    public Guid ImageId { get; set; } //non-nullable only for watchimage since there's no mapping from contract to model without it
    public Guid WatchId { get; set; }

    public string BlobUrl { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public long? FileSize { get; set; }

    public string? ContentType { get; set; }

    public bool IsPrimary { get; set; }

    public DateTimeOffset UploadedAt { get; set; } // same exception here as ImageId
}
