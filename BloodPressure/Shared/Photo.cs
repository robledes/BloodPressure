namespace BloodPressure.Shared;

public class Photo
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public byte[]? PhotoImage { get; set; }
}
