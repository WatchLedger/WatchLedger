namespace WatchValuation.Storage.Records;

public record CachedValuation
{
    string id { get; init; } //watchreference
    string Type { get; init; } = "WatchValuation";
    public decimal AveragePriceValuation { get; init; }
}
