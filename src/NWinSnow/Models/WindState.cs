namespace NWinSnow.Models;

/// <summary>
/// Wind storm state enumeration
/// </summary>
public enum WindState : byte 
{ 
    None,       // No wind effects
    PhaseIn,    // Gradually increasing intensity (1 second)
    Active,     // Full storm intensity (3 seconds)
    PhaseOut    // Gradually decreasing intensity (1.5 seconds)
}

/// <summary>
/// Wind system state and configuration
/// </summary>
public struct WindSystem
{
    public WindState State;
    public float StateTimer;
    public float Intensity;
    public int Direction; // -1 or 1
    public float ChancePerFrame;
    public float MaxIntensity;
    
    private static Random s_random = new();
    
    /// <summary>
    /// Initialize wind system with configuration
    /// </summary>
    public WindSystem(float windChance, float maxIntensity)
    {
        State = WindState.None;
        StateTimer = 0f;
        Intensity = 0f;
        Direction = 1;
        ChancePerFrame = windChance / 100f; // Convert percentage to decimal
        MaxIntensity = maxIntensity;
    }
    
    /// <summary>
    /// Update wind system state machine
    /// </summary>
    public void Update(float deltaTime)
    {
        StateTimer += deltaTime;
        
        switch (State)
        {
            case WindState.None:
                if (s_random.NextSingle() < ChancePerFrame * deltaTime)
                {
                    StartStorm();
                }
                break;
                
            case WindState.PhaseIn:
                var phaseInProgress = Math.Min(StateTimer / 1.0f, 1.0f); // 1 second phase-in
                Intensity = MaxIntensity * phaseInProgress;
                if (StateTimer >= 1.0f)
                {
                    State = WindState.Active;
                    StateTimer = 0f;
                }
                break;
                
            case WindState.Active:
                Intensity = MaxIntensity;
                if (StateTimer >= 3.0f) // 3 seconds active
                {
                    State = WindState.PhaseOut;
                    StateTimer = 0f;
                }
                break;
                
            case WindState.PhaseOut:
                var phaseOutProgress = Math.Min(StateTimer / 1.5f, 1.0f); // 1.5 second phase-out
                Intensity = MaxIntensity * (1.0f - phaseOutProgress);
                if (StateTimer >= 1.5f)
                {
                    State = WindState.None;
                    StateTimer = 0f;
                    Intensity = 0f;
                }
                break;
        }
    }
    
    /// <summary>
    /// Start a new wind storm with random direction
    /// </summary>
    private void StartStorm()
    {
        State = WindState.PhaseIn;
        StateTimer = 0f;
        Direction = s_random.Next(0, 2) == 0 ? -1 : 1;
    }
    
    /// <summary>
    /// Get current wind effect for snowflakes
    /// </summary>
    public readonly float GetWindEffect() => Intensity * Direction * 2.0f;
}
