using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.Devices;

public sealed class MockControllerDiscoveryService : IControllerDiscoveryService
{
    public Task<IReadOnlyList<ConnectedController>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ConnectedController>>([MockControllerFactory.CreateDualSense()]);
    }
}
