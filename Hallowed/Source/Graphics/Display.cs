using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Graphics;

/// <summary>
/// Centralized display and graphics information.
/// Provides easy access to screen dimensions, resolution management, and display settings.
/// </summary>
public static class Display
{
    static GraphicsDeviceManager manager;
    static GraphicsDevice device;
    
    /// <summary>
    /// Initialize the Display class with the graphics device manager and graphics device.
    /// This should be called once in your GameEngine.Initialize() method.
    /// </summary>
    /// <param name="graphicsManager">The GraphicsDeviceManager instance.</param>
    /// <param name="graphicsDevice">The GraphicsDevice instance.</param>
    public static void Initialize(GraphicsDeviceManager graphicsManager, GraphicsDevice graphicsDevice)
    {
        manager = graphicsManager;
        device = graphicsDevice;
    }
    
    #region Screen Dimensions
    
    /// <summary>
    /// The current display width in pixels.
    /// </summary>
    public static int Width => device?.Viewport.Width ?? 0;
    
    /// <summary>
    /// The current display height in pixels.
    /// </summary>
    public static int Height => device?.Viewport.Height ?? 0;
    
    /// <summary>
    /// The center point of the display.
    /// </summary>
    public static Vector2 Center => new Vector2(Width / 2f, Height / 2f);
    
    /// <summary>
    /// The display bounds as a rectangle (0, 0, Width, Height).
    /// </summary>
    public static Rectangle Bounds => new Rectangle(0, 0, Width, Height);
    
    /// <summary>
    /// The aspect ratio of the display (Width / Height).
    /// </summary>
    public static float AspectRatio => Width / (float)Height;
    
    #endregion
    
    #region Device Access
    
    /// <summary>
    /// Gets the GraphicsDevice instance.
    /// </summary>
    public static GraphicsDevice Device => device;
    
    /// <summary>
    /// Gets the GraphicsDeviceManager instance.
    /// </summary>
    public static GraphicsDeviceManager Manager => manager;
    
    /// <summary>
    /// Gets the current viewport.
    /// </summary>
    public static Viewport Viewport => device?.Viewport ?? default;
    
    #endregion
    
    #region Display Settings
    
    /// <summary>
    /// Gets or sets whether the display is in fullscreen mode.
    /// </summary>
    public static bool IsFullScreen
    {
        get => manager?.IsFullScreen ?? false;
        set
        {
            if (manager != null && manager.IsFullScreen != value)
            {
                manager.IsFullScreen = value;
                manager.ApplyChanges();
            }
        }
    }
    
    /// <summary>
    /// Gets or sets whether VSync is enabled.
    /// </summary>
    public static bool IsVSyncEnabled
    {
        get => manager?.SynchronizeWithVerticalRetrace ?? true;
        set
        {
            if (manager != null && manager.SynchronizeWithVerticalRetrace != value)
            {
                manager.SynchronizeWithVerticalRetrace = value;
                manager.ApplyChanges();
            }
        }
    }
    
    /// <summary>
    /// Toggles fullscreen mode on/off.
    /// </summary>
    public static void ToggleFullScreen()
    {
        IsFullScreen = !IsFullScreen;
    }
    
    #endregion
    
    #region Resolution Management
    
    /// <summary>
    /// Resizes the display to the specified dimensions.
    /// </summary>
    /// <param name="width">The new width in pixels.</param>
    /// <param name="height">The new height in pixels.</param>
    public static void Resize(int width, int height)
    {
        if (manager != null)
        {
            manager.PreferredBackBufferWidth = width;
            manager.PreferredBackBufferHeight = height;
            manager.ApplyChanges();
        }
    }
    
    /// <summary>
    /// Sets the display resolution using a Point.
    /// </summary>
    /// <param name="resolution">The resolution as a Point (X = width, Y = height).</param>
    public static void SetResolution(Point resolution)
    {
        Resize(resolution.X, resolution.Y);
    }
    
    /// <summary>
    /// Common display resolutions.
    /// </summary>
    public static class Resolutions
    {
        /// <summary>HD - 1280x720</summary>
        public static readonly Point HD = new Point(1280, 720);
        
        /// <summary>Full HD - 1920x1080</summary>
        public static readonly Point FullHD = new Point(1920, 1080);
        
        /// <summary>QHD - 2560x1440</summary>
        public static readonly Point QHD = new Point(2560, 1440);
        
        /// <summary>4K UHD - 3840x2160</summary>
        public static readonly Point UHD4K = new Point(3840, 2160);
        
        /// <summary>SNES Resolution - 256x224</summary>
        public static readonly Point SNES = new Point(256, 224);
        
        /// <summary>NES Resolution - 256x240</summary>
        public static readonly Point NES = new Point(256, 240);
        
        /// <summary>Game Boy Resolution - 160x144</summary>
        public static readonly Point GameBoy = new Point(160, 144);
        
        /// <summary>Game Boy Advance Resolution - 240x160</summary>
        public static readonly Point GBA = new Point(240, 160);
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Clears the display with the specified color.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    public static void Clear(Color color)
    {
        device?.Clear(color);
    }
    
    /// <summary>
    /// Checks if a point is within the display bounds.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is within bounds, false otherwise.</returns>
    public static bool Contains(Vector2 point)
    {
        return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
    }
    
    /// <summary>
    /// Checks if a point is within the display bounds.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>True if the point is within bounds, false otherwise.</returns>
    public static bool Contains(float x, float y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
    
    #endregion
}