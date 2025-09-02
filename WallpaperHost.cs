using System.Runtime.InteropServices;

namespace NWinSnow;

public sealed class WallpaperHost : IDisposable
{
    private readonly IntPtr _parent;
    private readonly HostWindow _host;
    private readonly Size _size;
    private readonly BufferedGraphicsContext _bufferContext;
    private BufferedGraphics? _buffer;
    public Size SurfaceSize => _size;

    public IntPtr Handle => _host.Handle;

    public WallpaperHost()
    {
        _parent = WallpaperHelper.FindWallpaperTarget();
        if (_parent == IntPtr.Zero)
        {
            throw new InvalidOperationException("Wallpaper parent window not found.");
        }
        var screen = SystemInformation.VirtualScreen;
        _size = new Size(screen.Width, screen.Height);
        _host = new HostWindow(_parent, screen);
        _bufferContext = BufferedGraphicsManager.Current;
        EnsureBuffer();
    }

    public WallpaperHost(IntPtr parent)
    {
        _parent = parent;
        var screen = SystemInformation.VirtualScreen;
        _size = new Size(screen.Width, screen.Height);
        _host = new HostWindow(_parent, screen);
        _bufferContext = BufferedGraphicsManager.Current;
        EnsureBuffer();
    }

    public void Render(Action<Graphics, Size> draw)
    {
        if (_buffer == null)
        {
            EnsureBuffer();
            if (_buffer == null) return;
        }

        var g = _buffer.Graphics;
        g.Clear(Color.Transparent);
        draw(g, _size);
        using var target = Graphics.FromHwnd(_host.Handle);
        _buffer.Render(target);
    }

    private void EnsureBuffer()
    {
        _buffer?.Dispose();
        using var target = Graphics.FromHwnd(_host.Handle);
        _bufferContext.MaximumBuffer = new Size(_size.Width + 1, _size.Height + 1);
        try
        {
            _buffer = _bufferContext.Allocate(target, new Rectangle(0, 0, _size.Width, _size.Height));
        }
        catch
        {
            // If allocation failed due to race, retry once
            _buffer = _bufferContext.Allocate(target, new Rectangle(0, 0, _size.Width, _size.Height));
        }
    }

    public void Dispose()
    {
        _buffer?.Dispose();
        _host.DestroyHandleSafe();
    }

    private sealed class HostWindow : NativeWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_NOSENDCHANGING = 0x0400;
        private const int SW_SHOW = 5;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_STYLE = -16;
        private const int WS_CHILD = unchecked((int)0x40000000);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public HostWindow(IntPtr parent, Rectangle screen)
        {
            var cp = new CreateParams
            {
                Caption = string.Empty,
                ClassName = "STATIC",
                Style = WS_CHILD,
                ExStyle = WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE,
                Parent = parent,
                X = screen.Left,
                Y = screen.Top,
                Width = screen.Width,
                Height = screen.Height
            };
            CreateHandle(cp);
            if (Handle != IntPtr.Zero)
            {
                SetParent(Handle, parent);
                // Ensure correct styles
                var ex = GetWindowLong(Handle, GWL_EXSTYLE);
                SetWindowLong(Handle, GWL_EXSTYLE, ex | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);
                var st = GetWindowLong(Handle, GWL_STYLE);
                SetWindowLong(Handle, GWL_STYLE, st | WS_CHILD);
                SetWindowPos(Handle, IntPtr.Zero, screen.Left, screen.Top, screen.Width, screen.Height, SWP_NOZORDER | SWP_NOACTIVATE | SWP_NOSENDCHANGING);
                ShowWindow(Handle, SW_SHOW);
            }
        }

        public void DestroyHandleSafe()
        {
            try { DestroyHandle(); } catch { }
        }
    }
}


