using System.CommandLine;
using Microsoft.Extensions.Configuration;

namespace NWinSnow.Configuration;

/// <summary>
/// Command-line argument parsing using System.CommandLine
/// </summary>
public static class CommandLineParser
{
    /// <summary>
    /// Create root command with all supported options
    /// </summary>
    public static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand("NWinSnow - Windows Snow Simulation Application");

        // Display options
        var modeOption = new Option<DisplayMode>(
            name: "--mode",
            description: "Display mode: Wallpaper, Screensaver, or Windowed",
            getDefaultValue: () => DisplayMode.Wallpaper);
        
        var fullscreenOption = new Option<bool>(
            name: "--fullscreen",
            description: "Enable fullscreen mode",
            getDefaultValue: () => false);

        // Snow options
        var snowSpeedOption = new Option<float>(
            name: "--snow-speed",
            description: "Snow falling speed (1-40)",
            getDefaultValue: () => 12f);
        
        var maxSnowflakesOption = new Option<int>(
            name: "--max-snowflakes",
            description: "Maximum number of snowflakes (50-400)",
            getDefaultValue: () => 200);

        // Wind options
        var windIntensityOption = new Option<float>(
            name: "--wind-intensity",
            description: "Wind storm intensity (1-60)",
            getDefaultValue: () => 5f);
        
        var windChanceOption = new Option<float>(
            name: "--wind-chance",
            description: "Wind storm chance percentage (0-100)",
            getDefaultValue: () => 20f);

        // Tree options
        var treesOption = new Option<int>(
            name: "--trees",
            description: "Number of Christmas trees (0-36)",
            getDefaultValue: () => 12);

        // Performance options
        var powerSaveOption = new Option<bool>(
            name: "--power-save",
            description: "Enable power save mode",
            getDefaultValue: () => false);
        
        var fpsOption = new Option<int>(
            name: "--fps",
            description: "Target frame rate (10-120)",
            getDefaultValue: () => 60);

        // Add all options to root command
        rootCommand.AddOption(modeOption);
        rootCommand.AddOption(fullscreenOption);
        rootCommand.AddOption(snowSpeedOption);
        rootCommand.AddOption(maxSnowflakesOption);
        rootCommand.AddOption(windIntensityOption);
        rootCommand.AddOption(windChanceOption);
        rootCommand.AddOption(treesOption);
        rootCommand.AddOption(powerSaveOption);
        rootCommand.AddOption(fpsOption);

        return rootCommand;
    }

    /// <summary>
    /// Parse command line arguments and return configuration overrides
    /// </summary>
    public static async Task<Dictionary<string, object>> ParseArgumentsAsync(string[] args)
    {
        var overrides = new Dictionary<string, object>();
        
        // Simple command line parsing for now - can be enhanced later
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--mode" when i + 1 < args.Length:
                    overrides["SnowConfiguration:Display:Mode"] = args[++i];
                    break;
                case "--snow-speed" when i + 1 < args.Length:
                    if (float.TryParse(args[++i], out var speed))
                        overrides["SnowConfiguration:Snow:Speed"] = speed;
                    break;
                case "--max-snowflakes" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out var maxFlakes))
                        overrides["SnowConfiguration:Snow:MaxSnowflakes"] = maxFlakes;
                    break;
                case "--wind-intensity" when i + 1 < args.Length:
                    if (float.TryParse(args[++i], out var windIntensity))
                        overrides["SnowConfiguration:Wind:Intensity"] = windIntensity;
                    break;
                case "--wind-chance" when i + 1 < args.Length:
                    if (float.TryParse(args[++i], out var windChance))
                        overrides["SnowConfiguration:Wind:Chance"] = windChance;
                    break;
                case "--trees" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out var trees))
                        overrides["SnowConfiguration:Trees:Count"] = trees;
                    break;
                case "--fps" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out var fps))
                        overrides["SnowConfiguration:Performance:TargetFrameRate"] = fps;
                    break;
                case "--power-save" when i + 1 < args.Length:
                    if (bool.TryParse(args[++i], out var powerSave))
                        overrides["SnowConfiguration:Performance:PowerSaveMode"] = powerSave;
                    break;
            }
        }
        
        return await Task.FromResult(overrides);
    }

    /// <summary>
    /// Build configuration with command line overrides
    /// </summary>
    public static IConfiguration BuildConfiguration(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables("NWINSNOW_")
            .AddCommandLine(args, GetCommandLineMappings());

        return builder.Build();
    }

    /// <summary>
    /// Get command line to configuration mappings
    /// </summary>
    private static Dictionary<string, string> GetCommandLineMappings()
    {
        return new Dictionary<string, string>
        {
            ["--mode"] = "SnowConfiguration:Display:Mode",
            ["--fullscreen"] = "SnowConfiguration:Display:FullscreenMode",
            ["--snow-speed"] = "SnowConfiguration:Snow:Speed",
            ["--max-snowflakes"] = "SnowConfiguration:Snow:MaxSnowflakes",
            ["--wind-intensity"] = "SnowConfiguration:Wind:Intensity",
            ["--wind-chance"] = "SnowConfiguration:Wind:Chance",
            ["--trees"] = "SnowConfiguration:Trees:Count",
            ["--power-save"] = "SnowConfiguration:Performance:PowerSaveMode",
            ["--fps"] = "SnowConfiguration:Performance:TargetFrameRate"
        };
    }

    /// <summary>
    /// Display help information
    /// </summary>
    public static void ShowHelp()
    {
        var rootCommand = CreateRootCommand();
        Console.WriteLine(rootCommand.Description);
        Console.WriteLine();
        
        foreach (var option in rootCommand.Options)
        {
            Console.WriteLine($"  {option.Name,-20} {option.Description}");
        }
        
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  NWinSnow.exe --mode screensaver --snow-speed 30 --wind-intensity 40");
        Console.WriteLine("  NWinSnow.exe --mode wallpaper --trees 24 --power-save true");
        Console.WriteLine("  NWinSnow.exe --mode windowed --fps 30 --max-snowflakes 100");
    }
}
