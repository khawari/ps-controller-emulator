using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices.Hid;

namespace PSControllerEmulator.Devices;

public static class DeviceServiceFactory
{
    public static IControllerDiscoveryService CreateForApp()
    {
        return CreateDefault(enableMockFallback: true);
    }

    public static IControllerDiscoveryService CreateForCli(bool useMock, IHidDeviceScanner? scanner = null)
    {
        return useMock ? new MockControllerDiscoveryService() : CreateDefault(enableMockFallback: false, scanner);
    }

    public static IControllerDiscoveryService CreateDefault(bool enableMockFallback, IHidDeviceScanner? scanner = null)
    {
        return new ControllerDiscoveryService(
            new HardwareControllerDiscoveryService(scanner ?? CreateHidDeviceScanner()),
            new MockControllerDiscoveryService(),
            enableMockFallback);
    }

    private static IHidDeviceScanner CreateHidDeviceScanner()
    {
        return OperatingSystem.IsWindows()
            ? new WindowsHidDeviceScanner()
            : new NullHidDeviceScanner();
    }
}
