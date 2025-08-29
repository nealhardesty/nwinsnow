@echo off
echo Building NWinSnow...

REM Check if .NET 6 SDK is installed
dotnet --version | findstr /C:"6." >nul
if %ERRORLEVEL% neq 0 (
    echo Error: .NET 6 SDK is required
    echo Please install .NET 6 SDK from https://dotnet.microsoft.com/download/dotnet/6.0
    exit /b 1
)

REM Restore dependencies
echo Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo Error: Failed to restore packages
    exit /b %ERRORLEVEL%
)

REM Build in Release configuration
echo Building in Release configuration...
dotnet build -c Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo Error: Build failed
    exit /b %ERRORLEVEL%
)

echo Build completed successfully!
echo.
echo Run the application with:
echo   dotnet run --project src\NWinSnow
echo.
echo Or build a single-file executable with:
echo   tools\package.cmd
