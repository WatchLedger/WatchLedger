using System;

namespace WatchCollection.Domain.Model;

public class BidsModel
{
    public Guid? BidId { get; set; }
    public Guid AdvertisementId { get; set; }
    public Guid BidderId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal Amount { get; set; }
}
