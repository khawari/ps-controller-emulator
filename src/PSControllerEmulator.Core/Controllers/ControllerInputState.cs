namespace PSControllerEmulator.Core.Controllers;

public sealed record StickPosition(double X, double Y)
{
    public static StickPosition Centered { get; } = new(0, 0);
}

public sealed record TriggerState(byte Left, byte Right)
{
    public static TriggerState Released { get; } = new(0, 0);
}

public sealed record ControllerInputState(
    StickPosition LeftStick,
    StickPosition RightStick,
    TriggerState Triggers,
    IReadOnlyList<string> PressedButtons)
{
    public static ControllerInputState Neutral { get; } = new(
        StickPosition.Centered,
        StickPosition.Centered,
        TriggerState.Released,
        Array.Empty<string>());

    public string ToDisplayText()
    {
        var buttons = PressedButtons.Count == 0 ? "none" : string.Join(", ", PressedButtons);

        return string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"Left stick ({LeftStick.X:0.00}, {LeftStick.Y:0.00}), right stick ({RightStick.X:0.00}, {RightStick.Y:0.00}), L2 {Triggers.Left}, R2 {Triggers.Right}, buttons {buttons}");
    }
}
