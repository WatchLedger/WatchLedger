using System;
using System.Collections.Generic;

namespace WatchCollection.Storage.Entities.Models;

public partial class WatchImage
{
    public Guid ImageId { get; set; }

    public Guid WatchId { get; set; }

    public string BlobUrl { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public long? FileSize { get; set; }

    public string? ContentType { get; set; }

    public bool IsPrimary { get; set; }

    public DateTimeOffset UploadedAt { get; set; }

    public virtual Watch Watch { get; set; } = null!;
}
