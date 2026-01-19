using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Register;

public class InputModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = default!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = default!;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = default!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = default!;

    public string? ReturnUrl { get; set; }
}
