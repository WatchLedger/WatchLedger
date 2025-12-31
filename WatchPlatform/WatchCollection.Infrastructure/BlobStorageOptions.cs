namespace WatchCollection.Infrastructure;

public class BlobStorageOptions
{
    public required string BlobStorageConnectionString { get; set; }
    public required string ContainerName { get; set; }
}
