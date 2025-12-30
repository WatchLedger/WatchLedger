using System;

namespace WatchCollection.Domain.Services.Exceptions;

public class InvalidAdvertisementStatusException : Exception
{
    public InvalidAdvertisementStatusException(string? message) : base(message)
    {
    }

    public InvalidAdvertisementStatusException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
