using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityServer.Services.Email;

public class SendGridEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var apiKey = _configuration["SendGridApiKey"]
            ?? throw new InvalidOperationException("SendGridApiKey is not configured");
        var fromEmail = _configuration["SendGridFromEmail"]
            ?? throw new InvalidOperationException("SendGridFromEmail is not configured");
        var fromName = _configuration["SendGridFromName"] ?? "WatchLedger";

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);

        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email sent successfully to {ToEmail} via SendGrid", toEmail);
        }
        else
        {
            var responseBody = await response.Body.ReadAsStringAsync();
            _logger.LogError(
                "Failed to send email to {ToEmail} via SendGrid. Status: {StatusCode}, Response: {Response}",
                toEmail, response.StatusCode, responseBody);
            throw new InvalidOperationException($"SendGrid returned {response.StatusCode}: {responseBody}");
        }
    }
}
