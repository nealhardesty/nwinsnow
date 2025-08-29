using NWinSnow.Models;

namespace NWinSnow.Physics;

/// <summary>
/// High-performance snow physics calculations with no allocations
/// </summary>
public static class SnowPhysics
{
    private static Random s_random = new();
    
    /// <summary>
    /// Calculate randomized speed with bias toward slower speeds
    /// </summary>
    public static float CalculateSpeed(float userSetting)
    {
        var maxSpeed = userSetting + 3.0f;
        var minSpeed = Math.Max(maxSpeed * 0.8f, 3.0f);
        var randomValue = s_random.NextSingle();
        var skewed = MathF.Sqrt(randomValue); // Bias toward slower speeds
        return minSpeed + skewed * (maxSpeed - minSpeed);
    }

    /// <summary>
    /// Update all snowflakes with physics calculations
    /// </summary>
    public static void UpdateSnowflakes(Span<Snowflake> snowflakes, float deltaTime, 
                                       int screenWidth, int screenHeight, float windEffect)
    {
        for (int i = 0; i < snowflakes.Length; i++)
        {
            ref var snowflake = ref snowflakes[i];
            if (!snowflake.Active) continue;
            
            // Apply gravity (vertical movement)
            snowflake.Y += snowflake.Speed * deltaTime;
            
            // Apply wind effect (horizontal movement)
            var totalWindEffect = (snowflake.WindEffect + windEffect) * deltaTime;
            snowflake.X += totalWindEffect;
            
            // Wrap horizontally
            if (snowflake.X < -20f) 
                snowflake.X = screenWidth + 20f;
            else if (snowflake.X > screenWidth + 20f) 
                snowflake.X = -20f;
            
            // Reset if off bottom
            if (snowflake.Y > screenHeight)
            {
                snowflake.Reset(ref s_random, screenWidth);
            }
        }
    }
    
    /// <summary>
    /// Apply wind effect to all active snowflakes
    /// </summary>
    public static void ApplyWindEffect(Span<Snowflake> snowflakes, float windIntensity, float deltaTime)
    {
        for (int i = 0; i < snowflakes.Length; i++)
        {
            ref var snowflake = ref snowflakes[i];
            if (!snowflake.Active) continue;
            
            // Each snowflake responds differently to wind based on its WindEffect property
            var effectiveWind = windIntensity * snowflake.WindEffect;
            snowflake.X += effectiveWind * deltaTime;
        }
    }
    
    /// <summary>
    /// Reset snowflake to spawn position with random properties
    /// </summary>
    public static void ResetSnowflake(ref Snowflake snowflake, int screenWidth, float baseSpeed)
    {
        snowflake.X = s_random.NextSingle() * screenWidth;
        snowflake.Y = -20f;
        snowflake.Size = 0.5f + s_random.NextSingle() * 0.5f;
        snowflake.Speed = CalculateSpeed(baseSpeed);
        snowflake.TextureIndex = (byte)s_random.Next(0, 7);
        snowflake.WindEffect = s_random.NextSingle() * 0.5f;
        snowflake.Active = true;
    }
}
