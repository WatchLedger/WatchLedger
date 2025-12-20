using System;

namespace WatchCollection.Api.Contracts;

public class BidResponseContract
{
    public Guid? BidId { get; set; }
    public Guid AdvertisementId { get; set; }
    public Guid BidderId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal Amount { get; set; }
}
