using System.ComponentModel.DataAnnotations;

namespace BloodPressure.Shared;

public class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Identification card is required")]
    public string IdentificationCard { get; set; } = string.Empty;

    public string Firstname { get; set; } = string.Empty;

    public string Lastname { get; set; } = string.Empty;

    public string FullName => Firstname + " " + Lastname;

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public char GenderCode { get; set; } = 'M';

    public string Gender => GenderCode == 'M' ? "Male" : "Female";

    [Required(ErrorMessage = "Birthdate is required")]
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

    public char Role { get; set; } = 'U';
}
