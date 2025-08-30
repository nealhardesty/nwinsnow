# NWinSnow

NWinSnow is a lightweight Windows desktop snowfall simulation inspired by xsnow. It renders animated snow with occasional wind gusts and randomly placed Christmas trees. It runs in three modes: wallpaper, screensaver-style fullscreen, and standard windowed mode.

## Project Philosophy
- Single-file portable app when published; minimal dependencies
- Smooth visuals at low CPU usage
- Sensible defaults with optional configuration via CLI, env vars, or JSON
- Native Windows integration (WinForms), no installers or registry writes

## Features
- Snow physics: gravity, per-flake size/speed variation, edge wrapping, recycling
- Wind system: calm → phase-in → active → phase-out, random left/right gusts
- Trees: random placement, variable size, optional disable
- Power save mode: adapts to battery status (optional)
- Three display modes: wallpaper, screensaver, windowed
- Configurable FPS, anti-aliasing, texture filtering

## Display Modes
- Wallpaper: the window is reparented to the WorkerW behind desktop icons
- Screensaver: borderless, top-most, ESC to exit
- Windowed: resizable development/testing window

## Configuration
Settings are layered (highest precedence first):
1. Command-line arguments
2. Environment variables (prefix `NWINSNOW_`)
3. `appsettings.json`
4. Built-in defaults

### CLI Options
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

### Environment Variables
- `NWINSNOW_Display__Mode`
- `NWINSNOW_Snow__Speed`, `NWINSNOW_Snow__MaxSnowflakes`, `NWINSNOW_Snow__SpawnRate`
- `NWINSNOW_Wind__Intensity`, `NWINSNOW_Wind__ChancePercent`, `NWINSNOW_Wind__DurationSeconds`
- `NWINSNOW_Trees__Count`, `NWINSNOW_Trees__ScaleMin`, `NWINSNOW_Trees__ScaleMax`
- `NWINSNOW_Performance__PowerSaveEnabled`, `NWINSNOW_Performance__TargetFps`, `NWINSNOW_Performance__VSync`, `NWINSNOW_Performance__HardwareAcceleration`
- `NWINSNOW_Graphics__AntiAliasing`, `NWINSNOW_Graphics__TextureFilteringLinear`, `NWINSNOW_Graphics__ParticleAlphaBlend`

### appsettings.json (example)
```json
{
  "Display": { "Mode": "Wallpaper", "Fullscreen": false },
  "Snow": { "Speed": 12, "MaxSnowflakes": 200, "SpawnRate": 0.1 },
  "Wind": { "Intensity": 5, "ChancePercent": 20, "DurationSeconds": 3.0 },
  "Trees": { "Count": 12, "ScaleMin": 0.8, "ScaleMax": 1.3 },
  "Performance": { "PowerSaveEnabled": false, "TargetFps": 60, "VSync": true, "HardwareAcceleration": true },
  "Graphics": { "AntiAliasing": true, "TextureFilteringLinear": true, "ParticleAlphaBlend": true }
}
```

## Build & Run
Requires .NET 6 SDK on Windows 10/11.

### Using Makefile
- Build: `make build`
- Package (self-contained folder): `make package` (output in `dist/`)
- Run windowed: `make run-windowed`
- Run screensaver: `make run-screensaver`
- Run wallpaper: `make run-wallpaper`

### Using dotnet CLI
- Build: `dotnet build NWinSnow.sln -c Release -f net6.0-windows`
- Run (windowed): `dotnet run --project NWinSnow.csproj -- --mode windowed`

## Assets
- Seven snowflake textures: `assets/snow00.png` .. `assets/snow06.png`
- Tree texture: `assets/tannenbaum.png`
- All assets are embedded as resources and loaded once at startup

## Performance
- Default 60 FPS (configurable 10–120)
- Power Save: if enabled and on battery with low charge, FPS drops to 20 and max snowflakes are halved
- Object pooling: snowflakes are reused, no per-frame allocations

## Code Overview
- `Program.cs`: configuration binding, CLI help, mode wiring
- `MainForm.cs`: main window, render timer, power save handling
- `Systems.cs`: `SnowSystem`, `WindSystem`, `TreesSystem`
- `Assets.cs`: embedded resource loader for textures
- `WallpaperHelper.cs`: WorkerW attach for wallpaper mode
- `Config.cs`: strongly-typed config models
- `CommandLineMap.cs`: CLI switch to config key mapping

## Packaging
Create a distributable folder:
```
dotnet publish NWinSnow.csproj -c Release -f net6.0-windows -o dist
```
Copy the `dist` folder to any Windows machine with .NET 6 runtime (or publish self-contained if desired).

## Notes
- ESC exits only in screensaver mode.
- Wallpaper mode may vary across Windows builds; the app attaches to WorkerW behind icons.
- No network or disk writes; clean exit releases all resources.

## License
MIT (add your preferred license here).
