namespace PSControllerEmulator.Core.Controllers;

public interface IControllerDiscoveryService
{
    Task<IReadOnlyList<ConnectedController>> DiscoverAsync(CancellationToken cancellationToken = default);
}
