using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.ForgotPasswordConfirmation;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    public void OnGet()
    {
    }
}
