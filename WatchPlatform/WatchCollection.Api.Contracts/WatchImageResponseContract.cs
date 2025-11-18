using System;

namespace WatchCollection.Api.Contracts;

public class WatchImageResponseContract
{
    public required Guid ImageId { get; set; }
    public required Guid WatchId { get; set; }
    public required string BlobUrl { get; set; }
    public required string FileName { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public bool? IsPrimary { get; set; }
    public DateTimeOffset? UploadedAt { get; set; }
}
