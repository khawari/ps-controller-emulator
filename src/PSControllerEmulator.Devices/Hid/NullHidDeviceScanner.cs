namespace PSControllerEmulator.Devices.Hid;

public sealed class NullHidDeviceScanner : IHidDeviceScanner
{
    public Task<IReadOnlyList<HidDeviceInfo>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<HidDeviceInfo>>(Array.Empty<HidDeviceInfo>());
    }
}
