using System.Drawing;
using System.Drawing.Drawing2D;
using NWinSnow.Native;

namespace NWinSnow.Graphics;

/// <summary>
/// Direct GDI+ wrapper for maximum performance graphics operations
/// </summary>
public static class GraphicsEngine
{
    /// <summary>
    /// Create a high-performance graphics context for a window
    /// </summary>
    public static System.Drawing.Graphics CreateWindowGraphics(IntPtr windowHandle)
    {
        var hdc = User32.GetDC(windowHandle);
        return System.Drawing.Graphics.FromHdc(hdc);
    }

    /// <summary>
    /// Create a memory graphics context for double buffering
    /// </summary>
    public static (System.Drawing.Graphics graphics, IntPtr memDC, IntPtr bitmap) CreateMemoryGraphics(IntPtr windowHandle, int width, int height)
    {
        var windowDC = User32.GetDC(windowHandle);
        var memDC = Gdi32.CreateCompatibleDC(windowDC);
        var bitmap = Gdi32.CreateCompatibleBitmap(windowDC, width, height);
        
        Gdi32.SelectObject(memDC, bitmap);
        User32.ReleaseDC(windowHandle, windowDC);
        
        var graphics = System.Drawing.Graphics.FromHdc(memDC);
        return (graphics, memDC, bitmap);
    }

    /// <summary>
    /// Optimize graphics context for high-speed rendering
    /// </summary>
    public static void OptimizeForSpeed(System.Drawing.Graphics graphics)
    {
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
        graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
    }

    /// <summary>
    /// Optimize graphics context for high-quality rendering
    /// </summary>
    public static void OptimizeForQuality(System.Drawing.Graphics graphics)
    {
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
    }

    /// <summary>
    /// Fast bitmap copy using GDI BitBlt
    /// </summary>
    public static void FastBitBlt(IntPtr destDC, IntPtr srcDC, int width, int height)
    {
        Gdi32.BitBlt(destDC, 0, 0, width, height, srcDC, 0, 0, Gdi32.SRCCOPY);
    }

    /// <summary>
    /// Alpha blend bitmaps with transparency support
    /// </summary>
    public static void AlphaBlend(IntPtr destDC, IntPtr srcDC, Rectangle destRect, Rectangle srcRect, byte alpha = 255)
    {
        var blendFunc = new BlendFunction
        {
            BlendOp = 0,        // AC_SRC_OVER
            BlendFlags = 0,
            SourceConstantAlpha = alpha,
            AlphaFormat = 1     // AC_SRC_ALPHA
        };

        Gdi32.AlphaBlend(
            destDC,
            destRect.X, destRect.Y, destRect.Width, destRect.Height,
            srcDC,
            srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
            blendFunc);
    }

    /// <summary>
    /// Clear a graphics context with a solid color
    /// </summary>
    public static void ClearWithColor(System.Drawing.Graphics graphics, Color color, int width, int height)
    {
        using var brush = new SolidBrush(color);
        graphics.FillRectangle(brush, 0, 0, width, height);
    }

    /// <summary>
    /// Measure text size for UI elements
    /// </summary>
    public static SizeF MeasureText(System.Drawing.Graphics graphics, string text, Font font)
    {
        return graphics.MeasureString(text, font);
    }

    /// <summary>
    /// Draw text with shadow effect
    /// </summary>
    public static void DrawTextWithShadow(System.Drawing.Graphics graphics, string text, Font font, Brush textBrush, Brush shadowBrush, PointF location, PointF shadowOffset)
    {
        // Draw shadow first
        graphics.DrawString(text, font, shadowBrush, location.X + shadowOffset.X, location.Y + shadowOffset.Y);
        
        // Draw main text
        graphics.DrawString(text, font, textBrush, location);
    }
}
