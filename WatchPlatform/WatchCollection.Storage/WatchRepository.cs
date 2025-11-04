using System;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Storage;

public class WatchRepository(WatchServiceDbContext _context) : IWatchRepository
{
    public Watch Create(Watch watch)
    {
        var addedWatch = watch;
        _context.Watches.Add(watch);
        _context.SaveChanges();
        return addedWatch;
    }

    public Watch? GetWatchById(Guid guid)
    {
        var watch = _context.Find<Watch>(guid);
        return watch;
    }

    public IEnumerable<Watch> GetAll()
    {
        var watches = _context.Watches;
        return watches;
    }
}
