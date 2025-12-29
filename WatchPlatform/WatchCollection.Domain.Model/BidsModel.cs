using System;

namespace WatchCollection.Domain.Model;

public class BidsModel
{
    public Guid? BidId { get; set; } // nullable since value assignment happens in service after mapping to model
    public required Guid AdvertisementId { get; set; }
    public required Guid BidderId { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } // same reasoning as BidId
    public required decimal Amount { get; set; }
}
