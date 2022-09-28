namespace BloodPressure.Shared;

public static class Helpers
{
    public static string MsgFormat(params string[] lines)
    {
        string formatted = "<div style=\"font-size: 1.1em; text-align: center;\">";

        for (int i = 0; i < lines.Length - 1; i++)
            formatted = $"{formatted}{lines[i]}<br />";

        return $"{formatted}{lines[^1]}</div>";
    }
}
