using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.Devices;

public sealed class ControllerDiscoveryService : IControllerDiscoveryService
{
    private readonly IControllerDiscoveryService _hardwareDiscoveryService;
    private readonly IControllerDiscoveryService _mockDiscoveryService;
    private readonly bool _enableMockFallback;

    public ControllerDiscoveryService(
        IControllerDiscoveryService hardwareDiscoveryService,
        IControllerDiscoveryService mockDiscoveryService,
        bool enableMockFallback)
    {
        _hardwareDiscoveryService = hardwareDiscoveryService;
        _mockDiscoveryService = mockDiscoveryService;
        _enableMockFallback = enableMockFallback;
    }

    public async Task<IReadOnlyList<ConnectedController>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var hardwareControllers = await _hardwareDiscoveryService.DiscoverAsync(cancellationToken);

        if (hardwareControllers.Count > 0 || !_enableMockFallback)
        {
            return hardwareControllers;
        }

        return await _mockDiscoveryService.DiscoverAsync(cancellationToken);
    }
}
