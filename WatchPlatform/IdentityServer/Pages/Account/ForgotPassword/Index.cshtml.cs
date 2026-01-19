using System.Text;
using System.Text.Encodings.Web;
using IdentityServer.Models;
using IdentityServer.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.Pages.ForgotPassword;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<Index> _logger;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Index(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<Index> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);

        // Always redirect to confirmation to prevent email enumeration
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            _logger.LogInformation("Password reset requested for non-existent or unconfirmed email: {Email}", Input.Email);
            return RedirectToPage("/Account/ForgotPasswordConfirmation/Index");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.Page(
            "/Account/ResetPassword/Index",
            pageHandler: null,
            values: new { code },
            protocol: Request.Scheme);

        await _emailSender.SendEmailAsync(
            Input.Email,
            "Reset Your Password",
            $"""
            <h1>Password Reset Request</h1>
            <p>You requested a password reset for your WatchLedger account.</p>
            <p>Click the link below to reset your password:</p>
            <p><a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>Reset Password</a></p>
            <p>If you did not request a password reset, you can safely ignore this email.</p>
            <p>This link will expire in 24 hours.</p>
            """);

        _logger.LogInformation("Password reset email sent to {Email}", Input.Email);

        return RedirectToPage("/Account/ForgotPasswordConfirmation/Index");
    }
}
