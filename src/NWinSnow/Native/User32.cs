using System.Runtime.InteropServices;

namespace NWinSnow.Native;

/// <summary>
/// P/Invoke declarations for User32.dll Windows API functions
/// </summary>
public static class User32
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x, int y,
        int nWidth, int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(
        IntPtr hwnd,
        uint crKey,
        byte bAlpha,
        uint dwFlags);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X, int Y,
        int cx, int cy,
        uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool UpdateWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll")]
    public static extern IntPtr DispatchMessage(ref MSG lpmsg);

    [DllImport("user32.dll")]
    public static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    public static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll")]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

    [DllImport("user32.dll")]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

    // Window styles
    public const uint WS_OVERLAPPED = 0x00000000;
    public const uint WS_POPUP = 0x80000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_MINIMIZE = 0x20000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_DISABLED = 0x08000000;
    public const uint WS_CLIPSIBLINGS = 0x04000000;
    public const uint WS_CLIPCHILDREN = 0x02000000;
    public const uint WS_MAXIMIZE = 0x01000000;
    public const uint WS_CAPTION = 0x00C00000;
    public const uint WS_BORDER = 0x00800000;
    public const uint WS_DLGFRAME = 0x00400000;
    public const uint WS_VSCROLL = 0x00200000;
    public const uint WS_HSCROLL = 0x00100000;
    public const uint WS_SYSMENU = 0x00080000;
    public const uint WS_THICKFRAME = 0x00040000;
    public const uint WS_GROUP = 0x00020000;
    public const uint WS_TABSTOP = 0x00010000;

    // Extended window styles
    public const uint WS_EX_DLGMODALFRAME = 0x00000001;
    public const uint WS_EX_NOPARENTNOTIFY = 0x00000004;
    public const uint WS_EX_TOPMOST = 0x00000008;
    public const uint WS_EX_ACCEPTFILES = 0x00000010;
    public const uint WS_EX_TRANSPARENT = 0x00000020;
    public const uint WS_EX_MDICHILD = 0x00000040;
    public const uint WS_EX_TOOLWINDOW = 0x00000080;
    public const uint WS_EX_WINDOWEDGE = 0x00000100;
    public const uint WS_EX_CLIENTEDGE = 0x00000200;
    public const uint WS_EX_CONTEXTHELP = 0x00000400;
    public const uint WS_EX_RIGHT = 0x00001000;
    public const uint WS_EX_LEFT = 0x00000000;
    public const uint WS_EX_RTLREADING = 0x00002000;
    public const uint WS_EX_LTRREADING = 0x00000000;
    public const uint WS_EX_LEFTSCROLLBAR = 0x00004000;
    public const uint WS_EX_RIGHTSCROLLBAR = 0x00000000;
    public const uint WS_EX_CONTROLPARENT = 0x00010000;
    public const uint WS_EX_STATICEDGE = 0x00020000;
    public const uint WS_EX_APPWINDOW = 0x00040000;
    public const uint WS_EX_LAYERED = 0x00080000;

    // SetWindowPos flags
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOREDRAW = 0x0008;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_FRAMECHANGED = 0x0020;
    public const uint SWP_SHOWWINDOW = 0x0040;
    public const uint SWP_HIDEWINDOW = 0x0080;
    public const uint SWP_NOCOPYBITS = 0x0100;
    public const uint SWP_NOOWNERZORDER = 0x0200;
    public const uint SWP_NOSENDCHANGING = 0x0400;

    // Special HWND values
    public static readonly IntPtr HWND_TOP = IntPtr.Zero;
    public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    // Show window states
    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;

    // SetLayeredWindowAttributes flags
    public const uint LWA_COLORKEY = 0x00000001;
    public const uint LWA_ALPHA = 0x00000002;

    // Virtual key codes
    public const int VK_ESCAPE = 0x1B;
    public const int VK_SPACE = 0x20;

    // Window messages
    public const uint WM_DESTROY = 0x0002;
    public const uint WM_PAINT = 0x000F;
    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
    public const uint WM_TIMER = 0x0113;

    // PeekMessage flags
    public const uint PM_NOREMOVE = 0x0000;
    public const uint PM_REMOVE = 0x0001;
    public const uint PM_NOYIELD = 0x0002;
}
