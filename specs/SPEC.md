# NWinSnow - Project Specification

## Overview

**NWinSnow** is a lightweight Windows desktop snowfall simulation inspired by the classic Unix `xsnow` program. It renders animated falling snow with dynamic wind effects and decorative Christmas trees in three display modes: wallpaper integration, screensaver, and windowed.

### Design Principles
- **Single-file portable application** with minimal dependencies
- **Native Windows integration** using WinForms and Win32 APIs
- **Zero-installation deployment** - no registry or system file modifications
- **Layered configuration** - CLI, environment variables, JSON, and built-in defaults
- **Resource efficient** with intelligent power management

### Target Platform
- Windows 10/11 with .NET 6.0 runtime
- Primary: x64, Compatible: x86

## Architecture

### System Design
The application uses a layered architecture with clear separation of concerns:

- **Application Layer**: Entry point, main window, configuration management
- **Rendering Layer**: Graphics engine, asset management, display configuration
- **Simulation Layer**: Snow physics, wind effects, tree placement
- **Wallpaper Layer**: Windows desktop integration and drawing systems

### Core Philosophy
- **Object pooling** for snowflakes to prevent garbage collection
- **Embedded assets** for single-file deployment
- **Lazy initialization** of components
- **Explicit resource cleanup** and disposal

### Configuration System
Cascading configuration with precedence order:
1. Command-line arguments (highest priority)
2. Environment variables (prefixed `NWINSNOW_`)
3. JSON configuration file (`appsettings.json`)
4. Built-in defaults (fallback)

## Core Systems

### Display Modes
The application supports three distinct modes:

- **Wallpaper Mode**: Renders behind desktop icons, integrated with Windows desktop
- **Screensaver Mode**: Full-screen overlay, exits on ESC key
- **Windowed Mode**: Standard resizable window for development/testing

### Main Components

#### Application Entry Point
- Configuration system initialization using Microsoft.Extensions.Configuration
- Command-line argument parsing and help system
- Display mode setup and window styling
- Desktop integration for wallpaper mode

#### Main Window Controller
- Multi-mode window management with distinct styling per mode
- Renderer lifecycle management
- Paint event handling with mode-specific rendering paths
- Power management integration

#### Rendering Engine
- Timer-based update loop with configurable FPS (10-120)
- Delta-time physics calculations
- Automatic power-save mode on battery (reduces FPS and particle count)
- Graphics quality configuration (anti-aliasing, texture filtering)

#### Asset Management
- Embedded resources for single-file deployment
- 7 snowflake textures (snow00.png - snow06.png) for visual variety
- 1 tree texture (tannenbaum.png) with scaling support
- PNG format with alpha transparency

### Physics Systems

#### Snow Particle System
**Particle Properties:**
- Position coordinates (X, Y)
- Size scaling factor (0.5-1.0 random)
- Individual fall speed (power-law distribution for realism)
- Texture index (0-6 for visual variety)

**Physics Behavior:**
- Time-based spawn ramp: 0 to max snowflakes over 10 seconds
- Vertical movement with gravity and individual speed variation
- Horizontal movement affected by wind force
- Screen edge wrapping (horizontal) and recycling (vertical)
- Object pooling for performance

#### Wind System
**State Machine:**
```
Calm → PhaseIn (1s) → Active (configurable duration) → PhaseOut (1.5s) → Calm
```

**Wind Effects:**
- Random direction (left/right) per storm
- Configurable intensity (1-60), frequency (0-100%), and duration
- Smooth transitions between states
- Affects all particles based on their size and characteristics

#### Tree System
- Random placement across screen bottom
- Configurable count (0-36) and size scaling (0.8x-1.3x default)
- Static placement that persists during session
- Rendered behind snowflakes but in front of desktop

## Wallpaper Integration Technology

### Current Implementation
The application uses three complementary approaches for Windows desktop integration:

#### Desktop Window Targeting (WallpaperHelper)
- Finds Progman window (desktop shell root)
- Sends WM_SPAWN_WORKERW message to create WorkerW windows
- Enumerates windows to find SHELLDLL_DefView host (desktop icons)
- Targets empty WorkerW window for rendering behind icons

#### Direct HDC Rendering (WallpaperDrawer)
- Creates full-screen back buffer with 32bpp ARGB format
- Renders to memory bitmap with high-quality settings
- Blits directly to target window's device context
- Handles transparency and alpha compositing

#### Child Window Management (WallpaperHost)
- Creates native child window parented to WorkerW
- Uses BufferedGraphics for double-buffering
- Manages proper window styles and cleanup

### Proposed VLC-Inspired Enhancement

Based on VLC's wallpaper implementation using **DirectX/Direct3D**, here's the enhanced approach:

#### VLC Integration Method

**DirectX Initialization:**
1. Create DXGI Factory and enumerate graphics adapters
2. Create D3D11 Device with hardware acceleration
3. Obtain desktop window handle (using current WorkerW targeting)
4. Create swap chain targeting desktop window
5. Set up render target views and depth buffers

**VLC-Style Memory Management:**
- Pre-allocate texture buffers for efficient rendering
- Use staging textures for CPU→GPU transfers
- Implement ring buffer for frame management
- Support multiple pixel formats (BGRA, YUV420, etc.)

**Enhanced Rendering Pipeline:**
1. Clear render target with transparent background
2. Render trees using texture sampling with alpha blending
3. Render particles using instanced drawing for performance
4. Apply optional post-processing effects
5. Present to desktop using DirectX present chain

#### Benefits Over Current System

**Performance Improvements:**
- **GPU Acceleration**: Offload rendering to graphics hardware
- **Reduced CPU Usage**: Eliminate software-only GDI+ rendering
- **Better Scaling**: Handle high-DPI displays efficiently
- **Smoother Animation**: Hardware-accelerated alpha blending

**Visual Quality Enhancements:**
- **Sub-pixel Accuracy**: Better positioning and movement
- **Advanced Blending**: More sophisticated transparency effects
- **Shader Effects**: Optional particle effects, glow, blur
- **Multi-monitor Support**: Native spanning across displays

#### Implementation Strategy

**Rendering Mode Options:**
- **Legacy**: Current GDI+ implementation
- **DirectX**: New VLC-inspired DirectX approach
- **Automatic**: Auto-detect best available method

**Backward Compatibility:**
- Detect DirectX 11 availability at runtime
- Fall back to current implementation if DirectX unavailable
- Provide configuration option for manual override
- Maintain identical API surface for seamless integration

## Configuration

### Configuration Schema
The application supports comprehensive configuration through multiple sources:

#### Display Settings
- **Mode**: Wallpaper, Screensaver, or Windowed
- **Fullscreen**: Enable/disable fullscreen mode
- **Monitor**: Target monitor for multi-display setups

#### Snow Physics
- **Speed**: Base falling speed (1-40, default: 12)
- **MaxSnowflakes**: Active particle limit (50-400, default: 200)
- **SpawnRate**: Rate of new snowflake creation (default: 0.1)
- **Size/Speed Variation**: Randomization factors for realism

#### Wind Effects
- **Intensity**: Wind storm strength (1-60, default: 5)
- **ChancePercent**: Storm frequency (0-100%, default: 20%)
- **DurationSeconds**: How long storms last (default: 3.0)

#### Tree Settings
- **Count**: Number of Christmas trees (0-36, default: 12)
- **Scale Range**: Size variation (default: 0.8x to 1.3x)

#### Performance Options
- **PowerSaveEnabled**: Battery optimization (default: false)
- **TargetFps**: Desired frame rate (10-120, default: 60)
- **VSync**: Vertical synchronization
- **HardwareAcceleration**: DirectX support
- **RenderingMode**: Legacy/DirectX/Automatic

#### Graphics Quality
- **AntiAliasing**: Smooth edge rendering
- **TextureFilteringLinear**: Linear vs nearest-neighbor filtering
- **ParticleAlphaBlend**: Alpha blending mode for transparency

### Command Line Interface

```bash
NWinSnow.exe [options]

Common Options:
  --mode <wallpaper|screensaver|windowed>    Display mode
  --snow-speed <1-40>                        Snow falling speed
  --max-snowflakes <50-400>                  Maximum particles
  --wind-intensity <1-60>                    Wind strength
  --wind-chance <0-100>                      Storm frequency %
  --trees <0-36>                             Tree count
  --fps <10-120>                             Target frame rate
  --power-save <true|false>                  Battery optimization
  --rendering-mode <legacy|directx|auto>     Render backend
  --help                                     Show help
```

### Environment Variables
All settings can be overridden using environment variables with `NWINSNOW_` prefix:
- `NWINSNOW_Display__Mode=Wallpaper`
- `NWINSNOW_Snow__MaxSnowflakes=300`
- `NWINSNOW_Performance__TargetFps=30`

## Visual Assets

### Asset Requirements
The application requires specific visual assets for proper operation:

#### Snowflake Textures
- **Files**: `snow00.png` through `snow06.png` (7 variants)
- **Format**: PNG with 32-bit RGBA (full alpha channel)
- **Size**: 16x16 to 32x32 pixels recommended
- **Design**: Distinct snowflake patterns for visual variety
- **Color**: Pure white (#FFFFFF) with varying opacity

#### Christmas Tree Texture
- **File**: `tannenbaum.png`
- **Format**: PNG with 32-bit RGBA
- **Size**: 64x128 to 128x256 pixels recommended
- **Design**: Classic Christmas tree silhouette
- **Color**: Dark green with subtle shading
- **Transparency**: Clean alpha cutout for desktop integration

#### Asset Management
- All assets embedded as resources for single-file deployment
- Loaded once at startup and cached in memory
- Proper disposal on application exit

## Performance Considerations

### Memory Management
- **Object Pooling**: Snowflakes recycled to prevent garbage collection
- **Pre-allocation**: Memory reserved at startup
- **Efficient Data Structures**: Value types for small data, immutable collections

### Power Management
- **Battery Detection**: Automatic detection of battery vs AC power
- **Power Save Mode**: When enabled and on low battery:
  - Reduces FPS from 60 to 20
  - Halves maximum snowflake count (minimum 25)
  - Disables expensive visual effects

### Performance Optimization
- **Adaptive Quality**: Monitor frame times and adjust quality automatically
- **Multi-threading**: Where appropriate for physics calculations
- **Hardware Acceleration**: DirectX path for supported systems

## Build and Deployment

### Project Structure
```
NWinSnow/
├── src/                     # Source code
├── assets/                  # Visual resources (PNG files)
├── config/                  # Configuration files
├── docs/                    # Documentation
├── NWinSnow.csproj         # Project file
├── Makefile                # Build automation
└── appsettings.json        # Default configuration
```

### Build Requirements
- .NET 6.0 SDK
- Windows 10/11 for testing
- Optional: DirectX SDK for enhanced rendering

### Deployment Options
- **Single-file executable**: Self-contained with runtime included
- **Framework-dependent**: Smaller size, requires .NET 6.0 runtime
- **Platform-specific**: x64, x86, or ARM64 builds

## Implementation Notes

### Key Technical Details

#### Desktop Integration Approach
The wallpaper mode uses Windows' WorkerW window system:
1. Locate Progman window (desktop shell)
2. Send WM_SPAWN_WORKERW message to create WorkerW windows
3. Find WorkerW window positioned behind desktop icons
4. Render content to this background window

#### Physics Implementation
- **Particle Lifecycle**: Time-based spawn ramp (0 to max over 10 seconds)
- **Movement**: Gravity-based with individual speed variations
- **Wind Response**: Particles affected by wind based on size/mass
- **Boundary Handling**: Horizontal wrapping, vertical recycling

#### Power Save Behavior
Automatically activates when:
- Power save enabled in configuration
- System running on battery power
- Battery level ≤ 20%

Effects: FPS drops to 20, particle count halved (minimum 25)

### Development Guidelines

#### Error Handling
- Graceful degradation when DirectX unavailable
- Fallback to GDI+ rendering if needed
- Continue operation with reduced functionality rather than crash

#### Performance Targets
- **Memory Usage**: < 50MB typical, < 100MB maximum
- **CPU Usage**: < 5% normal mode, < 2% power save mode
- **Startup Time**: < 500ms from launch to first frame

#### Testing Considerations
- Test on multiple Windows versions (10, 11)
- Verify wallpaper mode across different desktop configurations
- Performance testing on low-end hardware
- Battery life impact assessment

---

This specification provides a complete blueprint for recreating the NWinSnow application. It covers the essential architecture, systems, and implementation details needed to build a lightweight, efficient Windows desktop snowfall simulation.

### Key Features Summary

**Core Application:**
- Three display modes: wallpaper, screensaver, windowed
- Realistic snow physics with wind effects and Christmas trees
- Configurable through CLI, environment variables, and JSON
- Single-file portable deployment

**VLC-Inspired Enhancement:**
- DirectX-based wallpaper rendering for hardware acceleration
- Improved performance and visual quality
- Backward compatibility with current GDI+ implementation
- Multi-monitor support capabilities

**Performance Features:**
- Object pooling for memory efficiency
- Automatic power management on battery
- Adaptive quality based on system performance
- < 50MB memory usage, < 5% CPU usage targets

The specification balances technical detail with implementation flexibility, allowing developers to create a faithful recreation while making appropriate technology choices for their specific requirements.


