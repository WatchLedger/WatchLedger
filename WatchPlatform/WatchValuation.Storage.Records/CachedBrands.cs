namespace WatchValuation.Storage.Records;

public record CachedBrands(string id, string type, List<string> Brands, int? Ttl);
