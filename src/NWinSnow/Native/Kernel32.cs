using System.Runtime.InteropServices;

namespace NWinSnow.Native;

/// <summary>
/// P/Invoke declarations for Kernel32.dll Windows API functions
/// </summary>
public static class Kernel32
{
    [DllImport("kernel32.dll")]
    public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("kernel32.dll")]
    public static extern bool QueryPerformanceFrequency(out long lpFrequency);

    [DllImport("kernel32.dll")]
    public static extern uint GetTickCount();

    [DllImport("kernel32.dll")]
    public static extern ulong GetTickCount64();

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    public static extern uint GetLastError();

    [DllImport("kernel32.dll")]
    public static extern bool SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

    [DllImport("kernel32.dll")]
    public static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

    [DllImport("kernel32.dll")]
    public static extern void Sleep(uint dwMilliseconds);

    [DllImport("kernel32.dll")]
    public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

    [DllImport("kernel32.dll")]
    public static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS sps);
}

/// <summary>
/// System information structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SYSTEM_INFO
{
    public ushort ProcessorArchitecture;
    public ushort Reserved;
    public uint PageSize;
    public IntPtr MinimumApplicationAddress;
    public IntPtr MaximumApplicationAddress;
    public IntPtr ActiveProcessorMask;
    public uint NumberOfProcessors;
    public uint ProcessorType;
    public uint AllocationGranularity;
    public ushort ProcessorLevel;
    public ushort ProcessorRevision;
}

/// <summary>
/// System power status structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SYSTEM_POWER_STATUS
{
    public byte ACLineStatus;
    public byte BatteryFlag;
    public byte BatteryLifePercent;
    public byte SystemStatusFlag;
    public uint BatteryLifeTime;
    public uint BatteryFullLifeTime;

    public bool IsOnBattery => ACLineStatus == 0;
    public bool IsBatteryLow => BatteryLifePercent < 20;
}
