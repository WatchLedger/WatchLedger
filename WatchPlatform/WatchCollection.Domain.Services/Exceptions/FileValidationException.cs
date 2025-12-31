using System;

namespace WatchCollection.Domain.Services.Exceptions;

public class FileValidationException : Exception
{
    public FileValidationException()
    {
    }
    
    public FileValidationException(string message) : base(message)
    {
    }
}
