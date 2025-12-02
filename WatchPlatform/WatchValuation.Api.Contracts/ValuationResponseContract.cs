using System;

namespace WatchValuation.Api.Contracts;

public class ValuationResponseContract
{
    public required decimal AveragePriveLastSixMonths { get; set; }
}
