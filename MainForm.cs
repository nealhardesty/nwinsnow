using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace NWinSnow;

public partial class MainForm : Form
{
    private readonly Config.AppConfig _config;
    private readonly Renderer _renderer;
    private readonly IntPtr _wallpaperParent;

    public MainForm(Config.AppConfig config)
    {
        _config = config;
        InitializeComponent();

        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

        _renderer = new Renderer(_config, this);
        BackColor = Color.Black;

        if (_config.Display.Mode == Config.DisplayMode.Wallpaper)
        {
            ShowInTaskbar = false;
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TopMost = false;
        }
    }

    public MainForm(Config.AppConfig config, IntPtr wallpaperParent) : this(config)
    {
        _wallpaperParent = wallpaperParent;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_config.Display.Mode == Config.DisplayMode.Wallpaper)
        {
            BeginInvoke(new Action(() =>
            {
                try
                {
                    var parent = _wallpaperParent != IntPtr.Zero ? _wallpaperParent : WallpaperHelper.FindWallpaperTarget();
                    if (parent == IntPtr.Zero || !WallpaperHelper.ReparentAndStyleToWallpaper(this.Handle, parent))
                    {
                        // Try broader set of candidates
                        WallpaperHelper.TryAttachToWallpaperCandidates(this.Handle);
                    }
                    var screen = SystemInformation.VirtualScreen;
                    WallpaperHelperPrivate.SetWindowPos(this.Handle, IntPtr.Zero, screen.Left, screen.Top, screen.Width, screen.Height, 0x0004 | 0x0010 | 0x0400);
                    ShowInTaskbar = false;
                    TopMost = false;
                    Opacity = 1;
                }
                catch { }
            }));
        }

        _renderer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_config.Display.Mode == Config.DisplayMode.Wallpaper && _wallpaperHost != null)
        {
            // Draw via drawer to ensure it blits even if host buffering has issues
            if (_wallpaperDrawer != null)
            {
                _wallpaperDrawer.Draw((g, size) => _renderer.Render(g, size));
            }
            else
            {
                _wallpaperHost.Render((g, size) => _renderer.Render(g, size));
            }
        }
        else
        {
            _renderer.Render(e.Graphics, ClientSize);
        }
        base.OnPaint(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _renderer.Dispose();
        base.OnFormClosed(e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            Close();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    // No special handle manipulation; wallpaper uses a separate host window

    private WallpaperHost? _wallpaperHost;
    private WallpaperDrawer? _wallpaperDrawer;
}

internal static class WallpaperHelperPrivate
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}


