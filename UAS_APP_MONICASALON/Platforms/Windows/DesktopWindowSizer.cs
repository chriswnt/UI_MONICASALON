using System.Runtime.InteropServices;

namespace UAS_APP_MONICASALON;

/// <summary>
/// Sizes the desktop window against the monitor work area instead of the
/// logical display size. MAUI window sizes are in device-independent units,
/// so a fixed height can overflow shorter screens at 125% scaling.
/// </summary>
internal static class DesktopWindowSizer
{
    private const uint MonitorDefaultToNearest = 2;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;

    public static void Apply(Microsoft.Maui.Controls.Window window)
    {
        if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window nativeWindow)
        {
            return;
        }

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

        if (hwnd == IntPtr.Zero)
        {
            return;
        }

        var monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
        var monitorInfo = new MonitorInfo
        {
            Size = Marshal.SizeOf<MonitorInfo>()
        };

        if (!GetMonitorInfo(monitor, ref monitorInfo))
        {
            return;
        }

        var scale = GetDpiForWindow(hwnd) / 96.0;
        var workArea = monitorInfo.WorkArea;
        var margin = (int)(24 * scale);
        var width = Math.Min((int)(520 * scale), workArea.Right - workArea.Left - margin);
        var height = Math.Min((int)(820 * scale), workArea.Bottom - workArea.Top - margin);
        var x = workArea.Left + ((workArea.Right - workArea.Left) - width) / 2;
        var y = workArea.Top + ((workArea.Bottom - workArea.Top) - height) / 2;

        SetWindowPos(
            hwnd,
            IntPtr.Zero,
            x,
            y,
            width,
            height,
            SwpNoZOrder | SwpNoActivate);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo monitorInfo);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        IntPtr hwnd,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect WorkArea;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
