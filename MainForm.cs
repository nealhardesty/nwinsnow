using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace NWinSnow;

public partial class MainForm : Form
{
    private readonly Config.AppConfig _config;
    private readonly Renderer _renderer;

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

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_config.Display.Mode == Config.DisplayMode.Wallpaper)
        {
            BeginInvoke(new Action(() =>
            {
                var target = WallpaperHelper.FindWallpaperTarget();
                if (target != IntPtr.Zero)
                {
                    try
                    {
                        var exStyle = WallpaperHelperPrivate.GetWindowLong(Handle, -20);
                        WallpaperHelperPrivate.SetParent(Handle, target);
                        WallpaperHelperPrivate.SetWindowLong(Handle, -20, exStyle | 0x00000080); // WS_EX_TOOLWINDOW
                        FormBorderStyle = FormBorderStyle.None;
                        var screen = SystemInformation.VirtualScreen;
                        WallpaperHelperPrivate.SetWindowPos(Handle, IntPtr.Zero, screen.Left, screen.Top, screen.Width, screen.Height, 0x0004 | 0x0010 | 0x0400);
                    }
                    catch { }
                }
            }));
        }

        _renderer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        _renderer.Render(e.Graphics, ClientSize);
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

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (_config.Display.Mode == Config.DisplayMode.Wallpaper)
        {
            try
            {
                const int WS_EX_APPWINDOW = 0x00040000;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                // No-op: styles set during attach
            }
            catch { }
        }
    }

    private sealed class Renderer : IDisposable
    {
        private readonly Config.AppConfig _config;
        private readonly Control _control;
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Assets _assets;
        private readonly SnowSystem _snow;
        private readonly WindSystem _wind;
        private readonly TreesSystem _trees;

        private DateTime _lastTick;
        private bool _powerSaveActive;

        public Renderer(Config.AppConfig config, Control control)
        {
            _config = config;
            _control = control;
            _assets = new Assets();
            _snow = new SnowSystem(config, _assets);
            _wind = new WindSystem(config);
            _trees = new TreesSystem(config, _assets);

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000 / Math.Max(10, Math.Min(120, config.Performance.TargetFps));
            _timer.Tick += OnTick;
            _lastTick = DateTime.UtcNow;
        }

        public void Start() => _timer.Start();

        private void OnTick(object? sender, EventArgs e)
        {
            UpdatePowerSave();
            var now = DateTime.UtcNow;
            var dt = (float)(now - _lastTick).TotalSeconds;
            _lastTick = now;

            _wind.Update(dt);
            _snow.Update(dt, _control.ClientSize, _wind.CurrentHorizontalForce);
            _control.Invalidate();
        }

        public void Render(Graphics g, Size clientSize)
        {
            g.SmoothingMode = _config.Graphics.AntiAliasing ? SmoothingMode.AntiAlias : SmoothingMode.None;
            g.InterpolationMode = _config.Graphics.TextureFilteringLinear ? InterpolationMode.HighQualityBilinear : InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            _trees.EnsurePlaced(clientSize);
            _trees.Draw(g);
            _snow.Draw(g);
        }

        private void UpdatePowerSave()
        {
            var ps = SystemInformation.PowerStatus;
            var onBattery = ps.PowerLineStatus == PowerLineStatus.Offline;
            var lowBattery = ps.BatteryLifePercent >= 0 && ps.BatteryLifePercent <= 0.2f;
            var shouldSave = _config.Performance.PowerSaveEnabled && onBattery && lowBattery;
            if (shouldSave == _powerSaveActive) return;
            _powerSaveActive = shouldSave;
            if (shouldSave)
            {
                _timer.Interval = 1000 / 20;
                _snow.ApplyPowerSave(true);
            }
            else
            {
                _timer.Interval = 1000 / Math.Max(10, Math.Min(120, _config.Performance.TargetFps));
                _snow.ApplyPowerSave(false);
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
            _assets.Dispose();
        }
    }
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


