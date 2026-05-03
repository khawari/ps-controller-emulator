using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices;
using PSControllerEmulator.Devices.Hid;

namespace PSControllerEmulator.Tests;

public sealed class MockDiscoveryTests
{
    [Fact]
    public async Task MockDiscoveryReturnsOneDualSense()
    {
        var discovery = new MockControllerDiscoveryService();

        var controllers = await discovery.DiscoverAsync();

        var controller = Assert.Single(controllers);
        Assert.Equal(ControllerModel.DualSense, controller.Model);
        Assert.Equal("Mock DualSense", controller.DisplayName);
        Assert.Equal(ConnectionType.MockUsb, controller.ConnectionType);
        Assert.True(controller.IsMock);
    }

    [Fact]
    public async Task AppDiscoveryFallsBackToMockWhenHardwareIsEmpty()
    {
        var hardware = new HardwareControllerDiscoveryService(new FakeHidDeviceScanner([]));
        var discovery = new ControllerDiscoveryService(hardware, new MockControllerDiscoveryService(), enableMockFallback: true);

        var controllers = await discovery.DiscoverAsync();

        Assert.Single(controllers);
        Assert.True(controllers[0].IsMock);
    }

    [Fact]
    public async Task HardwareDiscoveryMapsKnownSonyPid()
    {
        var scanner = new FakeHidDeviceScanner(
        [
            new HidDeviceInfo(
                "hid://dualsense",
                PlayStationControllerDetector.SonyVendorId,
                PlayStationControllerDetector.DualSenseProductId,
                "Wireless Controller",
                ConnectionType.Usb)
        ]);
        var discovery = new HardwareControllerDiscoveryService(scanner);

        var controllers = await discovery.DiscoverAsync();

        var controller = Assert.Single(controllers);
        Assert.Equal(ControllerModel.DualSense, controller.Model);
        Assert.Equal(ConnectionType.Usb, controller.ConnectionType);
        Assert.False(controller.IsMock);
    }

    private sealed class FakeHidDeviceScanner(IReadOnlyList<HidDeviceInfo> devices) : IHidDeviceScanner
    {
        public Task<IReadOnlyList<HidDeviceInfo>> ScanAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(devices);
        }
    }
}
