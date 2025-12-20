using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class BidRequestContract
{
    [Required]
    public Guid AdvertisementId { get; set; }

    [Required]
    public Guid BidderId { get; set; }

    [Required]
    public decimal Amount { get; set; }
}
