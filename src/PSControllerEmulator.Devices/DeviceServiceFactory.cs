using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices.Hid;

namespace PSControllerEmulator.Devices;

public static class DeviceServiceFactory
{
    public static IControllerDiscoveryService CreateForApp()
    {
        return CreateDefault(enableMockFallback: true);
    }

    public static IControllerDiscoveryService CreateForCli(bool useMock)
    {
        return useMock ? new MockControllerDiscoveryService() : CreateDefault(enableMockFallback: false);
    }

    public static IControllerDiscoveryService CreateDefault(bool enableMockFallback)
    {
        return new ControllerDiscoveryService(
            new HardwareControllerDiscoveryService(new NullHidDeviceScanner()),
            new MockControllerDiscoveryService(),
            enableMockFallback);
    }
}
