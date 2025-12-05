namespace WatchValuation.Storage.Records;

public record CachedBrands(string Id, string Type, List<string> Brands, int? Ttl);
