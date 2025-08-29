using System.Runtime.InteropServices;

namespace NWinSnow.Native;

/// <summary>
/// P/Invoke declarations for GDI32.dll Windows API functions
/// </summary>
public static class Gdi32
{
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(
        IntPtr hdcDest,
        int nXDest, int nYDest,
        int nWidth, int nHeight,
        IntPtr hdcSrc,
        int nXSrc, int nYSrc,
        uint dwRop);

    [DllImport("gdi32.dll")]
    public static extern bool AlphaBlend(
        IntPtr hdcDest,
        int xoriginDest, int yoriginDest,
        int wDest, int hDest,
        IntPtr hdcSrc,
        int xoriginSrc, int yoriginSrc,
        int wSrc, int hSrc,
        BlendFunction ftn);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateSolidBrush(uint crColor);

    [DllImport("gdi32.dll")]
    public static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, uint dwRop);

    [DllImport("gdi32.dll")]
    public static extern uint SetBkColor(IntPtr hdc, uint crColor);

    [DllImport("gdi32.dll")]
    public static extern int SetBkMode(IntPtr hdc, int iBkMode);

    // Raster operation codes
    public const uint SRCCOPY = 0x00CC0020;
    public const uint SRCPAINT = 0x00EE0086;
    public const uint SRCAND = 0x008800C6;
    public const uint SRCINVERT = 0x00660046;
    public const uint SRCERASE = 0x00440328;
    public const uint NOTSRCCOPY = 0x00330008;
    public const uint NOTSRCERASE = 0x001100A6;
    public const uint MERGECOPY = 0x00C000CA;
    public const uint MERGEPAINT = 0x00BB0226;
    public const uint PATCOPY = 0x00F00021;
    public const uint PATPAINT = 0x00FB0A09;
    public const uint PATINVERT = 0x005A0049;
    public const uint DSTINVERT = 0x00550009;
    public const uint BLACKNESS = 0x00000042;
    public const uint WHITENESS = 0x00FF0062;

    // Background modes
    public const int TRANSPARENT = 1;
    public const int OPAQUE = 2;

    // Color values
    public const uint RGB_BLACK = 0x00000000;
    public const uint RGB_WHITE = 0x00FFFFFF;
    public const uint RGB_TRANSPARENT = 0x00000000;
}

/// <summary>
/// BlendFunction structure for AlphaBlend
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct BlendFunction
{
    public byte BlendOp;
    public byte BlendFlags;
    public byte SourceConstantAlpha;
    public byte AlphaFormat;

    public static BlendFunction Default => new()
    {
        BlendOp = 0,        // AC_SRC_OVER
        BlendFlags = 0,
        SourceConstantAlpha = 255,
        AlphaFormat = 1     // AC_SRC_ALPHA
    };
}
