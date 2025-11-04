using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace WatchCollection.Api.Contracts;

public class WatchRequestContract
{
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Model { get; set; }
    public string? RefereceNumber { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearOfProduction { get; set; }
    [Required]
    public string Condition { get; set; }
    public string? Description { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public bool? IsForSale { get; set; }
}
