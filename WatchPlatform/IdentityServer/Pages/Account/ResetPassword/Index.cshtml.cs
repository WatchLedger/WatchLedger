using System.Text;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.Pages.ResetPassword;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<Index> _logger;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Index(UserManager<ApplicationUser> userManager, ILogger<Index> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult OnGet(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("A code must be supplied for password reset.");
        }

        Input = new InputModel
        {
            Code = code
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToPage("/Account/ResetPasswordConfirmation/Index");
        }

        string code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Input.Code));
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Invalid password reset token.");
            return Page();
        }

        var result = await _userManager.ResetPasswordAsync(user, code, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {UserId} reset their password", user.Id);
            return RedirectToPage("/Account/ResetPasswordConfirmation/Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
