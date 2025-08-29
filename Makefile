# NWinSnow - Windows Snow Simulation Application
# Makefile for simplified command-line building

.PHONY: help build package clean run install restore test wallpaper screensaver windowed wallpaper-demo screensaver-demo windowed-demo
.DEFAULT_GOAL := help

# Display help information
help:
	@echo NWinSnow - Windows Snow Simulation Application
	@echo.
	@echo Available targets:
	@echo   help      - Show this help message
	@echo   restore   - Restore NuGet packages
	@echo   build     - Build the application (calls tools\build.cmd)
	@echo   package   - Create single-file executable (calls tools\package.cmd)
	@echo   run       - Run the application in wallpaper mode
	@echo   clean     - Clean build artifacts
	@echo   install   - Build and package for distribution
	@echo.
	@echo Display Mode Helpers:
	@echo   wallpaper        - Run in wallpaper mode (default settings)
	@echo   screensaver      - Run in screensaver mode (press ESC to exit)
	@echo   windowed         - Run in windowed mode for testing
	@echo   wallpaper-demo   - Wallpaper mode with enhanced settings
	@echo   screensaver-demo - Screensaver mode with heavy snow
	@echo   windowed-demo    - Windowed mode with custom settings
	@echo.
	@echo Examples:
	@echo   make build
	@echo   make package
	@echo   make wallpaper
	@echo   make screensaver-demo
	@echo   make run ARGS="--mode screensaver --snow-speed 30"

# Restore NuGet packages
restore:
	@echo Restoring NuGet packages...
	dotnet restore

# Build the application using the existing build script
build:
	@echo Building NWinSnow...
	tools\build.cmd

# Create single-file executable using the existing package script
package:
	@echo Creating distribution package...
	tools\package.cmd

# Run the application with optional arguments
run:
	@echo Running NWinSnow...
ifdef ARGS
	dotnet run --project src\NWinSnow -- $(ARGS)
else
	dotnet run --project src\NWinSnow
endif

# Clean build artifacts
clean:
	@echo Cleaning build artifacts...
	dotnet clean
	@if exist dist rmdir /s /q dist
	@echo Clean completed.

# Build and package for distribution
install: package
	@echo.
	@echo Installation package ready in dist\ folder:
	@dir dist /b
	@echo.
	@echo Run with: dist\NWinSnow.exe

# Alternative targets for common scenarios
debug: restore
	@echo Building in Debug configuration...
	dotnet build

release: restore
	@echo Building in Release configuration...
	dotnet build -c Release

# Quick test run in windowed mode
test-run:
	@echo Running NWinSnow in windowed test mode...
	dotnet run --project src\NWinSnow -- --mode windowed --fps 30 --trees 5

# Display mode helpers
wallpaper:
	@echo Running NWinSnow in wallpaper mode...
	dotnet run --project src\NWinSnow -- --mode wallpaper

screensaver:
	@echo Running NWinSnow in screensaver mode...
	@echo Press ESC to exit when running...
	dotnet run --project src\NWinSnow -- --mode screensaver

windowed:
	@echo Running NWinSnow in windowed mode...
	dotnet run --project src\NWinSnow -- --mode windowed

# Display mode helpers with custom settings
wallpaper-demo:
	@echo Running NWinSnow wallpaper demo with enhanced settings...
	dotnet run --project src\NWinSnow -- --mode wallpaper --trees 18 --snow-speed 15 --wind-intensity 8

screensaver-demo:
	@echo Running NWinSnow screensaver demo with heavy snow...
	@echo Press ESC to exit when running...
	dotnet run --project src\NWinSnow -- --mode screensaver --snow-speed 25 --wind-intensity 15 --wind-chance 40 --trees 6

windowed-demo:
	@echo Running NWinSnow windowed demo...
	dotnet run --project src\NWinSnow -- --mode windowed --fps 60 --trees 8 --snow-speed 20

# Show project information
info:
	@echo NWinSnow Project Information:
	@echo   Target Framework: .NET 6.0 Windows
	@echo   Architecture: Direct Windows API (P/Invoke)
	@echo   Graphics: GDI+ with hardware acceleration
	@echo   Features: Snow physics, wind system, Christmas trees
	@echo   Modes: Wallpaper, Screensaver, Windowed
	@echo.
	@dotnet --version
