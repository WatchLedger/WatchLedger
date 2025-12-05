namespace WatchValuation.Domain.Services.Exceptions;

public class WatchValuationUnavailableException : Exception
{
    public WatchValuationUnavailableException(string? message) : base(message)
    {
    }
}
