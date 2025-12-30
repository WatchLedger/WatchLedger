using System;

namespace WatchCollection.Api.Contracts;

public class BidResponseContract
{
    public required Guid BidId { get; set; }
    public required Guid AdvertisementId { get; set; }
    public required Guid BidderId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required decimal Amount { get; set; }
}
