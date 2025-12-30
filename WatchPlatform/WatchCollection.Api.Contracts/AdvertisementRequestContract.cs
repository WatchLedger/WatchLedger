using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class AdvertisementRequestContract
{
    [Required]
    public required Guid WatchId { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    [Required]
    public required string Status { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Asking price must be greater than zero.")]
    public required decimal AskingPrice { get; set; }
    [Required]
    public required bool AllowBids { get; set; }
}
