namespace PSControllerEmulator.Core.Modules;

public readonly record struct RumbleCommand(byte LowFrequencyMotor, byte HighFrequencyMotor, TimeSpan Duration);

public readonly record struct HapticCommand(float LeftIntensity, float RightIntensity, TimeSpan Duration);
