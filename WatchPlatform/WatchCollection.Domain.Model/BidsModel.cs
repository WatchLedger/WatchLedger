using System;

namespace WatchCollection.Domain.Model;

public class BidsModel
{
    public Guid? BidId { get; set; } // nullable since value assignment happens in service after mapping to model
    public Guid? AdvertisementId { get; set; }
    public Guid? BidderId { get; set; }
    public required decimal Amount { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } // same reasoning as BidId
}
