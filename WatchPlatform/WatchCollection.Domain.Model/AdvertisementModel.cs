using System;

namespace WatchCollection.Domain.Model;

public class AdvertisementModel
{
    public Guid? AdvertisementId { get; set; }
    public Guid WatchId { get; set; }
    public Guid SellerUserId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal AskingPrice { get; set; }
    public string? Status { get; set; } = null!;
    public int? ViewCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? SoldAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
