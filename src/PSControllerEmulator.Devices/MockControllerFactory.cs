using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.Devices;

public static class MockControllerFactory
{
    public static ConnectedController CreateDualSense()
    {
        return new ConnectedController(
            "mock-dualsense-001",
            ControllerModel.DualSense,
            "Mock DualSense",
            ConnectionType.MockUsb,
            BatteryStatus.FromPercentage(87, isCharging: true),
            ControllerInputState.Neutral,
            ControllerCapabilities.For(ControllerModel.DualSense),
            IsMock: true,
            DateTimeOffset.UtcNow);
    }
}
