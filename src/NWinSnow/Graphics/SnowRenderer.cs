using System.Drawing;
using System.Drawing.Drawing2D;
using NWinSnow.Models;
using NWinSnow.Native;
using NWinSnow.Resources;

namespace NWinSnow.Graphics;

/// <summary>
/// High-performance snow renderer using direct GDI+ with hardware acceleration
/// </summary>
public unsafe class SnowRenderer : IDisposable
{
    private readonly IntPtr _windowHandle;
    private readonly int _width;
    private readonly int _height;
    
    private IntPtr _hdc;
    private IntPtr _memDC;
    private IntPtr _bitmap;
    private System.Drawing.Graphics _graphics = null!;
    private Bitmap[] _snowTextures = null!;
    private Bitmap _treeTexture = null!;
    
    private bool _disposed;

    /// <summary>
    /// Initialize snow renderer with direct GDI+ setup
    /// </summary>
    public SnowRenderer(IntPtr windowHandle, int width, int height)
    {
        _windowHandle = windowHandle;
        _width = width;
        _height = height;
        
        InitializeGraphics();
        LoadTextures();
    }

    /// <summary>
    /// Initialize direct GDI+ graphics context
    /// </summary>
    private void InitializeGraphics()
    {
        // Direct GDI+ setup for maximum performance
        _hdc = User32.GetDC(_windowHandle);
        _memDC = Gdi32.CreateCompatibleDC(_hdc);
        _bitmap = Gdi32.CreateCompatibleBitmap(_hdc, _width, _height);
        Gdi32.SelectObject(_memDC, _bitmap);
        
        _graphics = System.Drawing.Graphics.FromHdc(_memDC);
        
        // Configure for high-speed rendering
        _graphics.SmoothingMode = SmoothingMode.HighSpeed;
        _graphics.CompositingQuality = CompositingQuality.HighSpeed;
        _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        _graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        _graphics.CompositingMode = CompositingMode.SourceOver;
    }

    /// <summary>
    /// Load all textures with proper alpha channel support
    /// </summary>
    private void LoadTextures()
    {
        _snowTextures = ResourceManager.GetAllSnowflakeTextures();
        _treeTexture = ResourceManager.GetTreeTexture();
    }

    /// <summary>
    /// Render complete frame with trees and snowflakes
    /// </summary>
    public void RenderFrame(ReadOnlySpan<Snowflake> snowflakes, ReadOnlySpan<Tree> trees)
    {
        // Clear with transparent background
        _graphics.Clear(Color.Transparent);
        
        // Draw trees first (back layer)
        RenderTrees(trees);
        
        // Draw snowflakes (front layer)
        RenderSnowflakes(snowflakes);
        
        // Blit to screen with alpha support
        BlitToScreen();
    }

    /// <summary>
    /// Render all trees with scaling and transparency
    /// </summary>
    private void RenderTrees(ReadOnlySpan<Tree> trees)
    {
        for (int i = 0; i < trees.Length; i++)
        {
            ref readonly var tree = ref trees[i];
            
            var destRect = new RectangleF(
                tree.X, 
                tree.Y, 
                tree.RenderedWidth, 
                tree.RenderedHeight);
            
            _graphics.DrawImage(_treeTexture, destRect);
        }
    }

    /// <summary>
    /// Render all active snowflakes with texture variation
    /// </summary>
    private void RenderSnowflakes(ReadOnlySpan<Snowflake> snowflakes)
    {
        for (int i = 0; i < snowflakes.Length; i++)
        {
            ref readonly var flake = ref snowflakes[i];
            if (!flake.Active) continue;
            
            var texture = _snowTextures[flake.TextureIndex];
            var size = 16f * flake.Size; // Base size 16px with scaling
            
            var destRect = new RectangleF(
                flake.X - size / 2, 
                flake.Y - size / 2, 
                size, 
                size);
            
            _graphics.DrawImage(texture, destRect);
        }
    }

    /// <summary>
    /// Blit rendered frame to screen with alpha blending
    /// </summary>
    private void BlitToScreen()
    {
        // Use BitBlt for maximum performance
        Gdi32.BitBlt(_hdc, 0, 0, _width, _height, _memDC, 0, 0, Gdi32.SRCCOPY);
    }

    /// <summary>
    /// Clear the entire rendering surface
    /// </summary>
    public void Clear()
    {
        _graphics.Clear(Color.Transparent);
    }

    /// <summary>
    /// Set rendering quality for performance vs quality trade-off
    /// </summary>
    public void SetRenderingQuality(bool highQuality)
    {
        if (highQuality)
        {
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphics.CompositingQuality = CompositingQuality.HighQuality;
            _graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }
        else
        {
            _graphics.SmoothingMode = SmoothingMode.HighSpeed;
            _graphics.CompositingQuality = CompositingQuality.HighSpeed;
            _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }
    }

    /// <summary>
    /// Present the rendered frame to the window
    /// </summary>
    public void Present()
    {
        BlitToScreen();
        User32.UpdateWindow(_windowHandle);
    }

    /// <summary>
    /// Get the render surface size
    /// </summary>
    public Size GetSize() => new(_width, _height);

    /// <summary>
    /// Dispose all graphics resources
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        
        _graphics?.Dispose();
        
        if (_bitmap != IntPtr.Zero)
        {
            Gdi32.DeleteObject(_bitmap);
            _bitmap = IntPtr.Zero;
        }
        
        if (_memDC != IntPtr.Zero)
        {
            Gdi32.DeleteDC(_memDC);
            _memDC = IntPtr.Zero;
        }
        
        if (_hdc != IntPtr.Zero)
        {
            User32.ReleaseDC(_windowHandle, _hdc);
            _hdc = IntPtr.Zero;
        }
        
        // Note: Texture disposal is handled by ResourceManager
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~SnowRenderer()
    {
        Dispose();
    }
}
