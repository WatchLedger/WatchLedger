namespace WatchValuation.Storage.Records;

public record CachedBrands
{
    public string id { get; init; } = "BrandsCache";
    public string Type { get; init; } = "WatchBrands";
    public List<string> Brands { get; init; } = [];
    public int? Ttl { get; init; }
}
