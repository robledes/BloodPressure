using System.ComponentModel.DataAnnotations;

namespace BloodPressure.Shared;

public class UserRegister
{
    [Required(ErrorMessage ="Identification card is required")]
    public string IdentificationCard { get; set; } = string.Empty;

    [Required(ErrorMessage ="Please enter a password"), StringLength(100, MinimumLength = 8, ErrorMessage ="Minimum length is 8")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "The passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string Firstname { get; set; } = string.Empty;

    public string Lastname { get; set; } = string.Empty;

    public char GenderCode { get; set; } = 'M';

    public string Gender => GenderCode == 'M' ? "Male" : "Female";

    [Required(ErrorMessage ="Birthdate is required")]
    public DateTime? BirthDate { get; set; }

    public int Age
    {
        get
        {
            if (BirthDate.HasValue)
            {
                int age = DateTime.Now.Year - BirthDate.Value.Year;

                if (DateTime.Now.DayOfYear + 1 < BirthDate.Value.DayOfYear)
                    age--;

                return age;
            }
            else
                return 0;
        }

        set { }
    }

    public byte[]? Photo { get; set; }
}
