using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Domain.Services;

public class WatchService(IWatchRepository _repository, IWatchImageService _watchImageService, IWatchValuationHttpClient _watchValuationClient) : IWatchService
{
    public async Task<WatchResponseContract> CreateWatch(string ownerIdString, WatchRequestContract contract)
    {
        var model = contract.AsModel();
        model.WatchId = Guid.NewGuid();
        model.OwnerId = Guid.TryParse(ownerIdString, out var ownerId) ? ownerId : throw new Exception("Invalid owner ID format.");
        var entity = model.AsEntity();
        var created = await _repository.Create(entity);

        return created.AsModel().AsContract();
    }

    public async Task<WatchResponseContract?> GetWatchById(string ownerIdString, Guid guid)
    {
        var watch = await _repository.GetWatchById(guid);

        if (watch is null)
            return null;

        if (watch.OwnerUserId.ToString() != ownerIdString)
            throw new UnauthorizedAccessException("You do not have access to this watch.");
        return watch.AsModel().AsContract();
    }

    public async Task<IEnumerable<WatchResponseContract>> GetAll(string ownerIdString)
    {
        var ownerId = Guid.TryParse(ownerIdString, out var oid) ? oid : throw new Exception("Invalid owner ID format.");
        var watches = await _repository.GetAll(ownerId);
        return watches.Select(wrc => wrc.AsModel().AsContract());
    }

    public async Task<IEnumerable<WatchResponseContract>> GetWatchesByBrand(string ownerIdString, string brand)
    {
        var ownerId = Guid.TryParse(ownerIdString, out var oid) ? oid : throw new Exception("Invalid owner ID format.");
        var watches = await _repository.GetWatchesByBrand(ownerId, brand);
        return watches.Select(wrc => wrc.AsModel().AsContract());
    }

    public async Task<WatchResponseContract> UpdateWatch(string ownerIdString, Guid watchId, WatchRequestContract contract)
    {
        var watch = await _repository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId);

        if (watch.OwnerUserId.ToString() != ownerIdString){
            // only the owner can update their watch no need for admin override here
            throw new UnauthorizedAccessException("You do not have access to this watch.");
        }

        var model = contract.AsModel();
        model.WatchId = watchId;
        model.OwnerId = Guid.TryParse(ownerIdString, out var ownerId) ? ownerId : throw new Exception("Invalid owner ID format.");
        var entity = model.AsEntity();
        var updatedWatch = await _repository.UpdateWatch(watchId, entity);
        
        return updatedWatch.AsModel().AsContract();
    }

    public async Task DeleteWatch(string ownerIdString, Guid watchId)
    {
        var watch = await _repository.GetWatchById(watchId);
        if (watch is null)
            throw new WatchNotFoundException(watchId);
        
        if (watch.OwnerUserId.ToString() != ownerIdString){
            // only the owner can delete their watch no need for admin override here
            throw new UnauthorizedAccessException("You do not have access to this watch.");
        }

        var filenames = await _watchImageService.GetFilenamesByWatchIdAsync(watchId);
        await _repository.DeleteWatch(watchId);
        foreach (var filename in filenames)
        {
            await _watchImageService.DeleteBlobsAsync(filename);
        }
    }

    public async Task<IEnumerable<string>> GetWatchBrands()
    {
        var brands = await _watchValuationClient.GetWatchBrandsAsync();
        if (brands is null || !brands.Any())
            throw new BrandsUnavailableException("No watch brands available.");
        return brands;
    }
}
