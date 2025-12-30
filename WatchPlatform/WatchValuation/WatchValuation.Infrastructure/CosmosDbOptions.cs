using System;

namespace WatchValuation.Infrastructure;

public class CosmosDbOptions
{
    public required string CosmosConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public required string ContainerName { get; set; }
}
