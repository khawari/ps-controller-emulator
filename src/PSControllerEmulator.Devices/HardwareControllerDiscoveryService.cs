using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices.Hid;

namespace PSControllerEmulator.Devices;

public sealed class HardwareControllerDiscoveryService : IControllerDiscoveryService
{
    private readonly IHidDeviceScanner _scanner;

    public HardwareControllerDiscoveryService(IHidDeviceScanner scanner)
    {
        _scanner = scanner;
    }

    public async Task<IReadOnlyList<ConnectedController>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var devices = await _scanner.ScanAsync(cancellationToken);
        var controllers = new List<ConnectedController>();

        foreach (var device in devices)
        {
            if (PlayStationControllerDetector.TryCreateController(device, out var controller) && controller is not null)
            {
                controllers.Add(controller);
            }
        }

        return controllers;
    }
}
