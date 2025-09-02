namespace NWinSnow;

public sealed class WallpaperDrawer : IDisposable
{
    private readonly IntPtr _targetHwnd;
    private readonly Size _size;
    private readonly Bitmap _backBuffer;
    private readonly Graphics _backG;

    public Size SurfaceSize => _size;

    public WallpaperDrawer(IntPtr target)
    {
        _targetHwnd = target;
        var screen = SystemInformation.VirtualScreen;
        _size = new Size(screen.Width, screen.Height);
        _backBuffer = new Bitmap(_size.Width, _size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        _backG = Graphics.FromImage(_backBuffer);
        _backG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        _backG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        _backG.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        _backG.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        _backG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
    }

    public void Draw(Action<Graphics, Size> draw)
    {
        _backG.Clear(Color.Transparent);
        draw(_backG, _size);
        using var targetG = Graphics.FromHwnd(_targetHwnd);
        targetG.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        targetG.DrawImage(_backBuffer, 0, 0, _size.Width, _size.Height);
    }

    public void Dispose()
    {
        _backG.Dispose();
        _backBuffer.Dispose();
    }
}


