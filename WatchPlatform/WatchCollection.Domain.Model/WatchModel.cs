using System;

namespace WatchCollection.Domain.Model;

public class WatchModel
{
    public Guid? WatchId { get; set; }
    public Guid? OwnerId { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearOfProduction { get; set; }
    public string Condition { get; set; }
    public string? Description { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public bool? IsForSale { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
