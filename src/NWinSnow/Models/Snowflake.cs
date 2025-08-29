namespace NWinSnow.Models;

/// <summary>
/// High-performance struct for snowflakes (value type for better cache locality)
/// </summary>
public struct Snowflake
{
    public float X;
    public float Y;
    public float Speed;
    public float Size;
    public byte TextureIndex;  // 0-6 for snow textures
    public float WindEffect;
    public bool Active;
    
    /// <summary>
    /// Reset snowflake to initial spawn state at top of screen
    /// </summary>
    public void Reset(ref Random random, int screenWidth)
    {
        X = random.NextSingle() * screenWidth;
        Y = -20f;
        Size = 0.5f + random.NextSingle() * 0.5f; // 0.5x to 1.0x size
        Speed = CalculateRandomSpeed(ref random);
        TextureIndex = (byte)random.Next(0, 7);
        WindEffect = random.NextSingle() * 0.5f; // Individual wind susceptibility
        Active = true;
    }
    
    /// <summary>
    /// Calculate randomized speed with bias toward slower speeds
    /// </summary>
    private static float CalculateRandomSpeed(ref Random random)
    {
        // Base speed range with randomization
        const float baseSpeed = 12f;
        var maxSpeed = baseSpeed + 3.0f;
        var minSpeed = Math.Max(maxSpeed * 0.8f, 3.0f);
        var randomValue = random.NextSingle();
        var skewed = MathF.Sqrt(randomValue); // Bias toward slower speeds
        return minSpeed + skewed * (maxSpeed - minSpeed);
    }
}
