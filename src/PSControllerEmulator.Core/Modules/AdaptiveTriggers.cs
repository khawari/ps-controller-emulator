namespace PSControllerEmulator.Core.Modules;

public enum AdaptiveTriggerMode
{
    Off,
    Resistance,
    TriggerStop,
    Vibration
}

public sealed record AdaptiveTriggerEffect(
    AdaptiveTriggerMode Mode,
    byte StartPosition,
    byte EndPosition,
    byte Strength);
