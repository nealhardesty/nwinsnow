namespace NWinSnow.Configuration;

/// <summary>
/// Display mode enumeration
/// </summary>
public enum DisplayMode
{
    Wallpaper,
    Screensaver,
    Windowed
}

/// <summary>
/// Main application configuration using modern C# record pattern
/// </summary>
public record SnowConfiguration
{
    public DisplayConfiguration Display { get; init; } = new();
    public SnowSettings Snow { get; init; } = new();
    public WindSettings Wind { get; init; } = new();
    public TreeSettings Trees { get; init; } = new();
    public PerformanceSettings Performance { get; init; } = new();
    public GraphicsSettings Graphics { get; init; } = new();
}

/// <summary>
/// Display configuration settings
/// </summary>
public record DisplayConfiguration
{
    public DisplayMode Mode { get; init; } = DisplayMode.Wallpaper;
    public bool FullscreenMode { get; init; } = false;
}

/// <summary>
/// Snow physics and spawning settings
/// </summary>
public record SnowSettings
{
    public float Speed { get; init; } = 12f;
    public int MaxSnowflakes { get; init; } = 200;
    public float SpawnRate { get; init; } = 0.1f;
}

/// <summary>
/// Wind system configuration
/// </summary>
public record WindSettings
{
    public float Intensity { get; init; } = 5f;
    public float Chance { get; init; } = 20f;
    public float StormDuration { get; init; } = 3.0f;
}

/// <summary>
/// Christmas tree settings
/// </summary>
public record TreeSettings
{
    public int Count { get; init; } = 12;
    public float MinScale { get; init; } = 0.8f;
    public float MaxScale { get; init; } = 1.3f;
}

/// <summary>
/// Performance and rendering settings
/// </summary>
public record PerformanceSettings
{
    public bool PowerSaveMode { get; init; } = false;
    public int TargetFrameRate { get; init; } = 60;
    public bool VSync { get; init; } = true;
    public bool HardwareAcceleration { get; init; } = true;
}

/// <summary>
/// Graphics quality settings
/// </summary>
public record GraphicsSettings
{
    public bool EnableAntiAliasing { get; init; } = true;
    public string TextureFiltering { get; init; } = "Linear";
    public string ParticleBlending { get; init; } = "Alpha";
}

/// <summary>
/// Runtime configuration that can be modified during execution
/// </summary>
public class RuntimeConfig
{
    public SnowConfiguration Snow { get; set; } = new();
    public bool IsRunning { get; set; } = true;
    public bool IsPaused { get; set; } = false;
    public DateTime StartTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Apply power save mode adjustments
    /// </summary>
    public void ApplyPowerSaveMode()
    {
        Snow = Snow with 
        { 
            Snow = Snow.Snow with 
            { 
                MaxSnowflakes = Math.Min(Snow.Snow.MaxSnowflakes, 100),
                SpawnRate = Snow.Snow.SpawnRate * 0.3f 
            },
            Performance = Snow.Performance with 
            { 
                TargetFrameRate = 20,
                PowerSaveMode = true 
            }
        };
    }
    
    /// <summary>
    /// Validate configuration values and apply constraints
    /// </summary>
    public void ValidateAndConstrain()
    {
        Snow = Snow with
        {
            Snow = Snow.Snow with
            {
                Speed = Math.Clamp(Snow.Snow.Speed, 1f, 40f),
                MaxSnowflakes = Math.Clamp(Snow.Snow.MaxSnowflakes, 50, 400),
                SpawnRate = Math.Clamp(Snow.Snow.SpawnRate, 0.01f, 1.0f)
            },
            Wind = Snow.Wind with
            {
                Intensity = Math.Clamp(Snow.Wind.Intensity, 1f, 60f),
                Chance = Math.Clamp(Snow.Wind.Chance, 0f, 100f)
            },
            Trees = Snow.Trees with
            {
                Count = Math.Clamp(Snow.Trees.Count, 0, 36),
                MinScale = Math.Clamp(Snow.Trees.MinScale, 0.1f, 2.0f),
                MaxScale = Math.Clamp(Snow.Trees.MaxScale, 0.1f, 2.0f)
            },
            Performance = Snow.Performance with
            {
                TargetFrameRate = Math.Clamp(Snow.Performance.TargetFrameRate, 10, 120)
            }
        };
        
        // Ensure MinScale <= MaxScale
        if (Snow.Trees.MinScale > Snow.Trees.MaxScale)
        {
            Snow = Snow with
            {
                Trees = Snow.Trees with
                {
                    MaxScale = Snow.Trees.MinScale
                }
            };
        }
    }
}
