# NWinSnow - Windows Snow Simulation

A modern C#/.NET application that displays animated falling snow on Windows systems, inspired by the classic `xsnow` application for X11 Unix systems. Built using **direct Windows API calls via P/Invoke** for maximum performance and minimal overhead.

## Features

- **Dual Display Modes**: Background wallpaper or fullscreen screensaver
- **Dynamic Weather System**: Realistic wind storms with smooth transitions  
- **Multiple Christmas Trees**: Randomly positioned and scaled decorative elements
- **Seven Snowflake Variations**: Different snowflake designs for visual variety
- **Performance Optimization**: Battery-aware rendering with power save mode
- **Command-Line Configuration**: All settings controllable via CLI arguments

## Quick Start

### Prerequisites

- **.NET 6 SDK**: Download from https://dotnet.microsoft.com/download/dotnet/6.0
- **Windows 10/11**: Required for Windows API functionality

### Build and Run

#### Using Makefile (Recommended)
```bash
# Build the project
make build

# Run with default settings (wallpaper mode)
make run

# Display mode helpers
make wallpaper           # Wallpaper mode with default settings
make screensaver         # Screensaver mode (press ESC to exit)
make windowed            # Windowed mode for testing

# Demo modes with enhanced settings
make wallpaper-demo      # Enhanced wallpaper with more trees
make screensaver-demo    # Heavy snow screensaver
make windowed-demo       # Windowed with custom settings

# Create single-file executable
make package

# Show all available commands
make help
```

#### Using .NET CLI Directly
```bash
# Build the project
tools\build.cmd

# Run with default settings (wallpaper mode)
dotnet run --project src\NWinSnow

# Run in screensaver mode
dotnet run --project src\NWinSnow -- --mode screensaver

# Create single-file executable
tools\package.cmd
```

### Command Line Examples

```bash
# Basic wallpaper mode with default settings
NWinSnow.exe

# Screensaver with heavy snow and strong wind
NWinSnow.exe --mode screensaver --snow-speed 30 --wind-intensity 40 --wind-chance 60

# Light snow with many trees for desktop background  
NWinSnow.exe --mode wallpaper --trees 24 --snow-speed 5 --wind-chance 5

# Power-save mode for laptop use
NWinSnow.exe --mode wallpaper --power-save true --max-snowflakes 50

# Windowed mode for testing
NWinSnow.exe --mode windowed --fps 30
```

## Configuration Options

### Display Mode (`--mode`)
- `wallpaper`: Renders behind desktop icons (default)
- `screensaver`: Fullscreen mode, ESC to exit
- `windowed`: Normal window for testing

### Snow Settings
- `--snow-speed <1-40>`: Base falling speed (default: 12)
- `--max-snowflakes <50-400>`: Maximum snowflakes (default: 200)

### Wind Effects  
- `--wind-intensity <1-60>`: Storm strength (default: 5)
- `--wind-chance <0-100>`: Storm frequency % (default: 20)

### Scenery
- `--trees <0-36>`: Number of Christmas trees (default: 12)

### Performance
- `--fps <10-120>`: Target frame rate (default: 60)
- `--power-save <true|false>`: Enable battery optimization (default: false)

## Architecture

The application follows a direct Windows API architecture with minimal abstractions:

- **Window Management**: Direct Win32 window creation and message handling
- **Graphics Rendering**: GDI+ for 2D graphics with hardware acceleration
- **Resource Management**: Manual memory management and object pooling
- **System Integration**: Direct P/Invoke calls to Windows APIs

### Key Components

- **SnowRenderer**: High-performance graphics engine using direct GDI+
- **SnowPhysics**: Physics calculations with no memory allocations
- **WindSystem**: Dynamic wind storm state machine
- **SnowflakePool**: Object pooling to avoid garbage collection
- **PowerService**: Battery monitoring for automatic power save mode

## Performance

### Normal Mode (60 FPS)
- Up to 200 active snowflakes
- Full wind storm effects
- Memory usage: <50MB typical

### Power Save Mode (20 FPS)  
- Up to 100 active snowflakes
- Reduced spawn rate
- CPU usage: <2% on modern systems

## Development

### Project Structure

```
src/
├── NWinSnow/              # Main executable
│   ├── Models/            # Data structures (Snowflake, Tree, WindState)
│   ├── Physics/           # Physics engine and object pooling
│   ├── Graphics/          # Direct GDI+ rendering
│   ├── Native/            # P/Invoke Windows API declarations
│   ├── Services/          # System integration services
│   └── Configuration/     # Settings and command-line parsing
└── NWinSnow.Resources/    # Embedded PNG assets
```

### Build Commands

#### Using Makefile
```bash
# Build commands
make build                # Release build (calls tools\build.cmd)
make debug                # Debug build
make package              # Create single-file executable
make clean                # Clean build artifacts
make install              # Build and package for distribution

# Run commands
make run                  # Run in wallpaper mode
make wallpaper            # Wallpaper mode with default settings
make screensaver          # Screensaver mode (press ESC to exit)
make windowed             # Windowed mode for testing

# Demo commands with enhanced settings
make wallpaper-demo       # Enhanced wallpaper with more trees and wind
make screensaver-demo     # Heavy snow screensaver with strong wind
make windowed-demo        # Windowed with custom frame rate and settings
make test-run             # Quick test in windowed mode

# Utility commands
make info                 # Show project information
make help                 # Show all available commands
```

#### Using .NET CLI Directly
```bash
dotnet build              # Debug build
dotnet build -c Release   # Optimized release build
dotnet run                # Run in wallpaper mode
dotnet publish            # Create deployment package
dotnet clean              # Clean build artifacts
```

## License

This project is inspired by the classic xsnow application and built with modern C# development practices for Windows systems.
