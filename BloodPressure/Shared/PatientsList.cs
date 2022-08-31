namespace BloodPressure.Shared;

public class PatientsList
{
    public int Id { get; set; }
    public string StrId => "#" + Id.ToString();

    public string IdentificationCard { get; set; }

    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string FullName => (Lastname + ", " + Firstname);

    public char GenderCode { get; set; }
    public string Gender => GenderCode == 'M' ? "Male" : "Female";

    public DateTime? BirthDate { get; set; }
    public string ShortBirthDate
    {
        get
        {
            DateTime date = (DateTime)BirthDate;
            return date.ToShortDateString();
        }
    }

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
    }

    public char Role { get; set; }
    public string RoleDesc => Role == 'A' ? "Admin" : "User";
}
