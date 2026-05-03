namespace PSControllerEmulator.Core.Controllers;

public static class DisplayExtensions
{
    public static string ToDisplayName(this ControllerModel model)
    {
        return model switch
        {
            ControllerModel.DualSense => "DualSense",
            ControllerModel.DualSenseEdge => "DualSense Edge",
            ControllerModel.DualShock4 => "DualShock 4",
            _ => "Unknown controller"
        };
    }

    public static string ToDisplayText(this ConnectionType connectionType)
    {
        return connectionType switch
        {
            ConnectionType.Usb => "USB",
            ConnectionType.Bluetooth => "Bluetooth",
            ConnectionType.MockUsb => "Mock USB",
            _ => "Unknown"
        };
    }
}
