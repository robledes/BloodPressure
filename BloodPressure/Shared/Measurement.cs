namespace BloodPressure.Shared;

public class Measurement
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int? Systolic { get; set; }

    public string? SystolicOB { get; set; }

    public int? Diastolic { get; set; }

    public string? DiastolicOB { get; set; }

    public string SystolicDiastolic => Systolic.ToString()
        + " "
        + SystolicOB
        + " | ‌‌"
        + Diastolic.ToString()
        + " "
        + DiastolicOB;

    public int? Pulse { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string DateOnly => Date.ToShortDateString();

    public string TimeOnly => Date.ToString("HH:mm");

    public string DayMonthOnly => Date.ToString("dd/MM");
}
