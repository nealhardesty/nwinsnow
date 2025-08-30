using Microsoft.Extensions.Configuration;

namespace NWinSnow;

internal static class CommandLineMap
{
    public static IDictionary<string, string> GetSwitchMappings()
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["--mode"] = "Display:Mode",
            ["--snow-speed"] = "Snow:Speed",
            ["--max-snowflakes"] = "Snow:MaxSnowflakes",
            ["--wind-intensity"] = "Wind:Intensity",
            ["--wind-chance"] = "Wind:ChancePercent",
            ["--trees"] = "Trees:Count",
            ["--fps"] = "Performance:TargetFps",
            ["--power-save"] = "Performance:PowerSaveEnabled"
        };
    }
}


