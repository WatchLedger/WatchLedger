using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class AdvertisementRequestContract
{
    [Required]
    public Guid WatchId { get; set; }
    [Required]
    public Guid SellerUserId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;
    [MaxLength(1000)]
    public string? Description { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Asking price must be greater than zero.")]
    public decimal AskingPrice { get; set; }
    [Required]
    public bool AllowBids { get; set; }
}
