# NWinSnow - Windows Snow Simulation Application Specification

## Overview

**NWinSnow** is a Windows desktop application that displays an animated falling snow simulation with Christmas trees and dynamic weather effects. The application is inspired by the classic `xsnow` application for Unix/X11 systems, adapted for modern Windows with native performance optimization.

## Core Functionality

### Display Modes

The application supports three distinct display modes:

1. **Wallpaper Mode** (Default)
   - Renders snow animation behind desktop icons and windows
   - Integrates seamlessly with the Windows desktop wallpaper system
   - Minimal performance impact for daily use
   - Snow appears to fall "on" the desktop background

2. **Screensaver Mode**
   - Full-screen overlay that covers all other applications
   - Suitable for presentations or actual screensaver use
   - User can exit by pressing the ESC key
   - Higher visual impact with complete screen coverage

3. **Windowed Mode**
   - Standard application window for testing and development
   - Resizable window with normal window controls
   - Useful for debugging and configuration testing

### Snow Physics System

The application simulates realistic snowfall with the following characteristics:

#### Snowflake Behavior
- **Gravity**: Snowflakes fall downward at configurable speeds
- **Size Variation**: Each snowflake has random size scaling (50% to 100% of base size)
- **Speed Variation**: Individual snowflakes fall at different speeds with bias toward slower movement
- **Visual Variety**: Seven different snowflake designs/textures randomly assigned
- **Boundary Wrapping**: Snowflakes wrap around screen edges horizontally
- **Recycling**: Snowflakes reset to top of screen when they fall off the bottom

#### Spawn System
- **Continuous Spawning**: New snowflakes spawn randomly at the top of the screen
- **Rate Control**: Spawn rate configurable from light to heavy snowfall
- **Pool Management**: Efficient object reuse to prevent memory allocation during runtime
- **Maximum Limits**: Configurable maximum number of active snowflakes (50-400)

### Dynamic Wind System

The application features a sophisticated wind storm system:

#### Wind States
1. **Calm** - No wind effects, normal vertical snowfall
2. **Phase In** - Gradually increasing wind intensity (1 second transition)
3. **Active Storm** - Full wind effect with horizontal snowflake movement (3 seconds)
4. **Phase Out** - Gradually decreasing wind intensity (1.5 seconds transition)

#### Wind Characteristics
- **Random Direction**: Storms can blow left or right, chosen randomly
- **Configurable Frequency**: Storm occurrence probability (0-100%)
- **Variable Intensity**: Wind strength affects all snowflakes horizontally
- **Smooth Transitions**: Gradual ramp-up and ramp-down for natural effect
- **Individual Response**: Each snowflake responds differently to wind based on its characteristics

### Christmas Tree Decorations

The application displays decorative Christmas trees with the following features:

#### Tree Placement
- **Random Positioning**: Trees placed randomly across the screen
- **Configurable Count**: Number of trees adjustable (0-36)
- **Size Variation**: Each tree randomly scaled (80% to 130% of original size)
- **No Tree Option**: Can be disabled entirely for snow-only experience

#### Visual Integration
- **Background Layer**: Trees render behind snowflakes but in front of desktop
- **Transparency Support**: Full alpha channel transparency for natural edges
- **Static Placement**: Trees remain in fixed positions during session

### Performance Optimization

#### Power Management
- **Battery Detection**: Automatically detects when running on battery power
- **Power Save Mode**: Reduces performance impact when battery is low
  - Lowers frame rate from 60fps to 20fps
  - Reduces maximum snowflake count by half
  - Simplifies rendering pipeline
- **Manual Override**: Power save mode can be manually enabled/disabled

#### Rendering Optimization
- **Frame Rate Control**: Configurable target frame rate (10-120 fps)
- **Memory Efficiency**: Object pooling prevents garbage collection during runtime
- **Direct Graphics API**: Uses native Windows graphics APIs for maximum performance
- **Hardware Acceleration**: Leverages available graphics hardware acceleration

## Configuration System

### Configuration Sources
The application supports multiple configuration sources in order of precedence:

1. **Command Line Arguments** - Runtime overrides
2. **Environment Variables** - Deployment-specific settings (prefixed with `NWINSNOW_`)
3. **Configuration File** - Default settings in `appsettings.json`
4. **Built-in Defaults** - Fallback values

### Available Settings

#### Display Configuration
- **Mode**: Wallpaper, Screensaver, or Windowed
- **Fullscreen**: Enable/disable fullscreen mode

#### Snow Physics
- **Speed**: Base falling speed (1-40, default: 12)
- **Maximum Snowflakes**: Active snowflake limit (50-400, default: 200)
- **Spawn Rate**: Rate of new snowflake creation (default: 0.1)

#### Wind Effects
- **Intensity**: Wind storm strength (1-60, default: 5)
- **Chance**: Storm frequency percentage (0-100%, default: 20%)
- **Duration**: How long storms last (default: 3 seconds)

#### Tree Settings
- **Count**: Number of Christmas trees (0-36, default: 12)
- **Scale Range**: Minimum and maximum size scaling (default: 0.8x to 1.3x)

#### Performance Settings
- **Power Save Mode**: Enable battery optimization (default: false)
- **Target Frame Rate**: Desired FPS (10-120, default: 60)
- **VSync**: Vertical synchronization (default: true)
- **Hardware Acceleration**: Use graphics hardware (default: true)

#### Graphics Quality
- **Anti-aliasing**: Smooth edge rendering (default: true)
- **Texture Filtering**: Linear or nearest-neighbor filtering
- **Particle Blending**: Alpha blending mode for transparency

### Command Line Interface

The application provides comprehensive command-line control:

```
NWinSnow.exe [options]

Options:
  --mode <wallpaper|screensaver|windowed>    Display mode
  --snow-speed <1-40>                        Snow falling speed
  --max-snowflakes <50-400>                  Maximum active snowflakes
  --wind-intensity <1-60>                    Wind storm strength
  --wind-chance <0-100>                      Wind storm frequency (%)
  --trees <0-36>                             Number of Christmas trees
  --fps <10-120>                             Target frame rate
  --power-save <true|false>                  Enable power optimization
  --help                                     Show help information
```

## Asset Requirements

### Image Assets
The application requires the following visual assets:

#### Snowflake Textures
- **Count**: 7 different snowflake designs
- **Format**: PNG with alpha transparency
- **Naming**: `snow00.png` through `snow06.png`
- **Size**: Recommended 16x16 to 32x32 pixels
- **Transparency**: Full alpha channel support required

#### Christmas Tree Texture
- **Format**: PNG with alpha transparency
- **File**: `tannenbaum.png`
- **Transparency**: Full alpha channel for natural tree edges
- **Size**: Variable, application handles scaling

### Resource Management
- **Embedded Resources**: Images embedded in application for single-file deployment
- **Caching**: Images loaded once and cached in memory
- **Memory Management**: Proper disposal and cleanup of graphics resources

## User Experience

### Ease of Use
- **Zero Configuration**: Works out-of-the-box with sensible defaults
- **Single File**: Can be deployed as a single executable
- **No Installation**: Portable application requiring no installation
- **Minimal System Impact**: Low CPU and memory usage

### Visual Quality
- **Smooth Animation**: 60fps animation (20fps in power save mode)
- **Natural Movement**: Realistic physics with gravity and wind effects
- **Transparency**: Proper alpha blending for snow and tree transparency
- **Visual Variety**: Multiple snowflake designs and random tree placement

### System Integration
- **Windows Native**: Integrates with Windows desktop and power management
- **Multi-Monitor Aware**: Handles multiple display configurations
- **Responsive**: Immediate response to configuration changes
- **Clean Exit**: Proper cleanup of all system resources

## Build and Deployment

### Development Requirements
- **Framework**: .NET 6.0 or later with Windows target
- **Language**: C# with modern language features
- **Dependencies**: Minimal external dependencies for core functionality
- **Build System**: Standard .NET CLI build process

### Packaging Options
- **Self-Contained**: Include runtime for systems without .NET
- **Framework-Dependent**: Smaller size requiring .NET runtime
- **Single File**: All dependencies bundled into single executable
- **Portable**: No installation or registry modifications required

### Distribution Features
- **Makefile Support**: Simplified command-line building
- **Automated Scripts**: Build and packaging automation
- **Cross-Platform Build**: Can be built on any system supporting .NET
- **Continuous Integration**: Suitable for automated build systems

## Technical Characteristics

### Performance Profile
- **Memory Usage**: Less than 50MB typical, under 100MB maximum
- **CPU Usage**: Less than 5% on modern systems (normal mode), under 2% (power save)
- **Startup Time**: Under 500ms from launch to first frame
- **Resource Cleanup**: Proper disposal of all graphics and system resources

### Platform Requirements
- **Operating System**: Windows 10 or Windows 11
- **Runtime**: .NET 6.0 runtime (if framework-dependent deployment)
- **Graphics**: Basic 2D graphics support (available on all Windows systems)
- **Memory**: Minimum 100MB available RAM

### Security Considerations
- **No Network Access**: Application operates entirely offline
- **No File System Writes**: Runs without modifying user files or registry
- **Minimal Permissions**: Requires only basic desktop integration permissions
- **Safe Operation**: No elevation or administrative privileges required

## Future Enhancement Possibilities

### Additional Features
- **Multi-Monitor Support**: Extend snow across multiple displays

### Advanced Configuration
- **GUI Settings Panel**: Graphical configuration interface
- **Real-time Adjustment**: Change settings while application is running
- **Profile System**: Save and load different configuration profiles
- **Automatic Adaptation**: Adjust settings based on system performance or time of day

### System Integration
- **Windows Screensaver**: Register as actual Windows screensaver
- **Taskbar Integration**: System tray icon with quick controls
- **Startup Integration**: Option to launch with Windows
- **Multiple Instance Support**: Different settings for different monitors

This specification provides a complete functional description of the NWinSnow application, focusing on user-visible behavior and configuration options rather than implementation details. The specification is sufficient for complete recreation of the application using any suitable development approach, while maintaining the core user experience and feature set.
