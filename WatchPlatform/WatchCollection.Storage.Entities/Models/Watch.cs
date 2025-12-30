using System;
using System.Collections.Generic;

namespace WatchCollection.Storage.Entities.Models;

public partial class Watch
{
    public Guid WatchId { get; set; }

    public Guid OwnerUserId { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? ReferenceNumber { get; set; }

    public string? SerialNumber { get; set; }

    public int? YearOfProduction { get; set; }

    public string Condition { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PurchasePrice { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual ICollection<WatchImage> WatchImages { get; set; } = new List<WatchImage>();
}
