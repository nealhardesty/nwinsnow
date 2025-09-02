using System.Runtime.InteropServices;

namespace NWinSnow;

internal static class WallpaperHelper
{
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int GWL_EXSTYLE = -20;
    // keep styles simple; avoid switching POPUP/CHILD at runtime

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string? lpszClass, string? lpszWindow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const uint WM_SPAWN_WORKERW = 0x052C;
    private const int SW_SHOW = 5;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_NOSENDCHANGING = 0x0400;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll", SetLastError = true, CharSet=CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
    private const uint GW_HWNDNEXT = 2;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    public static IntPtr FindWallpaperTarget()
    {
        var progman = FindWindow("Progman", null);
        // Ensure WorkerW exists; poke a few times for stubborn shells
        for (int i = 0; i < 5; i++)
        {
            SendMessageTimeout(progman, WM_SPAWN_WORKERW, IntPtr.Zero, IntPtr.Zero, 0, 1000, out _);
            System.Threading.Thread.Sleep(50);
        }

        // Step 1: enumerate top-level windows in Z-order to find the one that hosts SHELLDLL_DefView
        IntPtr iconsHost = IntPtr.Zero;
        EnumWindows((hWnd, l) =>
        {
            var shell = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shell != IntPtr.Zero)
            {
                iconsHost = hWnd;
                return false; // stop
            }
            return true;
        }, IntPtr.Zero);
        if (iconsHost == IntPtr.Zero)
        {
            // Sometimes icons hosted directly on Progman
            var shellOnProg = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellOnProg != IntPtr.Zero)
                iconsHost = progman;
        }

        // Step 2: walk forward in Z-order from iconsHost to find a WorkerW without SHELLDLL_DefView
        IntPtr targetWorkerW = IntPtr.Zero;
        if (iconsHost != IntPtr.Zero)
        {
            IntPtr next = GetWindow(iconsHost, GW_HWNDNEXT);
            var cls = new System.Text.StringBuilder(256);
            while (next != IntPtr.Zero)
            {
                cls.Clear();
                GetClassName(next, cls, cls.Capacity);
                if (cls.ToString() == "WorkerW")
                {
                    var shell = FindWindowEx(next, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (shell == IntPtr.Zero && IsWindowVisible(next))
                    {
                        targetWorkerW = next;
                        break;
                    }
                }
                next = GetWindow(next, GW_HWNDNEXT);
            }
        }

        if (targetWorkerW == IntPtr.Zero)
        {
            // fallback to Progman
            targetWorkerW = progman;
        }

        return targetWorkerW;
    }

    // Styling helpers
    private const int GWL_STYLE = -16;
    private const int WS_CHILD = unchecked((int)0x40000000);
    private const int WS_POPUP = unchecked((int)0x80000000);
    private const int WS_CAPTION = 0x00C00000;
    private const int WS_THICKFRAME = 0x00040000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_SYSMENU = 0x00080000;
    private const int WS_OVERLAPPED = 0x00000000;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int WS_EX_APPWINDOW = 0x00040000;

    public static bool ReparentAndStyleToWallpaper(IntPtr child, IntPtr parent)
    {
        if (parent == IntPtr.Zero || child == IntPtr.Zero) return false;
        SetParent(child, parent);
        int style = GetWindowLong(child, GWL_STYLE);
        style &= ~WS_POPUP;
        style &= ~WS_CAPTION;
        style &= ~WS_THICKFRAME;
        style &= ~WS_MINIMIZEBOX;
        style &= ~WS_MAXIMIZEBOX;
        style &= ~WS_SYSMENU;
        style |= WS_CHILD;
        SetWindowLong(child, GWL_STYLE, style);

        int ex = GetWindowLong(child, GWL_EXSTYLE);
        ex &= ~WS_EX_APPWINDOW;
        ex |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
        SetWindowLong(child, GWL_EXSTYLE, ex);
        return true;
    }

    public static bool TryAttachToWallpaperCandidates(IntPtr child)
    {
        // Build ordered candidate list
        var candidates = new List<IntPtr>();
        var target = FindWallpaperTarget();
        if (target != IntPtr.Zero) candidates.Add(target);
        // Add all visible WorkerWs as fallbacks
        IntPtr ww = IntPtr.Zero;
        while ((ww = FindWindowEx(IntPtr.Zero, ww, "WorkerW", null)) != IntPtr.Zero)
        {
            if (IsWindowVisible(ww) && !candidates.Contains(ww)) candidates.Add(ww);
        }
        // Add Progman last
        var prog = FindWindow("Progman", null);
        if (prog != IntPtr.Zero) candidates.Add(prog);

        foreach (var c in candidates)
        {
            if (ReparentAndStyleToWallpaper(child, c)) return true;
        }
        return false;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    public static Graphics? BeginDraw(out IntPtr targetHwnd, out IntPtr hdc)
    {
        targetHwnd = FindWallpaperTarget();
        hdc = IntPtr.Zero;
        if (targetHwnd == IntPtr.Zero) return null;
        hdc = GetDC(targetHwnd);
        if (hdc == IntPtr.Zero) return null;
        return Graphics.FromHdc(hdc);
    }

    public static void EndDraw(IntPtr targetHwnd, IntPtr hdc, Graphics? g)
    {
        try { g?.Dispose(); } catch { }
        if (hdc != IntPtr.Zero && targetHwnd != IntPtr.Zero)
        {
            ReleaseDC(targetHwnd, hdc);
        }
    }
}


