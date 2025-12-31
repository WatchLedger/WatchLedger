using System;

namespace WatchCollection.Domain.Services.Exceptions;

public class DomainInvalidOperationException : Exception
{
    public DomainInvalidOperationException()
    {
    }

    public DomainInvalidOperationException(string? message) : base(message)
    {
    }
}
