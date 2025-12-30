namespace WatchValuation.Storage.Records;

public record CachedValuation
{
    public string id { get; init; } = string.Empty; //watchreference
    public string Type { get; init; } = "WatchValuation";
    public decimal AveragePriceLastSixMonths{ get; init; }
}
