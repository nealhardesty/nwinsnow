using NWinSnow.Models;

namespace NWinSnow.Physics;

/// <summary>
/// High-performance object pool for snowflakes to avoid garbage collection
/// </summary>
public class SnowflakePool
{
    private readonly Snowflake[] _snowflakes;
    private readonly bool[] _active;
    private int _nextIndex;
    private readonly Random _random = new();
    
    /// <summary>
    /// Initialize snowflake pool with specified capacity
    /// </summary>
    public SnowflakePool(int maxCount)
    {
        _snowflakes = new Snowflake[maxCount];
        _active = new bool[maxCount];
        _nextIndex = 0;
        
        // Initialize all snowflakes as inactive
        for (int i = 0; i < maxCount; i++)
        {
            _active[i] = false;
            _snowflakes[i].Active = false;
        }
    }
    
    /// <summary>
    /// Attempt to spawn new snowflakes based on spawn rate
    /// </summary>
    public void SpawnSnowflakes(float spawnChance, float deltaTime, int screenWidth, float baseSpeed)
    {
        if (_random.NextSingle() < spawnChance * deltaTime)
        {
            // Find inactive snowflake slot
            for (int i = 0; i < _snowflakes.Length; i++)
            {
                int index = (_nextIndex + i) % _snowflakes.Length;
                if (!_active[index])
                {
                    SnowPhysics.ResetSnowflake(ref _snowflakes[index], screenWidth, baseSpeed);
                    _active[index] = true;
                    _nextIndex = (index + 1) % _snowflakes.Length;
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Get span of all snowflakes for efficient processing
    /// </summary>
    public ReadOnlySpan<Snowflake> GetActiveSnowflakes()
    {
        return _snowflakes.AsSpan();
    }
    
    /// <summary>
    /// Get mutable span for physics updates
    /// </summary>
    public Span<Snowflake> GetMutableSnowflakes()
    {
        return _snowflakes.AsSpan();
    }
    
    /// <summary>
    /// Mark a snowflake as inactive
    /// </summary>
    public void DeactivateSnowflake(int index)
    {
        if (index >= 0 && index < _snowflakes.Length)
        {
            _active[index] = false;
            _snowflakes[index].Active = false;
        }
    }
    
    /// <summary>
    /// Get count of currently active snowflakes
    /// </summary>
    public int GetActiveCount()
    {
        int count = 0;
        for (int i = 0; i < _active.Length; i++)
        {
            if (_active[i]) count++;
        }
        return count;
    }
    
    /// <summary>
    /// Reset all snowflakes to inactive state
    /// </summary>
    public void Reset()
    {
        for (int i = 0; i < _snowflakes.Length; i++)
        {
            _active[i] = false;
            _snowflakes[i].Active = false;
        }
        _nextIndex = 0;
    }
}
