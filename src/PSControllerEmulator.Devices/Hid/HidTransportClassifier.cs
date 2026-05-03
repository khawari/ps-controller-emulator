namespace PSControllerEmulator.Devices.Hid;

public static class HidTransportClassifier
{
    public static bool IsUsbDevicePath(string devicePath)
    {
        if (string.IsNullOrWhiteSpace(devicePath))
        {
            return false;
        }

        var normalized = devicePath.Trim().Replace('/', '\\').ToLowerInvariant();

        return normalized.StartsWith(@"\\?\hid#vid_", StringComparison.Ordinal)
            && normalized.Contains("&pid_", StringComparison.Ordinal);
    }
}
