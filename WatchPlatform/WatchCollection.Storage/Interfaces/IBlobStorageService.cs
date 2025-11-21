using System;

namespace WatchCollection.Storage.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(string filename, Stream image);
    Task DeleteImageAsync(string blobUrl);
}
