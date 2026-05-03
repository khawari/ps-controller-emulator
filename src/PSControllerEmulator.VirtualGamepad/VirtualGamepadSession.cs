namespace PSControllerEmulator.VirtualGamepad;

public sealed record VirtualGamepadSession(
    string SessionId,
    VirtualControllerTarget Target,
    bool IsActive,
    string Status);
