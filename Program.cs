using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace NWinSnow;

static class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [STAThread]
    static void Main(string[]? args)
    {
        ApplicationConfiguration.Initialize();

        var config = BuildConfiguration(args ?? Array.Empty<string>());
        var appConfig = new Config.AppConfig();
        config.Bind(appConfig);

        if (args != null && args.Any(a => string.Equals(a, "--help", StringComparison.OrdinalIgnoreCase)))
        {
            try { AllocConsole(); } catch { }
            PrintHelp();
            return;
        }

        using var form = new MainForm(appConfig);

        switch (appConfig.Display.Mode)
        {
            case Config.DisplayMode.Screensaver:
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.TopMost = true;
                break;
            case Config.DisplayMode.Windowed:
                form.Text = "NWinSnow";
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ClientSize = new Size(1280, 720);
                break;
            case Config.DisplayMode.Wallpaper:
                // Window will reparent itself to WorkerW in OnShown
                form.Text = "NWinSnow (Wallpaper)";
                break;
        }

        Application.Run(form);
    }

    private static IConfiguration BuildConfiguration(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "NWINSNOW_")
            .AddCommandLine(args, CommandLineMap.GetSwitchMappings());
        return builder.Build();
    }

    private static void PrintHelp()
    {
        Console.WriteLine("NWinSnow.exe [options]\n");
        Console.WriteLine("Options:");
        Console.WriteLine("  --mode <wallpaper|screensaver|windowed>    Display mode");
        Console.WriteLine("  --snow-speed <1-40>                        Snow falling speed");
        Console.WriteLine("  --max-snowflakes <50-400>                  Maximum active snowflakes");
        Console.WriteLine("  --wind-intensity <1-60>                    Wind storm strength");
        Console.WriteLine("  --wind-chance <0-100>                      Wind storm frequency (%)");
        Console.WriteLine("  --trees <0-36>                             Number of Christmas trees");
        Console.WriteLine("  --fps <10-120>                             Target frame rate");
        Console.WriteLine("  --power-save <true|false>                  Enable power optimization");
        Console.WriteLine("  --help                                     Show help information");
    }
}


