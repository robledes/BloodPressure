using System.ComponentModel.DataAnnotations;

namespace BloodPressure.Shared;

public class UserChangePassword
{
    [Required(ErrorMessage = "Please enter a password"), StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
