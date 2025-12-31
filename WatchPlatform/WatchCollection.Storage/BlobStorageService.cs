using WatchCollection.Storage.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using WatchCollection.Infrastructure;

namespace WatchCollection.Storage;

public class BlobStorageService(IOptions<BlobStorageOptions> _options) : IBlobStorageService
{
    private BlobClient GetBlobClient(string filename)
    {
        var options = _options.Value;
        var client = new BlobServiceClient(options.BlobStorageConnectionString);
        var containerClient = client.GetBlobContainerClient(options.ContainerName);
        return containerClient.GetBlobClient(filename);
    }
    public async Task<string> UploadImageAsync(string filename, Stream image)
    {
        var blob = GetBlobClient(filename);
        await blob.UploadAsync(image, true);
        return blob.Uri.ToString();
    }

    public async Task DeleteImageAsync(string filename)
    {
        var blob = GetBlobClient(filename);
        await blob.DeleteIfExistsAsync();
    }
}
