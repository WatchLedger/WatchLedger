using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace WatchCollection.Api.Contracts;

public class WatchRequestContract
{
    [Required]
    public required string Brand { get; set; }
    [Required]
    public required string Model { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearOfProduction { get; set; }
    [Required]
    public required string Condition { get; set; }
    public string? Description { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public required bool IsForSale { get; set; }
}
