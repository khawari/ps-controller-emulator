using PSControllerEmulator.Core.Controllers;
using System.Text.RegularExpressions;

namespace PSControllerEmulator.Devices.Hid;

public static partial class HidTransportClassifier
{
    [GeneratedRegex(@"vid[_&](?<vid>[0-9a-f]{4,8}).*pid[_&](?<pid>[0-9a-f]{4})", RegexOptions.IgnoreCase)]
    private static partial Regex VidPidRegex();

    public static ConnectionType ClassifyConnectionType(string devicePath)
    {
        if (string.IsNullOrWhiteSpace(devicePath))
        {
            return ConnectionType.Unknown;
        }

        var normalized = Normalize(devicePath);

        if (normalized.StartsWith(@"\\?\hid#vid_", StringComparison.Ordinal)
            && normalized.Contains("&pid_", StringComparison.Ordinal))
        {
            return ConnectionType.Usb;
        }

        if ((normalized.StartsWith(@"\\?\hid#{00001124-", StringComparison.Ordinal)
                || normalized.StartsWith(@"\\?\bthenum#{00001124-", StringComparison.Ordinal))
            && normalized.Contains("vid&", StringComparison.Ordinal)
            && normalized.Contains("_pid&", StringComparison.Ordinal))
        {
            return ConnectionType.Bluetooth;
        }

        return ConnectionType.Unknown;
    }

    public static bool TryExtractVendorProductId(string devicePath, out ushort vendorId, out ushort productId)
    {
        vendorId = 0;
        productId = 0;

        if (string.IsNullOrWhiteSpace(devicePath))
        {
            return false;
        }

        var normalized = Normalize(devicePath);
        var match = VidPidRegex().Match(normalized);

        if (!match.Success)
        {
            return false;
        }

        var vendorText = match.Groups["vid"].Value;
        var productText = match.Groups["pid"].Value;

        if (vendorText.Length > 4)
        {
            vendorText = vendorText[^4..];
        }

        if (!ushort.TryParse(vendorText, System.Globalization.NumberStyles.HexNumber, null, out vendorId))
        {
            return false;
        }

        if (!ushort.TryParse(productText, System.Globalization.NumberStyles.HexNumber, null, out productId))
        {
            vendorId = 0;
            return false;
        }

        return true;
    }

    private static string Normalize(string devicePath)
    {
        return devicePath.Trim().Replace('/', '\\').ToLowerInvariant();
    }
}
