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
                "Sony Interactive Entertainment",
                ConnectionType.Usb)
        ]);
        var discovery = new HardwareControllerDiscoveryService(scanner);

        var controllers = await discovery.DiscoverAsync();

        var controller = Assert.Single(controllers);
        Assert.Equal(ControllerModel.DualSense, controller.Model);
        Assert.Equal(ConnectionType.Usb, controller.ConnectionType);
        Assert.False(controller.IsMock);
    }

    [Theory]
    [InlineData(PlayStationControllerDetector.DualSenseProductId, ControllerModel.DualSense)]
    [InlineData(PlayStationControllerDetector.DualSenseEdgeProductId, ControllerModel.DualSenseEdge)]
    [InlineData(PlayStationControllerDetector.DualShock4ProductId, ControllerModel.DualShock4)]
    [InlineData(PlayStationControllerDetector.DualShock4SlimProductId, ControllerModel.DualShock4)]
    public void DetectorMapsSupportedSonyUsbProducts(int productId, ControllerModel expectedModel)
    {
        var device = new HidDeviceInfo(
            "hid://controller",
            PlayStationControllerDetector.SonyVendorId,
            (ushort)productId,
            string.Empty,
            "Sony Interactive Entertainment",
            ConnectionType.Usb);

        var mapped = PlayStationControllerDetector.TryCreateController(device, out var controller);

        Assert.True(mapped);
        Assert.NotNull(controller);
        Assert.Equal(expectedModel, controller.Model);
        Assert.Equal(expectedModel.ToDisplayName(), controller.DisplayName);
    }

    [Fact]
    public void DetectorIgnoresUnsupportedSonyUsbProduct()
    {
        var device = new HidDeviceInfo(
            "hid://unsupported",
            PlayStationControllerDetector.SonyVendorId,
            0x0001,
            "Unsupported Sony HID",
            "Sony",
            ConnectionType.Usb);

        var mapped = PlayStationControllerDetector.TryCreateController(device, out var controller);

        Assert.False(mapped);
        Assert.Null(controller);
    }

    [Fact]
    public async Task EmptyHardwareScanReturnsNoControllers()
    {
        var discovery = new HardwareControllerDiscoveryService(new FakeHidDeviceScanner([]));

        var controllers = await discovery.DiscoverAsync();

        Assert.Empty(controllers);
    }

    [Fact]
    public async Task CliMockModeBypassesHardwareScanner()
    {
        var discovery = DeviceServiceFactory.CreateForCli(useMock: true, new ThrowingHidDeviceScanner());

        var controllers = await discovery.DiscoverAsync();

        var controller = Assert.Single(controllers);
        Assert.True(controller.IsMock);
        Assert.Equal(ControllerModel.DualSense, controller.Model);
    }

    private sealed class FakeHidDeviceScanner(IReadOnlyList<HidDeviceInfo> devices) : IHidDeviceScanner
    {
        public Task<IReadOnlyList<HidDeviceInfo>> ScanAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(devices);
        }
    }

    private sealed class ThrowingHidDeviceScanner : IHidDeviceScanner
    {
        public Task<IReadOnlyList<HidDeviceInfo>> ScanAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Hardware scanner should not run in mock mode.");
        }
    }
}
