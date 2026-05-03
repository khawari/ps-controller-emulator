using PSControllerEmulator.Core.Controllers;

namespace PSControllerEmulator.Devices.Hid;

public sealed record HidDeviceInfo(
    string DevicePath,
    ushort VendorId,
    ushort ProductId,
    string ProductName,
    ConnectionType ConnectionType);
