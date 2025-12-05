namespace WatchValuation.Storage.Records;

public record CachedValuation(string Id, string Type, decimal AveragePriceValuation, int? Ttl);
