using NWinSnow.Native;

namespace NWinSnow.Services;

/// <summary>
/// Service for integrating with Windows desktop wallpaper system
/// </summary>
public class WallpaperService
{
    private IntPtr _desktopWindow;
    private IntPtr _wallpaperWindow;
    
    /// <summary>
    /// Initialize wallpaper service
    /// </summary>
    public WallpaperService()
    {
        _desktopWindow = User32.GetDesktopWindow();
        FindWallpaperWindow();
    }

    /// <summary>
    /// Find the wallpaper window (for Windows 10/11 compatibility)
    /// </summary>
    private void FindWallpaperWindow()
    {
        // Try to find WorkerW window that contains the wallpaper
        _wallpaperWindow = User32.FindWindow("WorkerW", null!);
        
        if (_wallpaperWindow == IntPtr.Zero)
        {
            // Fallback to desktop window
            _wallpaperWindow = _desktopWindow;
        }
    }

    /// <summary>
    /// Position window behind desktop icons (wallpaper mode)
    /// </summary>
    public bool SetWindowAsBehindDesktop(IntPtr windowHandle)
    {
        try
        {
            // First, make the window a child of the desktop
            // This ensures it appears behind desktop icons
            var result = User32.SetWindowPos(
                windowHandle,
                User32.HWND_BOTTOM,
                0, 0, 0, 0,
                User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOACTIVATE);
            
            if (!result)
                return false;

            // Configure window style for wallpaper mode
            return ConfigureWallpaperStyle(windowHandle);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Configure window style for wallpaper integration
    /// </summary>
    private bool ConfigureWallpaperStyle(IntPtr windowHandle)
    {
        // Set layered window attributes for transparency
        var result = User32.SetLayeredWindowAttributes(
            windowHandle,
            0, // No color key
            255, // Full opacity
            User32.LWA_ALPHA);

        return result;
    }

    /// <summary>
    /// Check if wallpaper mode is properly configured
    /// </summary>
    public bool IsWallpaperModeActive(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return false;

        // Check if window is positioned correctly
        User32.GetWindowRect(windowHandle, out var windowRect);
        User32.GetWindowRect(_desktopWindow, out var desktopRect);

        // Window should cover the desktop area
        return windowRect.Left <= desktopRect.Left &&
               windowRect.Top <= desktopRect.Top &&
               windowRect.Right >= desktopRect.Right &&
               windowRect.Bottom >= desktopRect.Bottom;
    }

    /// <summary>
    /// Refresh wallpaper positioning (call when screen resolution changes)
    /// </summary>
    public bool RefreshWallpaperPosition(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return false;

        // Get current desktop dimensions
        User32.GetWindowRect(_desktopWindow, out var desktopRect);

        // Reposition and resize window to match desktop
        return User32.SetWindowPos(
            windowHandle,
            User32.HWND_BOTTOM,
            desktopRect.Left, desktopRect.Top,
            desktopRect.Width, desktopRect.Height,
            User32.SWP_NOACTIVATE);
    }

    /// <summary>
    /// Get desktop window handle
    /// </summary>
    public IntPtr GetDesktopWindow() => _desktopWindow;

    /// <summary>
    /// Get wallpaper window handle
    /// </summary>
    public IntPtr GetWallpaperWindow() => _wallpaperWindow;

    /// <summary>
    /// Check if the system supports wallpaper mode
    /// </summary>
    public bool IsWallpaperModeSupported()
    {
        return _desktopWindow != IntPtr.Zero;
    }

    /// <summary>
    /// Configure window for optimal wallpaper performance
    /// </summary>
    public WallpaperConfiguration GetOptimalConfiguration()
    {
        User32.GetWindowRect(_desktopWindow, out var desktopRect);
        
        return new WallpaperConfiguration
        {
            Position = new System.Drawing.Point(desktopRect.Left, desktopRect.Top),
            Size = new System.Drawing.Size(desktopRect.Width, desktopRect.Height),
            IsSupported = IsWallpaperModeSupported(),
            DesktopHandle = _desktopWindow,
            WallpaperHandle = _wallpaperWindow
        };
    }

    /// <summary>
    /// Restore window from wallpaper mode
    /// </summary>
    public bool RestoreFromWallpaperMode(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return false;

        // Remove from bottom and make normal window
        return User32.SetWindowPos(
            windowHandle,
            IntPtr.Zero,
            0, 0, 0, 0,
            User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOACTIVATE);
    }
}

/// <summary>
/// Wallpaper configuration information
/// </summary>
public record WallpaperConfiguration
{
    public System.Drawing.Point Position { get; init; }
    public System.Drawing.Size Size { get; init; }
    public bool IsSupported { get; init; }
    public IntPtr DesktopHandle { get; init; }
    public IntPtr WallpaperHandle { get; init; }
    
    /// <summary>
    /// Get display string for wallpaper configuration
    /// </summary>
    public override string ToString()
    {
        return $"Position: {Position}, Size: {Size}, Supported: {IsSupported}";
    }
}
