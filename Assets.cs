using System.Collections.Immutable;

namespace NWinSnow;

public sealed class Assets : IDisposable
{
    public ImmutableArray<Image> SnowflakeTextures { get; }
    public Image TreeTexture { get; }

    public Assets()
    {
        SnowflakeTextures = LoadSnowflakes();
        TreeTexture = LoadImageFromResource("Assets/tannenbaum.png");
    }

    private static ImmutableArray<Image> LoadSnowflakes()
    {
        var builder = ImmutableArray.CreateBuilder<Image>(7);
        for (int i = 0; i < 7; i++)
        {
            var name = $"Assets/snow0{i}.png";
            builder.Add(LoadImageFromResource(name));
        }
        return builder.ToImmutable();
    }

    private static Image LoadImageFromResource(string logicalName)
    {
        var asm = typeof(Assets).Assembly;
        using var stream = asm.GetManifestResourceStream(logicalName) ?? throw new InvalidOperationException($"Missing resource: {logicalName}");
        return Image.FromStream(stream);
    }

    public void Dispose()
    {
        foreach (var img in SnowflakeTextures)
        {
            img.Dispose();
        }
        TreeTexture.Dispose();
    }
}


