using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.ForgotPassword;

public class InputModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;
}
