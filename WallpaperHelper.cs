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

    public static IntPtr FindWallpaperTarget()
    {
        var progman = FindWindow("Progman", null);
        SendMessageTimeout(progman, WM_SPAWN_WORKERW, IntPtr.Zero, IntPtr.Zero, 0, 1000, out _);

        // Find the WorkerW that hosts icons (or Progman if that's where it is)
        IntPtr iconsHost = IntPtr.Zero;
        var shellOnProgman = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
        if (shellOnProgman != IntPtr.Zero)
        {
            iconsHost = progman;
        }
        else
        {
            IntPtr w = IntPtr.Zero;
            while ((w = FindWindowEx(IntPtr.Zero, w, "WorkerW", null)) != IntPtr.Zero)
            {
                var shell = FindWindowEx(w, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell != IntPtr.Zero)
                {
                    iconsHost = w;
                    break;
                }
            }
        }

        // Prefer the WorkerW immediately after the icons host
        IntPtr targetWorkerW = IntPtr.Zero;
        if (iconsHost != IntPtr.Zero)
        {
            targetWorkerW = FindWindowEx(IntPtr.Zero, iconsHost, "WorkerW", null);
        }

        // If not found, pick any WorkerW that doesn't contain the icons
        if (targetWorkerW == IntPtr.Zero)
        {
            IntPtr w = IntPtr.Zero;
            while ((w = FindWindowEx(IntPtr.Zero, w, "WorkerW", null)) != IntPtr.Zero)
            {
                if (w == iconsHost) continue;
                var shell = FindWindowEx(w, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell == IntPtr.Zero)
                {
                    targetWorkerW = w;
                    break;
                }
            }
        }

        if (targetWorkerW == IntPtr.Zero)
        {
            targetWorkerW = progman;
        }

        return targetWorkerW;
    }
}


