using System;
using System.Collections.Generic;

namespace WatchCollection.Storage.Entities.Models;

public partial class Advertisement
{
    public Guid AdvertisementId { get; set; }

    public Guid WatchId { get; set; }

    public Guid SellerUserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal AskingPrice { get; set; }

    public string? Status { get; set; }

    public int? ViewCount { get; set; }

    public DateTimeOffset? PublishedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset? SoldAt { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual Watch Watch { get; set; } = null!;
}
