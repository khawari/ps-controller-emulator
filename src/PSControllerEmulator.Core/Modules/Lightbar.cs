namespace PSControllerEmulator.Core.Modules;

public readonly record struct LightbarColor(byte Red, byte Green, byte Blue)
{
    public static LightbarColor DefaultBlue { get; } = new(0, 76, 255);
}

public sealed record LightbarCommand(LightbarColor Color, TimeSpan FadeDuration);
