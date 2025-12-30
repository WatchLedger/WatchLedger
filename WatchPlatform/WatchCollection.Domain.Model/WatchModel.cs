using System;

namespace WatchCollection.Domain.Model;

public class WatchModel
{
    public Guid? WatchId { get; set; }
    public Guid? OwnerId { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearOfProduction { get; set; }
    public required string Condition { get; set; }
    public string? Description { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } // same reasoning as WatchId
    public DateTimeOffset? UpdatedAt { get; set; } // same reasoning as WatchId
}
