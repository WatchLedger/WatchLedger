using System;

namespace WatchCollection.Api.Contracts;

public class AdvertisementResponseContract
{
    public required Guid AdvertisementId { get; set; }

    public required Guid WatchId { get; set; }

    public required Guid SellerUserId { get; set; }

    public required string Title { get; set; } = null!;
    public string? Description { get; set; }

    public required decimal AskingPrice { get; set; }

    public string? Status { get; set; }

    public int? ViewCount { get; set; }
    public required bool AllowBids { get; set; }

    public DateTimeOffset? PublishedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset? SoldAt { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
