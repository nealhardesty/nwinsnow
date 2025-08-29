using System.Drawing;
using System.Runtime.InteropServices;
using NWinSnow.Configuration;
using NWinSnow.Graphics;
using NWinSnow.Models;
using NWinSnow.Native;
using NWinSnow.Physics;

namespace NWinSnow;

/// <summary>
/// Main window class with direct Windows API calls for maximum performance
/// </summary>
public class SnowWindow : IDisposable
{
    private const string WindowClassName = "NWinSnowWindow";
    private readonly RuntimeConfig _config;
    private readonly WndProc _wndProc;
    
    private IntPtr _windowHandle;
    private IntPtr _moduleHandle;
    private bool _disposed;
    private bool _running;
    
    // Graphics and rendering
    private SnowRenderer? _renderer;
    private int _screenWidth;
    private int _screenHeight;
    
    // Snow simulation
    private SnowflakePool? _snowflakePool;
    private Tree[] _trees = Array.Empty<Tree>();
    private WindSystem _windSystem;
    
    // Timing
    private long _performanceFrequency;
    private long _lastFrameTime;
    private float _deltaTime;
    private int _frameCount;
    private float _frameTimer;

    /// <summary>
    /// Initialize snow window with configuration
    /// </summary>
    public SnowWindow(RuntimeConfig config)
    {
        _config = config;
        _wndProc = WindowProcedure;
        _running = true;
        
        InitializeTimers();
        InitializeWindow();
        InitializeSnowSimulation();
    }

    /// <summary>
    /// Initialize high-resolution timers
    /// </summary>
    private void InitializeTimers()
    {
        Kernel32.QueryPerformanceFrequency(out _performanceFrequency);
        Kernel32.QueryPerformanceCounter(out _lastFrameTime);
    }

    /// <summary>
    /// Create and configure the main window
    /// </summary>
    private void InitializeWindow()
    {
        _moduleHandle = Kernel32.GetModuleHandle(null!);
        GetScreenDimensions();
        RegisterWindowClass();
        CreateMainWindow();
        ConfigureWindowForDisplayMode();
        ShowWindow();
    }

    /// <summary>
    /// Get screen dimensions for the primary monitor
    /// </summary>
    private void GetScreenDimensions()
    {
        var desktopWindow = User32.GetDesktopWindow();
        User32.GetWindowRect(desktopWindow, out var rect);
        _screenWidth = rect.Width;
        _screenHeight = rect.Height;
    }

    /// <summary>
    /// Register the window class with Windows
    /// </summary>
    private void RegisterWindowClass()
    {
        var wndClass = new WNDCLASS
        {
            style = 0,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = _moduleHandle,
            hIcon = IntPtr.Zero,
            hCursor = IntPtr.Zero,
            hbrBackground = IntPtr.Zero,
            lpszMenuName = null!,
            lpszClassName = WindowClassName
        };

        var classAtom = User32.RegisterClass(ref wndClass);
        if (classAtom == 0)
            throw new InvalidOperationException($"Failed to register window class. Error: {Kernel32.GetLastError()}");
    }

    /// <summary>
    /// Create the main application window
    /// </summary>
    private void CreateMainWindow()
    {
        uint style = User32.WS_POPUP;
        uint exStyle = User32.WS_EX_LAYERED | User32.WS_EX_TRANSPARENT | User32.WS_EX_TOOLWINDOW;

        _windowHandle = User32.CreateWindowEx(
            exStyle,
            WindowClassName,
            "NWinSnow",
            style,
            0, 0,
            _screenWidth, _screenHeight,
            IntPtr.Zero,
            IntPtr.Zero,
            _moduleHandle,
            IntPtr.Zero);

        if (_windowHandle == IntPtr.Zero)
            throw new InvalidOperationException($"Failed to create window. Error: {Kernel32.GetLastError()}");

        // Configure transparency
        User32.SetLayeredWindowAttributes(_windowHandle, 0, 255, User32.LWA_ALPHA);
    }

    /// <summary>
    /// Configure window based on display mode
    /// </summary>
    private void ConfigureWindowForDisplayMode()
    {
        switch (_config.Snow.Display.Mode)
        {
            case DisplayMode.Wallpaper:
                // Position behind desktop icons
                User32.SetWindowPos(_windowHandle, User32.HWND_BOTTOM, 0, 0, 0, 0, 
                    User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOACTIVATE);
                break;
                
            case DisplayMode.Screensaver:
                // Position on top
                User32.SetWindowPos(_windowHandle, User32.HWND_TOPMOST, 0, 0, 0, 0, 
                    User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOACTIVATE);
                break;
                
            case DisplayMode.Windowed:
                // Normal window positioning
                var windowWidth = Math.Min(_screenWidth, 1024);
                var windowHeight = Math.Min(_screenHeight, 768);
                var x = (_screenWidth - windowWidth) / 2;
                var y = (_screenHeight - windowHeight) / 2;
                
                User32.SetWindowPos(_windowHandle, IntPtr.Zero, x, y, windowWidth, windowHeight, 0);
                _screenWidth = windowWidth;
                _screenHeight = windowHeight;
                break;
        }
    }

    /// <summary>
    /// Show the window
    /// </summary>
    private void ShowWindow()
    {
        User32.ShowWindow(_windowHandle, User32.SW_SHOW);
        User32.UpdateWindow(_windowHandle);
    }

    /// <summary>
    /// Initialize snow simulation components
    /// </summary>
    private void InitializeSnowSimulation()
    {
        // Initialize renderer
        _renderer = new SnowRenderer(_windowHandle, _screenWidth, _screenHeight);
        
        // Initialize snowflake pool
        var maxSnowflakes = _config.Snow.Performance.PowerSaveMode 
            ? Math.Min(_config.Snow.Snow.MaxSnowflakes, 100)
            : _config.Snow.Snow.MaxSnowflakes;
        _snowflakePool = new SnowflakePool(maxSnowflakes);
        
        // Initialize wind system
        _windSystem = new WindSystem(_config.Snow.Wind.Chance, _config.Snow.Wind.Intensity);
        
        // Create trees
        CreateTrees();
    }

    /// <summary>
    /// Create randomly positioned trees
    /// </summary>
    private void CreateTrees()
    {
        if (_config.Snow.Trees.Count <= 0) return;

        var random = new Random();
        var treeTexture = NWinSnow.Resources.ResourceManager.GetTreeTexture();
        _trees = new Tree[_config.Snow.Trees.Count];

        for (int i = 0; i < _config.Snow.Trees.Count; i++)
        {
            _trees[i] = Tree.CreateRandom(random, _screenWidth, _screenHeight, 
                treeTexture.Width, treeTexture.Height);
        }
    }

    /// <summary>
    /// Main message loop and simulation update
    /// </summary>
    public void Run()
    {
        while (_running)
        {
            ProcessWindowMessages();
            
            if (_running && !_config.IsPaused)
            {
                UpdateSimulation();
                RenderFrame();
            }
            
            LimitFrameRate();
        }
    }

    /// <summary>
    /// Process Windows messages
    /// </summary>
    private void ProcessWindowMessages()
    {
        while (User32.PeekMessage(out var msg, IntPtr.Zero, 0, 0, User32.PM_REMOVE))
        {
            if (msg.message == User32.WM_KEYDOWN && msg.wParam == (IntPtr)User32.VK_ESCAPE)
            {
                if (_config.Snow.Display.Mode == DisplayMode.Screensaver)
                {
                    _running = false;
                }
            }

            User32.TranslateMessage(ref msg);
            User32.DispatchMessage(ref msg);
        }
    }

    /// <summary>
    /// Update snow simulation physics
    /// </summary>
    private void UpdateSimulation()
    {
        UpdateDeltaTime();
        
        // Update wind system
        _windSystem.Update(_deltaTime);
        
        // Spawn new snowflakes
        _snowflakePool!.SpawnSnowflakes(_config.Snow.Snow.SpawnRate, _deltaTime, 
            _screenWidth, _config.Snow.Snow.Speed);
        
        // Update snowflake physics
        var snowflakes = _snowflakePool.GetMutableSnowflakes();
        SnowPhysics.UpdateSnowflakes(snowflakes, _deltaTime, _screenWidth, _screenHeight, 
            _windSystem.GetWindEffect());
    }

    /// <summary>
    /// Update delta time for frame-rate independent animation
    /// </summary>
    private void UpdateDeltaTime()
    {
        Kernel32.QueryPerformanceCounter(out var currentTime);
        _deltaTime = (float)(currentTime - _lastFrameTime) / _performanceFrequency;
        _lastFrameTime = currentTime;
        
        // Clamp delta time to prevent large jumps
        _deltaTime = Math.Min(_deltaTime, 1.0f / 15.0f); // Max 15 FPS minimum
    }

    /// <summary>
    /// Render the current frame
    /// </summary>
    private void RenderFrame()
    {
        var snowflakes = _snowflakePool!.GetActiveSnowflakes();
        _renderer!.RenderFrame(snowflakes, _trees);
        
        UpdateFrameCounter();
    }

    /// <summary>
    /// Update frame counter for performance monitoring
    /// </summary>
    private void UpdateFrameCounter()
    {
        _frameCount++;
        _frameTimer += _deltaTime;
        
        if (_frameTimer >= 1.0f)
        {
            // Reset counter every second (could be used for FPS display)
            _frameCount = 0;
            _frameTimer = 0f;
        }
    }

    /// <summary>
    /// Limit frame rate to target FPS
    /// </summary>
    private void LimitFrameRate()
    {
        var targetFrameTime = 1000.0f / _config.Snow.Performance.TargetFrameRate;
        var actualFrameTime = _deltaTime * 1000.0f;
        var sleepTime = (int)(targetFrameTime - actualFrameTime);
        
        if (sleepTime > 0)
        {
            Kernel32.Sleep((uint)sleepTime);
        }
    }

    /// <summary>
    /// Window procedure for message handling
    /// </summary>
    private IntPtr WindowProcedure(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
    {
        switch (uMsg)
        {
            case User32.WM_DESTROY:
                _running = false;
                User32.PostQuitMessage(0);
                return IntPtr.Zero;
                
            case User32.WM_PAINT:
                // Handle paint messages if needed
                return IntPtr.Zero;
                
            default:
                return User32.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }
    }

    /// <summary>
    /// Stop the simulation
    /// </summary>
    public void Stop()
    {
        _running = false;
    }

    /// <summary>
    /// Dispose all resources
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        _running = false;
        
        _renderer?.Dispose();
        
        if (_windowHandle != IntPtr.Zero)
        {
            User32.DestroyWindow(_windowHandle);
            _windowHandle = IntPtr.Zero;
        }
        
        if (_moduleHandle != IntPtr.Zero)
        {
            User32.UnregisterClass(WindowClassName, _moduleHandle);
        }
        
        NWinSnow.Resources.ResourceManager.Cleanup();
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~SnowWindow()
    {
        Dispose();
    }
}
