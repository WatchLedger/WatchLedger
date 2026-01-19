using System.Text;
using System.Text.Encodings.Web;
using IdentityServer.Models;
using IdentityServer.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.Pages.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<Index> _logger;

    public ViewModel View { get; set; } = new();

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

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (!RegisterOptions.AllowRegistration)
        {
            return RedirectToPage("/Account/Login/Index");
        }

        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!RegisterOptions.AllowRegistration)
        {
            return RedirectToPage("/Account/Login/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            PhoneNumber = Input.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with email {Email}", Input.Email);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail/Index",
                pageHandler: null,
                values: new { userId = user.Id, code, returnUrl = Input.ReturnUrl },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm your email",
                $"""
                <h1>Welcome to WatchLedger!</h1>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>Confirm your email</a></p>
                <p>If you did not create an account, you can safely ignore this email.</p>
                """);

            return RedirectToPage("/Account/RegisterConfirmation/Index",
                new { email = Input.Email, returnUrl = Input.ReturnUrl });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
