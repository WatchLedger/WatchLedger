using System;
using System.Reflection.Metadata.Ecma335;

namespace WatchCollection.Storage.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(Guid? id = null, string? message = null)
        : base(message ?? $"Entity with id '{(id.HasValue ? id.ToString() : "unknown")}' not found.") { }
}

public class WatchNotFoundException : EntityNotFoundException
{
    public WatchNotFoundException() : base() { }
    public WatchNotFoundException(Guid id) : base(id: null, message: $"Watch entity '{id}' not found.") { }
    public WatchNotFoundException(Guid id, string message) : base(id, message) { }

}

public class AdvertisementNotFoundExceptions : EntityNotFoundException
{
    public AdvertisementNotFoundExceptions() : base() { }
    public AdvertisementNotFoundExceptions(Guid id) : base(id: null, message: $"Watch entity '{id}' not found.") { }
    public AdvertisementNotFoundExceptions(Guid id, string message) : base(id, message) { }
}

public class WatchImageNotFoundException : EntityNotFoundException
{
    public WatchImageNotFoundException() : base() { }
    public WatchImageNotFoundException(Guid id) : base(id: null, message: $"Watch entity '{id}' not found.") { }
    public WatchImageNotFoundException(Guid id, string message) : base(id, message) { }
}
