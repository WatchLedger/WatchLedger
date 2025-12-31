using System;
using System.Reflection.Metadata.Ecma335;

namespace WatchCollection.Storage.Exceptions;

public class EntityNotFoundException : Exception
{
    public Guid EntityId { get; }

    public EntityNotFoundException(Guid id, string? message = null)
        : base(message ?? "Entity not found.") 
    {
        EntityId = id;
    }
}

public class WatchNotFoundException : EntityNotFoundException
{
    public WatchNotFoundException(Guid id) : base(id) { }
    public WatchNotFoundException(Guid id, string message) : base(id, message) { }
}

public class AdvertisementNotFoundExceptions : EntityNotFoundException
{
    public AdvertisementNotFoundExceptions(Guid id) : base(id) { }
    public AdvertisementNotFoundExceptions(Guid id, string message) : base(id, message) { }
}

public class WatchImageNotFoundException : EntityNotFoundException
{
    public WatchImageNotFoundException(Guid id) : base(id) { }
    public WatchImageNotFoundException(Guid id, string message) : base(id, message) { }
}

public class BidNotFoundExceptions : EntityNotFoundException
{
    public BidNotFoundExceptions(Guid id) : base(id) { }
    public BidNotFoundExceptions(Guid id, string message) : base(id, message) { }
}
