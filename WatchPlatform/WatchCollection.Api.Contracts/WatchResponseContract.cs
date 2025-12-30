using System;

namespace WatchCollection.Api.Contracts;

public class WatchResponseContract
{
    public required Guid WatchId { get; set; }
    public required Guid OwnerUserId { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearOfProduction { get; set; }
    public required string Condition { get; set; }
    public string? Description { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
    public IEnumerable<WatchImageResponseContract>? Images { get; set; }
}
