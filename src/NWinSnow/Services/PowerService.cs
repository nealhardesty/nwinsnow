using NWinSnow.Native;

namespace NWinSnow.Services;

/// <summary>
/// Service for monitoring system power status and battery life
/// </summary>
public class PowerService
{
    private SYSTEM_POWER_STATUS _lastPowerStatus;
    private DateTime _lastCheck = DateTime.MinValue;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30); // Check every 30 seconds

    /// <summary>
    /// Check if system is currently running on battery power
    /// </summary>
    public bool IsOnBattery
    {
        get
        {
            UpdatePowerStatus();
            return _lastPowerStatus.IsOnBattery;
        }
    }

    /// <summary>
    /// Check if battery is low (less than 20%)
    /// </summary>
    public bool IsBatteryLow
    {
        get
        {
            UpdatePowerStatus();
            return _lastPowerStatus.IsBatteryLow;
        }
    }

    /// <summary>
    /// Get current battery percentage (0-100)
    /// </summary>
    public int BatteryPercentage
    {
        get
        {
            UpdatePowerStatus();
            return _lastPowerStatus.BatteryLifePercent == 255 ? 100 : _lastPowerStatus.BatteryLifePercent;
        }
    }

    /// <summary>
    /// Get AC line status (true if plugged in)
    /// </summary>
    public bool IsPluggedIn
    {
        get
        {
            UpdatePowerStatus();
            return _lastPowerStatus.ACLineStatus == 1;
        }
    }

    /// <summary>
    /// Determine if power save mode should be enabled based on current power status
    /// </summary>
    public bool ShouldUsePowerSaveMode()
    {
        UpdatePowerStatus();
        
        // Enable power save mode if:
        // 1. Running on battery AND battery is low, OR
        // 2. Battery is critically low (less than 10%) regardless of AC status
        return (_lastPowerStatus.IsOnBattery && _lastPowerStatus.IsBatteryLow) ||
               _lastPowerStatus.BatteryLifePercent < 10;
    }

    /// <summary>
    /// Get recommended frame rate based on power status
    /// </summary>
    public int GetRecommendedFrameRate(int defaultFrameRate)
    {
        if (ShouldUsePowerSaveMode())
        {
            return Math.Min(defaultFrameRate, 20); // Limit to 20 FPS in power save mode
        }
        
        return defaultFrameRate;
    }

    /// <summary>
    /// Get recommended maximum snowflakes based on power status
    /// </summary>
    public int GetRecommendedMaxSnowflakes(int defaultMaxSnowflakes)
    {
        if (ShouldUsePowerSaveMode())
        {
            return Math.Min(defaultMaxSnowflakes, 100); // Limit to 100 snowflakes in power save mode
        }
        
        return defaultMaxSnowflakes;
    }

    /// <summary>
    /// Update power status if enough time has passed since last check
    /// </summary>
    private void UpdatePowerStatus()
    {
        var now = DateTime.Now;
        if (now - _lastCheck < _checkInterval)
            return;

        if (Kernel32.GetSystemPowerStatus(out var powerStatus))
        {
            _lastPowerStatus = powerStatus;
            _lastCheck = now;
        }
    }

    /// <summary>
    /// Force immediate power status update
    /// </summary>
    public void RefreshPowerStatus()
    {
        _lastCheck = DateTime.MinValue;
        UpdatePowerStatus();
    }

    /// <summary>
    /// Get detailed power status information
    /// </summary>
    public PowerStatusInfo GetDetailedPowerStatus()
    {
        UpdatePowerStatus();
        
        return new PowerStatusInfo
        {
            IsOnBattery = _lastPowerStatus.IsOnBattery,
            IsBatteryLow = _lastPowerStatus.IsBatteryLow,
            BatteryPercentage = BatteryPercentage,
            IsPluggedIn = IsPluggedIn,
            ShouldUsePowerSave = ShouldUsePowerSaveMode(),
            BatteryLifeTime = _lastPowerStatus.BatteryLifeTime,
            BatteryFullLifeTime = _lastPowerStatus.BatteryFullLifeTime
        };
    }
}

/// <summary>
/// Detailed power status information
/// </summary>
public record PowerStatusInfo
{
    public bool IsOnBattery { get; init; }
    public bool IsBatteryLow { get; init; }
    public int BatteryPercentage { get; init; }
    public bool IsPluggedIn { get; init; }
    public bool ShouldUsePowerSave { get; init; }
    public uint BatteryLifeTime { get; init; }
    public uint BatteryFullLifeTime { get; init; }
    
    /// <summary>
    /// Get estimated battery life in hours and minutes
    /// </summary>
    public string GetBatteryLifeEstimate()
    {
        if (BatteryLifeTime == 0xFFFFFFFF)
            return "Unknown";
        
        var hours = BatteryLifeTime / 3600;
        var minutes = (BatteryLifeTime % 3600) / 60;
        
        return $"{hours}h {minutes}m";
    }
}
