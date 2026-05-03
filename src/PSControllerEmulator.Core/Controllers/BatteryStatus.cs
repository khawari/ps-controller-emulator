namespace PSControllerEmulator.Core.Controllers;

public sealed record BatteryStatus(int? Percentage, bool IsCharging, bool IsAvailable)
{
    public static BatteryStatus Unknown { get; } = new(null, false, false);

    public static BatteryStatus FromPercentage(int percentage, bool isCharging = false)
    {
        return new BatteryStatus(Math.Clamp(percentage, 0, 100), isCharging, true);
    }

    public string ToDisplayText()
    {
        if (!IsAvailable || Percentage is null)
        {
            return "Battery unavailable";
        }

        return IsCharging ? $"{Percentage}% charging" : $"{Percentage}%";
    }
}
