using System;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Domain.Services.Mapping;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Domain.Services;

public class WatchService(IWatchRepository repository) : IWatchService
{
    public WatchResponseContract CreateWatch(WatchRequestContract contract)
    {
        var model = contract.AsModel();
        model.OwnerId = Guid.NewGuid();
        model.WatchId = Guid.NewGuid();
        model.CreatedAt = DateTimeOffset.Now.Date;
        model.UpdatedAt = model.CreatedAt;

        var entity = model.AsEntity();
        var created = repository.Create(entity);

        return created.AsModel().AsContract();
    }

    public WatchResponseContract? GetWatchById(Guid guid)
    {
        var watch = repository.GetWatchById(guid);
        if (watch is null)
            return null;
        return watch.AsModel().AsContract();
    }

    public IEnumerable<WatchResponseContract> GetAll()
    {
        return repository.GetAll().Select(wrc => wrc.AsModel().AsContract());
    }
}
