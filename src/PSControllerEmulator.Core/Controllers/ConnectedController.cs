namespace PSControllerEmulator.Core.Controllers;

public sealed record ConnectedController(
    string InstanceId,
    ControllerModel Model,
    string DisplayName,
    ConnectionType ConnectionType,
    BatteryStatus Battery,
    ControllerInputState InputState,
    ControllerCapabilities Capabilities,
    bool IsMock,
    DateTimeOffset LastSeen);
