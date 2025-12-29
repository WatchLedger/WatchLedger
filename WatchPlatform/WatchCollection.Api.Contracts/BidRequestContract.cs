using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class BidRequestContract
{
    [Required]
    public required Guid AdvertisementId { get; set; }

    [Required]
    public required Guid BidderId { get; set; }

    [Required]
    public required decimal Amount { get; set; }
}
