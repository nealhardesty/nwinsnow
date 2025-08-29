namespace NWinSnow.Models;

/// <summary>
/// Christmas tree model with position and scaling information
/// </summary>
public struct Tree
{
    public float X, Y;
    public float Scale;
    public int Width, Height; // Original texture dimensions
    
    /// <summary>
    /// Create a randomly positioned and scaled tree
    /// </summary>
    public static Tree CreateRandom(Random random, int screenWidth, int screenHeight, 
                                   int textureWidth, int textureHeight)
    {
        return new Tree
        {
            X = random.NextSingle() * screenWidth,
            Y = random.NextSingle() * screenHeight,
            Scale = 0.8f + random.NextSingle() * 0.5f, // 0.8x to 1.3x
            Width = textureWidth,
            Height = textureHeight
        };
    }
    
    /// <summary>
    /// Get the actual rendered width of the tree
    /// </summary>
    public readonly float RenderedWidth => Width * Scale;
    
    /// <summary>
    /// Get the actual rendered height of the tree
    /// </summary>
    public readonly float RenderedHeight => Height * Scale;
}
