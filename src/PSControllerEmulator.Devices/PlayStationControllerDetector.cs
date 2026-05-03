using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices.Hid;

namespace PSControllerEmulator.Devices;

public static class PlayStationControllerDetector
{
    public const ushort SonyVendorId = 0x054C;
    public const ushort DualShock4ProductId = 0x05C4;
    public const ushort DualShock4SlimProductId = 0x09CC;
    public const ushort DualSenseProductId = 0x0CE6;
    public const ushort DualSenseEdgeProductId = 0x0DF2;

    public static bool TryCreateController(HidDeviceInfo device, out ConnectedController? controller)
    {
        controller = null;

        if (device.VendorId != SonyVendorId)
        {
            return false;
        }

        var model = device.ProductId switch
        {
            DualSenseProductId => ControllerModel.DualSense,
            DualSenseEdgeProductId => ControllerModel.DualSenseEdge,
            DualShock4ProductId => ControllerModel.DualShock4,
            DualShock4SlimProductId => ControllerModel.DualShock4,
            _ => ControllerModel.Unknown
        };

        if (model == ControllerModel.Unknown)
        {
            return false;
        }

        var displayName = string.IsNullOrWhiteSpace(device.ProductName)
            ? model.ToDisplayName()
            : device.ProductName;

        controller = new ConnectedController(
            device.DevicePath,
            model,
            displayName,
            device.ConnectionType,
            BatteryStatus.Unknown,
            ControllerInputState.Neutral,
            ControllerCapabilities.For(model),
            IsMock: false,
            DateTimeOffset.UtcNow);

        return true;
    }
}
