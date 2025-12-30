using System;
using System.Collections.Generic;

namespace WatchCollection.Storage.Entities.Models;

public partial class Bid
{
    public Guid BidId { get; set; }

    public Guid AdvertisementId { get; set; }

    public Guid BidderId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public decimal Amount { get; set; }

    public virtual Advertisement Advertisement { get; set; } = null!;
}
