using System;
using WatchCollection.Storage.Interfaces;
using Azure.Storage.Blobs;

namespace WatchCollection.Storage;

public class BlobStorageService : IBlobStorageService
{
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

    private BlobClient GetBlobClient(string filename)
    {
        var client = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=watchcollectionstorage;AccountKey=h5Rdf7clizxemrIAgGjR7/v1a4BQxc0EQhmi3USTnF77611hOuly58pHls82TNinm3H3xLjoGITv+AStShfmvA==;EndpointSuffix=core.windows.net");
        var containerClient = client.GetBlobContainerClient("watchimages");
        return containerClient.GetBlobClient(filename);
    }
}
