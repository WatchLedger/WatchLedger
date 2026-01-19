using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.RegisterConfirmation;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    public string? Email { get; set; }
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet(string? email, string? returnUrl)
    {
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Account/Register/Index");
        }

        Email = email;
        ReturnUrl = returnUrl;
        return Page();
    }
}
