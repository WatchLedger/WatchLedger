using System;

namespace WatchCollection.Domain.Services.Exceptions;

public class EntityUnavailableException : Exception
{
    public EntityUnavailableException(string message) : base(message)
    {
    }
}

public class BrandsUnavailableException : EntityUnavailableException
{
    public BrandsUnavailableException(string message) : base(message)
    {
    }
}

public class ValuationUnavailableException : EntityUnavailableException
{
    public ValuationUnavailableException(string message) : base(message)
    {
    }
}