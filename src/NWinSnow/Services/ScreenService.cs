using System.Drawing;
using NWinSnow.Native;

namespace NWinSnow.Services;

/// <summary>
/// Service for managing screen and monitor information
/// </summary>
public class ScreenService
{
    /// <summary>
    /// Get primary screen dimensions
    /// </summary>
    public Size GetPrimaryScreenSize()
    {
        var desktopWindow = User32.GetDesktopWindow();
        if (User32.GetWindowRect(desktopWindow, out var rect))
        {
            return new Size(rect.Width, rect.Height);
        }
        
        // Fallback to common resolution
        return new Size(1920, 1080);
    }

    /// <summary>
    /// Get virtual screen dimensions (all monitors combined)
    /// </summary>
    public Rectangle GetVirtualScreenBounds()
    {
        var primarySize = GetPrimaryScreenSize();
        
        // For now, return primary screen bounds
        // Future enhancement: enumerate all monitors
        return new Rectangle(0, 0, primarySize.Width, primarySize.Height);
    }

    /// <summary>
    /// Get desktop work area (excluding taskbar)
    /// </summary>
    public Rectangle GetDesktopWorkArea()
    {
        // For wallpaper mode, we want the full screen including taskbar area
        var screenSize = GetPrimaryScreenSize();
        return new Rectangle(0, 0, screenSize.Width, screenSize.Height);
    }

    /// <summary>
    /// Check if the system supports multiple monitors
    /// </summary>
    public bool IsMultiMonitorSystem()
    {
        // Future enhancement: implement multi-monitor detection
        return false;
    }

    /// <summary>
    /// Get screen DPI scaling factor
    /// </summary>
    public float GetDpiScaling()
    {
        // Default to 100% scaling
        // Future enhancement: implement DPI awareness
        return 1.0f;
    }

    /// <summary>
    /// Get recommended window size for windowed mode
    /// </summary>
    public Size GetRecommendedWindowSize()
    {
        var screenSize = GetPrimaryScreenSize();
        
        // Use 80% of screen size, with minimum and maximum constraints
        var width = Math.Max(800, Math.Min(1600, (int)(screenSize.Width * 0.8)));
        var height = Math.Max(600, Math.Min(1200, (int)(screenSize.Height * 0.8)));
        
        return new Size(width, height);
    }

    /// <summary>
    /// Get centered window position for windowed mode
    /// </summary>
    public Point GetCenteredWindowPosition(Size windowSize)
    {
        var screenSize = GetPrimaryScreenSize();
        
        var x = Math.Max(0, (screenSize.Width - windowSize.Width) / 2);
        var y = Math.Max(0, (screenSize.Height - windowSize.Height) / 2);
        
        return new Point(x, y);
    }

    /// <summary>
    /// Check if a position is within screen bounds
    /// </summary>
    public bool IsPositionOnScreen(Point position)
    {
        var screenBounds = GetVirtualScreenBounds();
        return screenBounds.Contains(position);
    }

    /// <summary>
    /// Clamp a rectangle to screen bounds
    /// </summary>
    public Rectangle ClampToScreen(Rectangle rectangle)
    {
        var screenBounds = GetVirtualScreenBounds();
        
        var x = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - rectangle.Width, rectangle.X));
        var y = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - rectangle.Height, rectangle.Y));
        var width = Math.Min(rectangle.Width, screenBounds.Width);
        var height = Math.Min(rectangle.Height, screenBounds.Height);
        
        return new Rectangle(x, y, width, height);
    }

    /// <summary>
    /// Get screen information for display
    /// </summary>
    public ScreenInfo GetScreenInfo()
    {
        var primarySize = GetPrimaryScreenSize();
        var workArea = GetDesktopWorkArea();
        var dpiScaling = GetDpiScaling();
        
        return new ScreenInfo
        {
            PrimaryScreenSize = primarySize,
            VirtualScreenBounds = GetVirtualScreenBounds(),
            WorkArea = workArea,
            DpiScaling = dpiScaling,
            IsMultiMonitor = IsMultiMonitorSystem(),
            RecommendedWindowSize = GetRecommendedWindowSize()
        };
    }
}

/// <summary>
/// Screen information data structure
/// </summary>
public record ScreenInfo
{
    public Size PrimaryScreenSize { get; init; }
    public Rectangle VirtualScreenBounds { get; init; }
    public Rectangle WorkArea { get; init; }
    public float DpiScaling { get; init; }
    public bool IsMultiMonitor { get; init; }
    public Size RecommendedWindowSize { get; init; }
    
    /// <summary>
    /// Get display string for screen information
    /// </summary>
    public override string ToString()
    {
        return $"Primary: {PrimaryScreenSize.Width}x{PrimaryScreenSize.Height}, " +
               $"DPI: {DpiScaling:P0}, " +
               $"Multi-monitor: {IsMultiMonitor}";
    }
}
