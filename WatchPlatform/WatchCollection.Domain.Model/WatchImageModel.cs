using System;

namespace WatchCollection.Domain.Model;

public class WatchImageModel
{
    public Guid? ImageId { get; set; }

    public Guid WatchId { get; set; }

    public string BlobUrl { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public long? FileSize { get; set; }

    public string? ContentType { get; set; }

    public bool? IsPrimary { get; set; }

    public DateTimeOffset? UploadedAt { get; set; }
}
