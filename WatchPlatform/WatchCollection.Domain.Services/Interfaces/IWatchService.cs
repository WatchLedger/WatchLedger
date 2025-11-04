using System;
using WatchCollection.Api.Contracts;

namespace WatchCollection.Domain.Services.Interfaces;

public interface IWatchService
{
    WatchResponseContract CreateWatch(WatchRequestContract contract);
    WatchResponseContract? GetWatchById(Guid guid);
    IEnumerable<WatchResponseContract> GetAll();
}
