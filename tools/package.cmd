@echo off
echo Creating release package...

REM Check if .NET 6 SDK is installed
dotnet --version | findstr /C:"6." >nul
if %ERRORLEVEL% neq 0 (
    echo Error: .NET 6 SDK is required
    echo Please install .NET 6 SDK from https://dotnet.microsoft.com/download/dotnet/6.0
    exit /b 1
)

REM Clean previous builds
echo Cleaning previous builds...
dotnet clean -c Release
if %ERRORLEVEL% neq 0 (
    echo Warning: Clean command failed, continuing...
)

REM Restore dependencies
echo Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo Error: Failed to restore packages
    exit /b %ERRORLEVEL%
)

REM Create single-file executable for Windows x64
echo Publishing single-file executable...
dotnet publish src\NWinSnow\NWinSnow.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:PublishTrimmed=false
if %ERRORLEVEL% neq 0 (
    echo Error: Publish failed
    exit /b %ERRORLEVEL%
)

REM Copy to distribution folder
set DIST_DIR=dist
set PUBLISH_DIR=src\NWinSnow\bin\Release\net6.0-windows\win-x64\publish

if not exist %DIST_DIR% mkdir %DIST_DIR%

echo Copying files to distribution folder...
copy "%PUBLISH_DIR%\NWinSnow.exe" "%DIST_DIR%\" >nul
copy "%PUBLISH_DIR%\appsettings.json" "%DIST_DIR%\" >nul

echo.
echo Package created successfully!
echo.
echo Distribution files are in the '%DIST_DIR%' folder:
dir /B %DIST_DIR%
echo.
echo Run with: %DIST_DIR%\NWinSnow.exe
echo.
echo Examples:
echo   %DIST_DIR%\NWinSnow.exe --mode screensaver --snow-speed 30
echo   %DIST_DIR%\NWinSnow.exe --mode wallpaper --trees 24 --power-save true
