using System;

namespace WatchCollection.Storage.Exceptions;

public class BidTooLowException : Exception
{
    public BidTooLowException() : base("The bid amount is too low.")
    {
    }
    public BidTooLowException(string message) : base(message)
    {
    }
}
