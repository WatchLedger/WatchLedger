using System.Text;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.Pages.ConfirmEmail;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<Index> _logger;

    public bool EmailConfirmed { get; set; }
    public string? StatusMessage { get; set; }
    public string? ReturnUrl { get; set; }

    public Index(UserManager<ApplicationUser> userManager, ILogger<Index> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? userId, string? code, string? returnUrl)
    {
        ReturnUrl = returnUrl;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            StatusMessage = "Invalid email confirmation link.";
            EmailConfirmed = false;
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "Unable to find user.";
            EmailConfirmed = false;
            return Page();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch
        {
            StatusMessage = "Invalid email confirmation link.";
            EmailConfirmed = false;
            return Page();
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {UserId} confirmed their email", userId);
            StatusMessage = "Thank you for confirming your email. You can now log in.";
            EmailConfirmed = true;
        }
        else
        {
            StatusMessage = "Error confirming your email. The link may have expired.";
            EmailConfirmed = false;
        }

        return Page();
    }
}
