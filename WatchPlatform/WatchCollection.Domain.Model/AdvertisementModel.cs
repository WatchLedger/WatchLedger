using System;

namespace WatchCollection.Domain.Model;

public class AdvertisementModel
{
    public Guid? AdvertisementId { get; set; } // Nullable since value assignment happens in service after mapping to model
    public required Guid WatchId { get; set; }
    public Guid? SellerUserId { get; set; } // Nullable since value assignment happens in service after mapping to model
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required decimal AskingPrice { get; set; }
    public required string Status { get; set; }
    public int? ViewCount { get; set; } // same reasoning as AdvertisementId
    public required bool AllowBids { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? SoldAt { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } // same reasoning as AdvertisementId
    public DateTimeOffset? UpdatedAt { get; set; } // same reasoning as AdvertisementId
}
