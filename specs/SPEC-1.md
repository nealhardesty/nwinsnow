# nwinsnow - Windows Snow Simulation Application

## Overview

**nwinsnow** is a C#/.NET application that displays animated falling snow on Windows systems, inspired by the classic `xsnow` application for X11 Unix systems. The application creates a visually appealing snow simulation with Christmas trees, dynamic wind effects, and customizable parameters built using **direct Windows API calls via P/Invoke** for maximum performance, minimal overhead, and true old-school Windows programming.

### Key Features

- **Dual Display Modes**: Background wallpaper or fullscreen screensaver
- **Dynamic Weather System**: Realistic wind storms with smooth transitions
- **Multiple Christmas Trees**: Randomly positioned and scaled decorative elements
- **Seven Snowflake Variations**: Different snowflake designs for visual variety
- **Performance Optimization**: Battery-aware rendering with power save mode
- **Command-Line Configuration**: All settings controllable via CLI arguments

## Project Structure

The solution follows a lean, performance-focused structure using direct WinAPI calls:

```
nwinsnow/
├── src/
│   ├── NWinSnow/                   # Main executable project
│   │   ├── Program.cs              # Entry point with command-line parsing
│   │   ├── SnowWindow.cs           # Main window class with WinAPI calls
│   │   ├── SnowRenderer.cs         # Direct GDI+ rendering engine
│   │   ├── Models/
│   │   │   ├── Snowflake.cs        # Snowflake data structure
│   │   │   ├── Tree.cs             # Christmas tree model
│   │   │   └── WindState.cs        # Wind system state
│   │   ├── Physics/
│   │   │   ├── SnowPhysics.cs      # Physics calculations
│   │   │   └── WindSystem.cs       # Wind storm logic
│   │   ├── Graphics/
│   │   │   ├── GraphicsEngine.cs   # Direct GDI+ wrapper
│   │   │   ├── ImageCache.cs       # Bitmap caching system
│   │   │   └── RenderingUtils.cs   # Drawing optimizations
│   │   ├── Configuration/
│   │   │   ├── AppConfig.cs        # Configuration model
│   │   │   └── CommandLineParser.cs # CLI argument parsing
│   │   ├── Native/                 # P/Invoke declarations
│   │   │   ├── User32.cs           # User32.dll imports
│   │   │   ├── Gdi32.cs            # GDI32.dll imports
│   │   │   ├── Shell32.cs          # Shell32.dll imports
│   │   │   ├── Kernel32.cs         # Kernel32.dll imports
│   │   │   └── WinApiStructs.cs    # Windows API structures
│   │   ├── Services/
│   │   │   ├── WallpaperService.cs # Desktop wallpaper integration
│   │   │   ├── ScreenService.cs    # Multi-monitor support
│   │   │   └── PowerService.cs     # Battery/power management
│   │   └── NWinSnow.csproj
│   └── NWinSnow.Resources/         # Resource assembly
│       ├── Images/
│       │   ├── snow00.png - snow06.png
│       │   └── tannenbaum.png
│       └── NWinSnow.Resources.csproj
├── tests/
│   ├── NWinSnow.Tests/             # Unit tests
│   └── NWinSnow.Benchmarks/        # Performance benchmarks
├── tools/
│   ├── build.cmd                   # Build script
│   └── package.cmd                 # Distribution packaging
├── assets/                         # Original asset files
│   ├── snow00.png - snow06.png
│   └── tannenbaum.png
├── NWinSnow.sln                   # Solution file
├── Directory.Build.props          # MSBuild properties
├── global.json                    # .NET SDK version
└── README.md                      # Documentation
```

### Direct WinAPI Architecture Benefits

- **Maximum Performance**: No framework overhead, direct system calls
- **Minimal Memory Footprint**: No UI framework dependencies
- **True Win32 Integration**: Full access to Windows desktop APIs
- **Hardware Acceleration**: Direct access to GDI+ and DirectX if needed
- **Old-School Simplicity**: Straightforward message loop and window procedures
- **Deployment Flexibility**: Single executable with minimal dependencies

## Display Modes

### 1. Background Wallpaper Mode
- Renders behind desktop icons and windows
- Integrates with Windows desktop wallpaper system
- Lower performance impact for daily use

### 2. Fullscreen Screensaver Mode
- Takes over entire screen display
- Always renders on top of other applications
- ESC key exits the application
- Higher visual impact for presentations or screensaver use

## Configuration Parameters

The application supports multiple configuration sources following .NET best practices:

1. **appsettings.json** - Default configuration values
2. **Command-line arguments** - Runtime overrides
3. **Environment variables** - Deployment-specific settings
4. **User settings** - Persistent user preferences (stored in AppData)

Configuration is managed using the `Microsoft.Extensions.Configuration` package with the Options pattern.

### Configuration Structure

#### appsettings.json Example
```json
{
  "SnowConfiguration": {
    "Display": {
      "Mode": "Wallpaper",
      "FullscreenMode": false
    },
    "Snow": {
      "Speed": 12,
      "MaxSnowflakes": 200,
      "SpawnRate": 0.1
    },
    "Wind": {
      "Intensity": 5,
      "Chance": 20,
      "StormDuration": 3.0
    },
    "Trees": {
      "Count": 12,
      "MinScale": 0.8,
      "MaxScale": 1.3
    },
    "Performance": {
      "PowerSaveMode": false,
      "TargetFrameRate": 60,
      "VSync": true,
      "HardwareAcceleration": true
    },
    "Graphics": {
      "EnableAntiAliasing": true,
      "TextureFiltering": "Linear",
      "ParticleBlending": "Alpha"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "NWinSnow": "Debug"
    }
  }
}
```

### Core Settings

#### Display Mode (`SnowConfiguration:Display:Mode`)
- **Default**: `Wallpaper`
- **Values**: `Wallpaper`, `Screensaver`, `Windowed`
- **CLI**: `--mode <wallpaper|screensaver|windowed>`
- **Description**: Display mode selection

#### Tree Count (`SnowConfiguration:Trees:Count`)
- **Default**: `12`
- **Range**: `0-36`
- **CLI**: `--trees <count>`
- **Description**: Number of Christmas trees displayed
- Trees are randomly positioned with scaling between 0.8x and 1.3x
- Set to 0 for snow-only experience

#### Snow Speed (`SnowConfiguration:Snow:Speed`)
- **Default**: `12`
- **Range**: `1-40`
- **CLI**: `--snow-speed <speed>`
- **Description**: Base falling speed of snowflakes
- Each snowflake gets randomized speed within calculated range
- Higher values create more dramatic snowfall

#### Wind Intensity (`SnowConfiguration:Wind:Intensity`)
- **Default**: `5`
- **Range**: `1-60`
- **CLI**: `--wind-intensity <intensity>`
- **Description**: Strength of wind storms
- Affects horizontal movement during storm phases
- Higher values create more dramatic sideways movement

#### Wind Chance (`SnowConfiguration:Wind:Chance`)
- **Default**: `20`
- **Range**: `0-100`
- **CLI**: `--wind-chance <percentage>`
- **Description**: Frequency of wind storms (percentage)
- Chance per frame that a storm will begin
- 0% disables wind effects, 100% creates constant storms

### Performance Settings

#### Power Save Mode (`SnowConfiguration:Performance:PowerSaveMode`)
- **Default**: `false`
- **CLI**: `--power-save <true|false>`
- **Description**: Enable battery optimization mode
- Reduces frame rate from 60 FPS to 20 FPS
- Halves maximum snowflake count
- Simplifies rendering pipeline

#### Max Snowflakes (`SnowConfiguration:Snow:MaxSnowflakes`)
- **Default**: `200` (normal), `100` (power save)
- **Range**: `50-400`
- **CLI**: `--max-snowflakes <count>`
- **Description**: Maximum number of active snowflakes

#### Target Frame Rate (`SnowConfiguration:Performance:TargetFrameRate`)
- **Default**: `60`
- **CLI**: `--fps <rate>`
- **Description**: Target frames per second for animation

## Technical Implementation

### Architecture Overview

The application follows a direct WinAPI architecture with minimal abstractions for maximum performance:

- **Window Management**: Direct Win32 window creation and message handling
- **Graphics Rendering**: GDI+ for 2D graphics with optional DirectX integration
- **Resource Management**: Manual memory management and object pooling
- **System Integration**: Direct P/Invoke calls to Windows APIs

### Snow Physics Engine

The snow simulation uses a high-resolution timer via `QueryPerformanceCounter` for precise frame timing and direct GDI+ rendering for 60 FPS animation (20 FPS in power save mode). No framework overhead - just raw Windows APIs.

#### Core Physics Implementation
```csharp
// High-performance struct for snowflakes (value type for better cache locality)
public struct Snowflake
{
    public float X;
    public float Y;
    public float Speed;
    public float Size;
    public byte TextureIndex;  // 0-6 for snow textures
    public float WindEffect;
    public bool Active;
    
    public void Reset(ref Random random, int screenWidth)
    {
        X = random.NextSingle() * screenWidth;
        Y = -20f;
        Size = 0.5f + random.NextSingle() * 0.5f;
        TextureIndex = (byte)random.Next(0, 7);
        WindEffect = 0f;
        Active = true;
    }
}

// Direct physics calculations with no allocations
public static class SnowPhysics
{
    private static Random s_random = new();
    
    public static float CalculateSpeed(float userSetting)
    {
        var maxSpeed = userSetting + 3.0f;
        var minSpeed = Math.Max(maxSpeed * 0.8f, 3.0f);
        var randomValue = s_random.NextSingle();
        var skewed = MathF.Sqrt(randomValue); // Bias toward slower speeds
        return minSpeed + skewed * (maxSpeed - minSpeed);
    }

    public static void UpdateSnowflakes(Span<Snowflake> snowflakes, float deltaTime, 
                                       int screenWidth, int screenHeight, float windEffect)
    {
        for (int i = 0; i < snowflakes.Length; i++)
        {
            ref var snowflake = ref snowflakes[i];
            if (!snowflake.Active) continue;
            
            snowflake.Y += snowflake.Speed * deltaTime;
            snowflake.X += (snowflake.WindEffect + windEffect) * deltaTime;
            
            // Wrap horizontally
            if (snowflake.X < -20f) snowflake.X = screenWidth + 20f;
            else if (snowflake.X > screenWidth + 20f) snowflake.X = -20f;
            
            // Reset if off bottom
            if (snowflake.Y > screenHeight)
            {
                snowflake.Reset(ref s_random, screenWidth);
            }
        }
    }
}
```

#### Graphics Engine with Direct GDI+
```csharp
public unsafe class SnowRenderer : IDisposable
{
    private IntPtr _hdc;
    private IntPtr _memDC;
    private IntPtr _bitmap;
    private Graphics _graphics;
    private Bitmap[] _snowTextures;
    private Bitmap _treeTexture;
    
    public SnowRenderer(IntPtr windowHandle, int width, int height)
    {
        // Direct GDI+ setup
        _hdc = User32.GetDC(windowHandle);
        _memDC = Gdi32.CreateCompatibleDC(_hdc);
        _bitmap = Gdi32.CreateCompatibleBitmap(_hdc, width, height);
        Gdi32.SelectObject(_memDC, _bitmap);
        
        _graphics = Graphics.FromHdc(_memDC);
        _graphics.SmoothingMode = SmoothingMode.HighSpeed;
        _graphics.CompositingQuality = CompositingQuality.HighSpeed;
        
        LoadTextures();
    }
    
    public void RenderFrame(ReadOnlySpan<Snowflake> snowflakes, ReadOnlySpan<Tree> trees)
    {
        // Clear with transparent black
        _graphics.Clear(Color.Transparent);
        
        // Draw trees first (back layer)
        foreach (var tree in trees)
        {
            _graphics.DrawImage(_treeTexture, tree.X, tree.Y, 
                               tree.Width * tree.Scale, tree.Height * tree.Scale);
        }
        
        // Draw snowflakes
        for (int i = 0; i < snowflakes.Length; i++)
        {
            ref readonly var flake = ref snowflakes[i];
            if (!flake.Active) continue;
            
            var texture = _snowTextures[flake.TextureIndex];
            var size = 16f * flake.Size; // Base size 16px
            
            _graphics.DrawImage(texture, flake.X - size/2, flake.Y - size/2, size, size);
        }
        
        // Blit to screen
        Gdi32.BitBlt(_hdc, 0, 0, width, height, _memDC, 0, 0, Gdi32.SRCCOPY);
    }
}
```

**Speed Examples:**
- Setting 12 (default): 11.6-15.0 pixels/frame
- Setting 1 (minimum): 3.0-4.0 pixels/frame
- Setting 40 (maximum): 34.4-43.0 pixels/frame

#### Size and Visual Variation
- **Size**: Random multiplier between 0.5x and 1.0x using `Random.NextSingle()`
- **Texture**: Randomly assigned from 7 snowflake images (snow00.png - snow06.png)
- **Transparency**: Full PNG alpha channel support for proper snowflake blending
- **Rendering**: Direct GDI+ with alpha compositing for smooth transparency

### Dynamic Wind System

#### High-Performance Wind System
```csharp
public enum WindState : byte { None, PhaseIn, Active, PhaseOut }

public struct WindSystem
{
    public WindState State;
    public float StateTimer;
    public float Intensity;
    public int Direction; // -1 or 1
    public float ChancePerFrame;
    public float MaxIntensity;
    
    private static Random s_random = new();
    
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
    
    private void StartStorm()
    {
        State = WindState.PhaseIn;
        StateTimer = 0f;
        Direction = s_random.Next(0, 2) == 0 ? -1 : 1;
    }
    
    public float GetWindEffect() => Intensity * Direction * 2.0f;
}
```

#### Storm State Machine
1. **`WindState.None`**: No wind effects
2. **`WindState.PhaseIn`**: Gradually increasing intensity (1 second)
3. **`WindState.Active`**: Full storm intensity (3 seconds)  
4. **`WindState.PhaseOut`**: Gradually decreasing intensity (1.5 seconds)

#### Storm Physics
- **Initiation**: Per-frame probability using `Random.NextSingle()` and configuration
- **Direction**: Randomly left (-1) or right (+1) using `Random.Next()`
- **Intensity**: `windLevel * 2.0f * phaseProgress`
- **Effect**: Horizontal displacement applied to all snowflakes via `WindEffect` property

### Spawning and Recycling

#### Object Pool Management
```csharp
public class SnowflakePool
{
    private readonly Snowflake[] _snowflakes;
    private readonly bool[] _active;
    private int _nextIndex;
    private readonly Random _random = new();
    
    public SnowflakePool(int maxCount)
    {
        _snowflakes = new Snowflake[maxCount];
        _active = new bool[maxCount];
    }
    
    public void SpawnSnowflakes(float spawnChance, float deltaTime, int screenWidth)
    {
        if (_random.NextSingle() < spawnChance * deltaTime)
        {
            // Find inactive snowflake slot
            for (int i = 0; i < _snowflakes.Length; i++)
            {
                int index = (_nextIndex + i) % _snowflakes.Length;
                if (!_active[index])
                {
                    _snowflakes[index].Reset(ref _random, screenWidth);
                    _active[index] = true;
                    _nextIndex = (index + 1) % _snowflakes.Length;
                    break;
                }
            }
        }
    }
    
    public ReadOnlySpan<Snowflake> GetActiveSnowflakes()
    {
        // Return span of currently active snowflakes
        return _snowflakes.AsSpan();
    }
}
```

#### Snowflake Spawning
- **Normal Mode**: 10% chance per frame (~6 per second at 60 FPS)
- **Power Save**: 3% chance per frame (~1.8 per second at 20 FPS)
- **Object Pooling**: Pre-allocated array to avoid garbage collection
- **Transparency**: PNG alpha channel preserved during loading and rendering

#### Boundary Handling
- **Vertical**: Snowflakes recycle from bottom to top of screen
- **Horizontal**: Wrap-around when pushed off-screen by wind
- **Memory Efficient**: Reuse existing snowflake objects via object pool

### Christmas Trees with Transparency Support

```csharp
public struct Tree
{
    public float X, Y;
    public float Scale;
    public int Width, Height; // Original texture dimensions
    
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
}
```

- **Asset**: `assets/tannenbaum.png` with full alpha channel support
- **Positioning**: Random X,Y coordinates across full screen
- **Scaling**: Random scale between 0.8x and 1.3x for variety
- **Count**: User-configurable (0-36 trees)
- **Transparency**: Proper PNG alpha blending for natural tree edges

## Build System

### .NET Project Configuration
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <UseWindowsForms>false</UseWindowsForms>
    <Nullable>enable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <TrimMode>partial</TrimMode>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>false</SelfContained>
    <ApplicationIcon>Resources\icon.ico</ApplicationIcon>
    <AssemblyTitle>NWinSnow - Windows Snow Simulation</AssemblyTitle>
    <Product>NWinSnow</Product>
    <Copyright>© 2025</Copyright>
    <Version>1.0.0</Version>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.CommandLine" Version="2.0.0" />
    <PackageReference Include="System.Drawing.Common" Version="9.0.0" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Resources\Images\*.png" />
    <None Include="Resources\icon.ico" />
  </ItemGroup>
</Project>
```

### Build Scripts
```batch
REM build.cmd - Build script
@echo off
echo Building NWinSnow...
dotnet build -c Release --no-restore
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%
echo Build completed successfully!

REM package.cmd - Create distribution package  
@echo off
echo Creating release package...
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%
echo Package created in bin\Release\net9.0-windows\win-x64\publish\
```

### MSBuild Targets
- **`dotnet build`**: Debug build with full debugging symbols
- **`dotnet build -c Release`**: Optimized release build
- **`dotnet publish`**: Create single-file deployment package
- **`dotnet run`**: Run in wallpaper mode (default)
- **`dotnet run -- --mode screensaver`**: Run in screensaver mode
- **`dotnet clean`**: Clean build artifacts
- **`dotnet test`**: Run unit tests and benchmarks

## Performance Characteristics

### Normal Mode (60 FPS)
- Up to 200 active snowflakes
- Full wind storm effects
- All trees rendered with full scaling
- ~6 new snowflakes spawned per second

### Power Save Mode (20 FPS)
- Up to 100 active snowflakes
- Reduced spawn rate (1.8 per second)
- Simplified rendering pipeline
- No wind storm processing during low battery

## Windows Integration

### Direct P/Invoke API Bindings
```csharp
public static class User32
{
    [DllImport("user32.dll")]
    public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, 
        string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, 
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, 
        byte bAlpha, uint dwFlags);
    
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
        int X, int Y, int cx, int cy, uint uFlags);
    
    public const uint WS_EX_LAYERED = 0x80000;
    public const uint WS_EX_TRANSPARENT = 0x20;
    public const uint WS_EX_TOOLWINDOW = 0x80;
    public const IntPtr HWND_BOTTOM = (IntPtr)1;
}

public static class Gdi32
{
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
    
    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, 
        int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
    
    [DllImport("gdi32.dll")]
    public static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, 
        int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, 
        int wSrc, int hSrc, BlendFunction ftn);
    
    public const uint SRCCOPY = 0xCC0020;
}

public static class Kernel32
{
    [DllImport("kernel32.dll")]
    public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
    
    [DllImport("kernel32.dll")]
    public static extern bool QueryPerformanceFrequency(out long lpFrequency);
    
    [DllImport("kernel32.dll")]
    public static extern uint GetTickCount();
}
```

### PNG Transparency Handling
```csharp
public class TransparentImageLoader
{
    public static Bitmap LoadPngWithAlpha(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(resourceName);
        
        if (stream == null)
            throw new FileNotFoundException($"Resource {resourceName} not found");
        
        // Load PNG with preserved alpha channel
        var bitmap = new Bitmap(stream);
        
        // Ensure 32-bit ARGB format for proper transparency
        if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
        {
            var argbBitmap = new Bitmap(bitmap.Width, bitmap.Height, 
                                       PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(argbBitmap);
            g.DrawImage(bitmap, 0, 0);
            bitmap.Dispose();
            return argbBitmap;
        }
        
        return bitmap;
    }
    
    public static void RenderWithAlpha(Graphics graphics, Bitmap bitmap, 
                                      float x, float y, float width, float height)
    {
        // Use Graphics.DrawImage which respects alpha channel
        var destRect = new RectangleF(x, y, width, height);
        var srcRect = new RectangleF(0, 0, bitmap.Width, bitmap.Height);
        
        graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
    }
}
```

### Window Management
- **Wallpaper Mode**: Create layered window as desktop child with `HWND_BOTTOM`
- **Screensaver Mode**: Fullscreen topmost window with transparency support
- **Alpha Blending**: Full PNG transparency via `SetLayeredWindowAttributes` and `AlphaBlend`
- **Event Handling**: ESC key detection for exit, window message processing
- **Resource Management**: Proper GDI object cleanup and disposal patterns

## Development Guidelines

### Modern C# Standards (2025)
```csharp
// Global using statements (GlobalUsings.cs)
global using System;
global using System.Drawing;
global using System.Drawing.Imaging;
global using System.Runtime.InteropServices;
global using System.Numerics;

// Nullable reference types enabled
#nullable enable

// File-scoped namespace declarations
namespace NWinSnow;

// Primary constructors for data classes
public readonly record struct SnowConfiguration(
    DisplayMode Mode,
    int Trees,
    float SnowSpeed,
    float WindIntensity,
    float WindChance,
    bool PowerSaveMode
);

// Required members for initialization
public class SnowRenderer
{
    public required IntPtr WindowHandle { get; init; }
    public required Size ScreenSize { get; init; }
}

// Pattern matching and switch expressions
public static RenderMode GetRenderMode(DisplayMode mode) => mode switch
{
    DisplayMode.Wallpaper => RenderMode.Background,
    DisplayMode.Screensaver => RenderMode.Fullscreen,
    DisplayMode.Windowed => RenderMode.Window,
    _ => throw new ArgumentOutOfRangeException(nameof(mode))
};
```

### Code Style
- **C# 12/13 Features**: Use latest language features (file-scoped namespaces, global usings, required members)
- **Nullable Reference Types**: Enable nullable context throughout project for null safety
- **EditorConfig**: Use `.editorconfig` for consistent formatting across team
- **Code Analysis**: Enable all CA rules and treat warnings as errors
- **Documentation**: Use XML documentation comments for public APIs

### Performance Considerations
- **Zero Allocations**: Minimize heap allocations in render loop using `Span<T>`, structs, and object pooling
- **SIMD Operations**: Use `System.Numerics.Vector` for bulk operations where applicable
- **Stack Allocation**: Use `stackalloc` for small temporary arrays
- **Unsafe Code**: Strategic use of `unsafe` blocks for maximum performance in hot paths
- **Memory Pools**: Use `ArrayPool<T>` for temporary large arrays
- **Profile-Guided Optimization**: Regular performance profiling with dotTrace/PerfView

### Testing Strategy
```csharp
// Unit tests with modern C# patterns
[Fact]
public void CalculateSpeed_WithDefaultSetting_ReturnsExpectedRange()
{
    // Arrange
    const float userSetting = 12f;
    
    // Act
    var speeds = Enumerable.Range(0, 1000)
        .Select(_ => SnowPhysics.CalculateSpeed(userSetting))
        .ToArray();
    
    // Assert
    speeds.Should().AllSatisfy(speed => 
    {
        speed.Should().BeInRange(11.6f, 15.0f);
    });
}

// Benchmark tests for performance validation
[Benchmark]
public void UpdateSnowflakes_1000Flakes()
{
    SnowPhysics.UpdateSnowflakes(_snowflakes, 0.016f, 1920, 1080, 0f);
}
```

- **Unit Tests**: xUnit with FluentAssertions for readable test assertions
- **Integration Tests**: Test Windows API interactions in isolated environment
- **Benchmark Tests**: BenchmarkDotNet for performance regression detection
- **Property-Based Testing**: FsCheck for testing edge cases in physics calculations

## Future Enhancements

### Potential Features
- **GUI Configuration**: Settings dialog interface
- **Multi-Monitor**: Support for multiple displays

## Getting Started

### Prerequisites
- **.NET 9 SDK**: Download from https://dotnet.microsoft.com/download/dotnet/9.0
- **Windows 10/11**: Required for WinAPI functionality
- **Visual Studio 2022** or **VS Code with C# extension**: Recommended IDEs
- **Git**: For version control

### Development Setup
```bash
# 1. Clone Repository
git clone <repository-url>
cd nwinsnow

# 2. Verify .NET installation
dotnet --version  # Should show 9.0.x

# 3. Restore dependencies
dotnet restore

# 4. Build project (Debug)
dotnet build

# 5. Run application in wallpaper mode (default)
dotnet run

# 6. Run with custom settings
dotnet run -- --mode screensaver --snow-speed 25 --wind-intensity 30

# 7. Build optimized release
dotnet build -c Release

# 8. Create single-file executable
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

### Project Structure Creation
```bash
# Create new project from scratch
dotnet new sln -n NWinSnow
dotnet new console -n NWinSnow
dotnet sln add NWinSnow/NWinSnow.csproj

# Add test projects
dotnet new xunit -n NWinSnow.Tests
dotnet sln add NWinSnow.Tests/NWinSnow.Tests.csproj

# Install required packages
dotnet add NWinSnow package System.CommandLine
dotnet add NWinSnow package System.Drawing.Common
dotnet add NWinSnow.Tests package FluentAssertions
dotnet add NWinSnow.Tests package BenchmarkDotNet
```

## Example Usage

```bash
# Basic wallpaper mode with default settings (this is the DEFAULT)
dotnet run
# or after publish:
NWinSnow.exe

# Screensaver with heavy snow and strong wind
dotnet run -- --mode screensaver --snow-speed 30 --wind-intensity 40 --wind-chance 60
# or:
NWinSnow.exe --mode screensaver --snow-speed 30 --wind-intensity 40 --wind-chance 60

# Light snow with many trees for desktop background
dotnet run -- --mode wallpaper --trees 24 --snow-speed 5 --wind-chance 5

# Power-save mode for laptop use
dotnet run -- --mode wallpaper --power-save true --max-snowflakes 50

# Windowed mode for testing
dotnet run -- --mode windowed --fps 30

# Maximum performance settings
dotnet run -- --mode screensaver --fps 120 --max-snowflakes 400 --snow-speed 40
```

### Performance Expectations
- **Memory Usage**: <50MB typical, <100MB maximum
- **CPU Usage**: <5% on modern systems (60 FPS), <2% (power save mode)
- **Startup Time**: <500ms from executable launch to first frame
- **Transparency**: Full PNG alpha channel support with hardware acceleration

This specification provides a comprehensive foundation for implementing the nwinsnow application using modern C# development practices, direct Windows API integration, and optimal performance through native code patterns.