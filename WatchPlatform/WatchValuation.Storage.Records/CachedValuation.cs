namespace WatchValuation.Storage.Records;

public record CachedValuation(string id, string type, decimal averagePriceValuation, int? Ttl);
