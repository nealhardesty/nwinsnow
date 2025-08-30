namespace NWinSnow.Config;

public enum DisplayMode
{
    Wallpaper,
    Screensaver,
    Windowed
}

public sealed class AppConfig
{
    public DisplayConfig Display { get; set; } = new();
    public SnowConfig Snow { get; set; } = new();
    public WindConfig Wind { get; set; } = new();
    public TreesConfig Trees { get; set; } = new();
    public PerformanceConfig Performance { get; set; } = new();
    public GraphicsConfig Graphics { get; set; } = new();
}

public sealed class DisplayConfig
{
    public DisplayMode Mode { get; set; } = DisplayMode.Wallpaper;
    public bool Fullscreen { get; set; } = false;
}

public sealed class SnowConfig
{
    public int Speed { get; set; } = 12;
    public int MaxSnowflakes { get; set; } = 200;
    public float SpawnRate { get; set; } = 0.1f;
}

public sealed class WindConfig
{
    public int Intensity { get; set; } = 5;
    public int ChancePercent { get; set; } = 20;
    public float DurationSeconds { get; set; } = 3f;
}

public sealed class TreesConfig
{
    public int Count { get; set; } = 12;
    public float ScaleMin { get; set; } = 0.8f;
    public float ScaleMax { get; set; } = 1.3f;
}

public sealed class PerformanceConfig
{
    public bool PowerSaveEnabled { get; set; } = false;
    public int TargetFps { get; set; } = 60;
    public bool VSync { get; set; } = true;
    public bool HardwareAcceleration { get; set; } = true;
}

public sealed class GraphicsConfig
{
    public bool AntiAliasing { get; set; } = true;
    public bool TextureFilteringLinear { get; set; } = true;
    public bool ParticleAlphaBlend { get; set; } = true;
}


