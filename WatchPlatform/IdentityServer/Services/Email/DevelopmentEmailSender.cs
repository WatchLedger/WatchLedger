using Microsoft.Extensions.Logging;

namespace IdentityServer.Services.Email;

public class DevelopmentEmailSender : IEmailSender
{
    private readonly ILogger<DevelopmentEmailSender> _logger;
    private readonly string _emailDirectory;

    public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger)
    {
        _logger = logger;
        _emailDirectory = Path.Combine(Path.GetTempPath(), "IdentityServer", "Emails");
        Directory.CreateDirectory(_emailDirectory);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
        var sanitizedSubject = SanitizeFileName(subject);
        var fileName = $"{timestamp}_{sanitizedSubject}.html";
        var filePath = Path.Combine(_emailDirectory, fileName);

        var emailContent = $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8">
                <title>{subject}</title>
            </head>
            <body>
                <div style="background: #f5f5f5; padding: 20px; margin-bottom: 20px; border-radius: 5px;">
                    <p><strong>To:</strong> {toEmail}</p>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Sent:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                </div>
                <hr>
                {htmlMessage}
            </body>
            </html>
            """;

        await File.WriteAllTextAsync(filePath, emailContent);

        _logger.LogInformation(
            "Development email saved to: {FilePath}\nTo: {ToEmail}\nSubject: {Subject}",
            filePath, toEmail, subject);
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName
            .Where(c => !invalidChars.Contains(c))
            .Take(50)
            .ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "email" : sanitized;
    }
}
