using System;

namespace WatchValuation.Infrastructure;

public class ExternalApiOptions
{
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
}
