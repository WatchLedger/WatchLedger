using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Services.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var smtpHost = _configuration["SmtpHost"]
            ?? throw new InvalidOperationException("SmtpHost is not configured");
        var smtpPort = int.Parse(_configuration["SmtpPort"] ?? "587");
        var smtpUser = _configuration["SmtpUser"]
            ?? throw new InvalidOperationException("SmtpUser is not configured");
        var smtpPassword = _configuration["SmtpPassword"]
            ?? throw new InvalidOperationException("SmtpPassword is not configured");
        var fromEmail = _configuration["SmtpFromEmail"]
            ?? throw new InvalidOperationException("SmtpFromEmail is not configured");
        var fromName = _configuration["SmtpFromName"] ?? "WatchLedger";

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            throw;
        }
    }
}
