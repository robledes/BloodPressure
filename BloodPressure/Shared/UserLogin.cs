using System.ComponentModel.DataAnnotations;

namespace BloodPressure.Shared;

public class UserLogin
{
    [Required(ErrorMessage = "Identification card is required")]
    public string IdentificationCard { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a password")]
    public string Password { get; set; } = string.Empty;
}
