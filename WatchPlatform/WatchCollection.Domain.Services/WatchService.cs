using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Exceptions;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Domain.Services;

public class WatchService(IWatchRepository _repository, IWatchImageService _watchImageService) : IWatchService
{
    public async Task<WatchResponseContract> CreateWatch(WatchRequestContract contract)
    {
        var model = contract.AsModel();
        model.OwnerId = Guid.NewGuid();
        model.WatchId = Guid.NewGuid();
        model.CreatedAt = DateTimeOffset.Now;
        model.UpdatedAt = model.CreatedAt;

        var entity = model.AsEntity();
        var created = await _repository.Create(entity);

        return created.AsModel().AsContract();
    }

    public async Task<WatchResponseContract?> GetWatchById(Guid guid)
    {
        var watch = await _repository.GetWatchById(guid);

        if (watch is null)
            return null;

        return watch.AsModel().AsContract();
    }

    public async Task<IEnumerable<WatchResponseContract>> GetAll()
    {
        var watches = await _repository.GetAll();
        return watches.Select(wrc => wrc.AsModel().AsContract());
    }

    public async Task<IEnumerable<WatchResponseContract>> GetWatchesByBrand(string brand)
    {
        var watches = await _repository.GetWatchesByBrand(brand);
        return watches.Select(wrc => wrc.AsModel().AsContract());
    }

    public async Task<WatchResponseContract> UpdateWatch(Guid watchId, WatchRequestContract contract)
    {
        try
        {
            var model = contract.AsModel();
            model.UpdatedAt = DateTimeOffset.Now;

            var entity = model.AsEntity();
            var updatedWatch = await _repository.UpdateWatch(watchId, entity);
        
            return updatedWatch.AsModel().AsContract();
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
    }

    public async Task DeleteWatch(Guid watchId)
    {
        await _repository.DeleteWatch(watchId);
        await _watchImageService.DeleteImagesByWatchIdAsync(watchId);
    }
}
