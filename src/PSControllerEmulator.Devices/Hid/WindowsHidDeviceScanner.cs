using Microsoft.Win32.SafeHandles;
using PSControllerEmulator.Core.Controllers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PSControllerEmulator.Devices.Hid;

public sealed class WindowsHidDeviceScanner : IHidDeviceScanner
{
    private const int DigcfPresent = 0x00000002;
    private const int DigcfDeviceinterface = 0x00000010;
    private const uint MetadataOnlyAccess = 0;
    private const uint FileShareRead = 0x00000001;
    private const uint FileShareWrite = 0x00000002;
    private const uint OpenExisting = 3;
    private const uint FileFlagOverlapped = 0x40000000;
    private static readonly IntPtr InvalidHandleValue = new(-1);

    public Task<IReadOnlyList<HidDeviceInfo>> ScanAsync(CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            return Task.FromResult<IReadOnlyList<HidDeviceInfo>>(Array.Empty<HidDeviceInfo>());
        }

        cancellationToken.ThrowIfCancellationRequested();

        HidD_GetHidGuid(out var hidGuid);
        var deviceInfoSet = SetupDiGetClassDevs(
            ref hidGuid,
            null,
            IntPtr.Zero,
            DigcfPresent | DigcfDeviceinterface);

        if (deviceInfoSet == InvalidHandleValue)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to enumerate HID devices.");
        }

        var devices = new List<HidDeviceInfo>();

        try
        {
            var index = 0u;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var interfaceData = SP_DEVICE_INTERFACE_DATA.Create();

                if (!SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref hidGuid, index, ref interfaceData))
                {
                    var error = Marshal.GetLastWin32Error();

                    if (error == Win32ErrorNoMoreItems)
                    {
                        break;
                    }

                    throw new Win32Exception(error, "Unable to enumerate a HID device interface.");
                }

                var path = GetDevicePath(deviceInfoSet, interfaceData);

                if (!string.IsNullOrWhiteSpace(path) && TryReadDeviceInfo(path, out var deviceInfo))
                {
                    devices.Add(deviceInfo);
                }

                index++;
            }
        }
        finally
        {
            SetupDiDestroyDeviceInfoList(deviceInfoSet);
        }

        return Task.FromResult<IReadOnlyList<HidDeviceInfo>>(devices);
    }

    private static string GetDevicePath(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA interfaceData)
    {
        SetupDiGetDeviceInterfaceDetail(
            deviceInfoSet,
            ref interfaceData,
            IntPtr.Zero,
            0,
            out var requiredSize,
            IntPtr.Zero);

        if (requiredSize == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to determine HID device path length.");
        }

        var detailData = Marshal.AllocHGlobal((int)requiredSize);

        try
        {
            Marshal.WriteInt32(detailData, IntPtr.Size == 8 ? 8 : 6);

            if (!SetupDiGetDeviceInterfaceDetail(
                deviceInfoSet,
                ref interfaceData,
                detailData,
                requiredSize,
                out _,
                IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to read HID device path.");
            }

            return Marshal.PtrToStringUni(IntPtr.Add(detailData, 4)) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(detailData);
        }
    }

    private static bool TryReadDeviceInfo(string devicePath, out HidDeviceInfo deviceInfo)
    {
        deviceInfo = default!;

        if (!HidTransportClassifier.IsUsbDevicePath(devicePath))
        {
            return false;
        }

        using var handle = CreateFile(
            devicePath,
            MetadataOnlyAccess,
            FileShareRead | FileShareWrite,
            IntPtr.Zero,
            OpenExisting,
            FileFlagOverlapped,
            IntPtr.Zero);

        if (handle.IsInvalid)
        {
            return false;
        }

        var attributes = HIDD_ATTRIBUTES.Create();

        if (!HidD_GetAttributes(handle, ref attributes))
        {
            return false;
        }

        deviceInfo = new HidDeviceInfo(
            devicePath,
            attributes.VendorID,
            attributes.ProductID,
            ReadHidString(handle, HidStringKind.Product),
            ReadHidString(handle, HidStringKind.Manufacturer),
            ConnectionType.Usb);

        return true;
    }

    private static string ReadHidString(SafeFileHandle handle, HidStringKind kind)
    {
        var buffer = new StringBuilder(256);
        var success = kind switch
        {
            HidStringKind.Product => HidD_GetProductString(handle, buffer, buffer.Capacity * sizeof(char)),
            HidStringKind.Manufacturer => HidD_GetManufacturerString(handle, buffer, buffer.Capacity * sizeof(char)),
            _ => false
        };

        return success ? buffer.ToString().TrimEnd('\0') : string.Empty;
    }

    private enum HidStringKind
    {
        Product,
        Manufacturer
    }

    private const int Win32ErrorNoMoreItems = 259;

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVICE_INTERFACE_DATA
    {
        public int cbSize;
        public Guid InterfaceClassGuid;
        public int Flags;
        public IntPtr Reserved;

        public static SP_DEVICE_INTERFACE_DATA Create()
        {
            return new SP_DEVICE_INTERFACE_DATA
            {
                cbSize = Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>()
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HIDD_ATTRIBUTES
    {
        public int Size;
        public ushort VendorID;
        public ushort ProductID;
        public ushort VersionNumber;

        public static HIDD_ATTRIBUTES Create()
        {
            return new HIDD_ATTRIBUTES
            {
                Size = Marshal.SizeOf<HIDD_ATTRIBUTES>()
            };
        }
    }

    [DllImport("hid.dll")]
    private static extern void HidD_GetHidGuid(out Guid hidGuid);

    [DllImport("hid.dll", SetLastError = true)]
    private static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

    [DllImport("hid.dll", EntryPoint = "HidD_GetProductString", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern bool HidD_GetProductString(SafeFileHandle hidDeviceObject, StringBuilder buffer, int bufferLength);

    [DllImport("hid.dll", EntryPoint = "HidD_GetManufacturerString", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern bool HidD_GetManufacturerString(SafeFileHandle hidDeviceObject, StringBuilder buffer, int bufferLength);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr SetupDiGetClassDevs(
        ref Guid classGuid,
        string? enumerator,
        IntPtr hwndParent,
        int flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInterfaces(
        IntPtr deviceInfoSet,
        IntPtr deviceInfoData,
        ref Guid interfaceClassGuid,
        uint memberIndex,
        ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(
        IntPtr deviceInfoSet,
        ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
        IntPtr deviceInterfaceDetailData,
        uint deviceInterfaceDetailDataSize,
        out uint requiredSize,
        IntPtr deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeFileHandle CreateFile(
        string fileName,
        uint desiredAccess,
        uint shareMode,
        IntPtr securityAttributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);
}
