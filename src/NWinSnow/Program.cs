using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NWinSnow.Configuration;
using NWinSnow.Services;

namespace NWinSnow;

/// <summary>
/// Main entry point for NWinSnow application
/// </summary>
internal class Program
{
    /// <summary>
    /// Application entry point with command-line argument processing
    /// </summary>
    [STAThread]
    private static Task<int> Main(string[] args)
    {
        try
        {
            // Handle help request
            if (args.Contains("--help") || args.Contains("-h") || args.Contains("/?"))
            {
                CommandLineParser.ShowHelp();
                return Task.FromResult(0);
            }

            // Build configuration from multiple sources
            var configuration = CommandLineParser.BuildConfiguration(args);
            
            // Create and configure services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            
            // Get runtime configuration
            var runtimeConfig = serviceProvider.GetRequiredService<RuntimeConfig>();
            
            // Apply power save mode if needed
            var powerService = serviceProvider.GetRequiredService<PowerService>();
            if (powerService.ShouldUsePowerSaveMode())
            {
                runtimeConfig.ApplyPowerSaveMode();
                Console.WriteLine("Power save mode enabled due to low battery.");
            }
            
            // Validate and constrain configuration
            runtimeConfig.ValidateAndConstrain();
            
            // Create and run snow window
            using var snowWindow = new SnowWindow(runtimeConfig);
            
            Console.WriteLine($"Starting NWinSnow in {runtimeConfig.Snow.Display.Mode} mode...");
            Console.WriteLine("Press ESC to exit (in screensaver mode)");
            
            // Run the main application loop
            snowWindow.Run();
            
            Console.WriteLine("NWinSnow terminated successfully.");
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            
            #if DEBUG
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            #endif
            
            return Task.FromResult(1);
        }
    }

    /// <summary>
    /// Configure dependency injection services
    /// </summary>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<SnowConfiguration>(configuration.GetSection("SnowConfiguration"));
        
        // Create runtime config from bound configuration
        services.AddSingleton<RuntimeConfig>(provider =>
        {
            var snowConfig = provider.GetRequiredService<IOptions<SnowConfiguration>>().Value;
            return new RuntimeConfig { Snow = snowConfig };
        });
        
        // Services
        services.AddSingleton<PowerService>();
        services.AddSingleton<ScreenService>();
        services.AddSingleton<WallpaperService>();
        
        // Logging (if needed)
        services.AddLogging();
    }

    /// <summary>
    /// Display application information
    /// </summary>
    private static void DisplayApplicationInfo()
    {
        Console.WriteLine("NWinSnow - Windows Snow Simulation");
        Console.WriteLine("Inspired by the classic xsnow application");
        Console.WriteLine("Built with modern C# and direct Windows APIs");
        Console.WriteLine();
    }

    /// <summary>
    /// Display configuration summary
    /// </summary>
    private static void DisplayConfigurationSummary(RuntimeConfig config)
    {
        Console.WriteLine("Configuration Summary:");
        Console.WriteLine($"  Display Mode: {config.Snow.Display.Mode}");
        Console.WriteLine($"  Snow Speed: {config.Snow.Snow.Speed}");
        Console.WriteLine($"  Max Snowflakes: {config.Snow.Snow.MaxSnowflakes}");
        Console.WriteLine($"  Wind Intensity: {config.Snow.Wind.Intensity}");
        Console.WriteLine($"  Wind Chance: {config.Snow.Wind.Chance}%");
        Console.WriteLine($"  Trees: {config.Snow.Trees.Count}");
        Console.WriteLine($"  Target FPS: {config.Snow.Performance.TargetFrameRate}");
        Console.WriteLine($"  Power Save: {config.Snow.Performance.PowerSaveMode}");
        Console.WriteLine();
    }
}
