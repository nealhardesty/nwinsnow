namespace NWinSnow;

public sealed class Renderer : IDisposable
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
    private WallpaperHost? _wallpaperHost;

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
        var size = _wallpaperHost?.SurfaceSize ?? _control.ClientSize;
        _snow.Update(dt, size, _wind.CurrentHorizontalForce);
        if (_wallpaperHost != null)
        {
            _wallpaperHost.Render((g, size) => Render(g, size));
        }
        else
        {
            _control.Invalidate();
        }
    }

    public void Render(Graphics g, Size clientSize)
    {
        g.SmoothingMode = _config.Graphics.AntiAliasing ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;
        g.InterpolationMode = _config.Graphics.TextureFilteringLinear ? System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

        _trees.EnsurePlaced(clientSize);
        _trees.Draw(g);
        _snow.Draw(g);
    }

    public void ApplyImmediateRender()
    {
        _control.Invalidate();
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

    public void AttachWallpaperHost(WallpaperHost host)
    {
        _wallpaperHost = host;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
        _assets.Dispose();
    }
}


