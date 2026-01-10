using System;
using System.ComponentModel.DataAnnotations;
using WatchCollection.Shared.Enums;

namespace WatchCollection.Api.Contracts;

public class AdvertisementUpdateRequestContract
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    [Required]
    public required AdvertisementStatus Status { get; set; }
    [Required]
    [Range(0.01, 10000000, ErrorMessage = "Asking price must be greater than zero.")]
    public required decimal AskingPrice { get; set; }
    [Required]
    public required bool AllowBids { get; set; }
}
