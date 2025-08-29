using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace NWinSnow.Resources;

/// <summary>
/// Provides access to embedded image resources with proper transparency support
/// </summary>
public static class ResourceManager
{
    private static readonly Assembly ResourceAssembly = typeof(ResourceManager).Assembly;
    private static readonly Dictionary<string, Bitmap> CachedImages = new();

    /// <summary>
    /// Load snowflake texture by index (0-6)
    /// </summary>
    public static Bitmap GetSnowflakeTexture(int index)
    {
        if (index < 0 || index > 6)
            throw new ArgumentOutOfRangeException(nameof(index), "Snowflake index must be between 0 and 6");

        var resourceName = $"NWinSnow.Resources.Images.snow{index:00}.png";
        return LoadPngWithAlpha(resourceName);
    }

    /// <summary>
    /// Load Christmas tree texture
    /// </summary>
    public static Bitmap GetTreeTexture()
    {
        const string resourceName = "NWinSnow.Resources.Images.tannenbaum.png";
        return LoadPngWithAlpha(resourceName);
    }

    /// <summary>
    /// Load all snowflake textures into an array
    /// </summary>
    public static Bitmap[] GetAllSnowflakeTextures()
    {
        var textures = new Bitmap[7];
        for (int i = 0; i < 7; i++)
        {
            textures[i] = GetSnowflakeTexture(i);
        }
        return textures;
    }

    /// <summary>
    /// Load PNG with preserved alpha channel for proper transparency
    /// </summary>
    private static Bitmap LoadPngWithAlpha(string resourceName)
    {
        if (CachedImages.TryGetValue(resourceName, out var cachedBitmap))
            return cachedBitmap;

        using var stream = ResourceAssembly.GetManifestResourceStream(resourceName);
        
        if (stream == null)
            throw new FileNotFoundException($"Resource {resourceName} not found");
        
        // Load PNG with preserved alpha channel
        var bitmap = new Bitmap(stream);
        
        // Ensure 32-bit ARGB format for proper transparency
        if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
        {
            var argbBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(argbBitmap);
            g.DrawImage(bitmap, 0, 0);
            bitmap.Dispose();
            bitmap = argbBitmap;
        }
        
        CachedImages[resourceName] = bitmap;
        return bitmap;
    }

    /// <summary>
    /// Dispose all cached images and clean up resources
    /// </summary>
    public static void Cleanup()
    {
        foreach (var bitmap in CachedImages.Values)
        {
            bitmap.Dispose();
        }
        CachedImages.Clear();
    }
}
