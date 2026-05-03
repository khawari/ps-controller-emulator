namespace PSControllerEmulator.Devices.Hid;

public interface IHidDeviceScanner
{
    Task<IReadOnlyList<HidDeviceInfo>> ScanAsync(CancellationToken cancellationToken = default);
}
