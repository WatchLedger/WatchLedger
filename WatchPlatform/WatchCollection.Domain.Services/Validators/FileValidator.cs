using System;
using WatchCollection.Domain.Services.Exceptions;

namespace WatchCollection.Domain.Services.Validators;

public static class FileValidator
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public static void ValidateImageFile(string fileName, string contentType, long fileSize)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new FileValidationException("File name cannot be empty.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new FileValidationException("Content type cannot be empty.");

        if (fileSize <= 0)
            throw new FileValidationException("File size must be greater than 0.");

        if (fileSize > MaxFileSizeBytes)
            throw new FileValidationException($"File size exceeds maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new FileValidationException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");  
        if (!AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
            throw new FileValidationException($"Content type '{contentType}' is not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}");
    }
}
