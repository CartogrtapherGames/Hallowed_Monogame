using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Rendering;

enum PingPongBufferType
{
  A,
  B
}

/// <summary>
/// A ping-pong buffer for efficient render texture operations in MonoGame.
/// Maintains two render textures and alternates between them to avoid read/write conflicts.
/// </summary>
/// <remarks>
/// The ping-pong pattern allows you to read from one texture (Source) while writing to another (Target),
/// then swap them for the next iteration. This is essential for iterative post-processing, feedback loops,
/// and blitting multiple textures together without memory reallocation overhead.
/// </remarks>
/// <example>
/// Initialize the buffer and blit textures together:
/// <code>
/// var buffer = new PingPongBuffer(graphicsDevice, 1024, 1024);
///
/// // Render to the target texture
/// graphicsDevice.SetRenderTarget(buffer.Target);
/// graphicsDevice.Clear(Color.Transparent);
///
/// 
/// spriteBatch.Begin();
/// spriteBatch.Draw(texture1, Vector2.Zero, Color.White);
/// spriteBatch.Draw(texture2, new Vector2(512, 0), Color.White);
/// spriteBatch.End();
///
/// 
/// graphicsDevice.SetRenderTarget(null);
/// buffer.Swap(); // Swap so Target becomes Source for next iteration
///
/// 
/// Texture2D result = buffer.Source; // Result is now ready to use
/// buffer.Dispose();
/// </code>
/// </example>
/// <link>https://en.wikipedia.org/wiki/Ping-pong_scheme</link>

public class PingPongBuffer : IDisposable
{
  
  private GraphicsDevice _graphicsDevice;
  private SpriteBatch _spriteBatch;
  private RenderTarget2D _textureA;
  private RenderTarget2D _textureB;
  private PingPongBufferType _current;
  private bool _hasBeenInitialized;
  
  /// <summary>
  /// The ping pong buffer constructor.
  /// </summary>
  /// <param name="device"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <param name="format"></param>
  public PingPongBuffer(GraphicsDevice device, int width, int height, SurfaceFormat format = SurfaceFormat.Color)
  {
    _graphicsDevice = device;
    _spriteBatch = new SpriteBatch(_graphicsDevice);
    _textureA = CreateRenderTarget(width, height, format);
    _textureB = CreateRenderTarget(width, height, format);
    _current = PingPongBufferType.A;
    _hasBeenInitialized = false;
  }

  /// <summary>
  /// create a render target.
  /// </summary>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  private RenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat format)
  {
    return new RenderTarget2D(
      _graphicsDevice,
      width,
      height,
      false,
      format,
      DepthFormat.None
    );
  }
  
  /// <summary>Gets the current source texture (read from this).</summary>
  public RenderTarget2D Source => _current == PingPongBufferType.A ? _textureA : _textureB;
  
  /// <summary>Gets the current target texture (write to this).</summary>
  public RenderTarget2D Target => _current == PingPongBufferType.A ? _textureB : _textureA;
  
  /// <summary>
  /// the buffer width.
  /// </summary>
  public int Width => _textureA.Width;
  
  /// <summary>
  /// the buffer height. 
  /// </summary>
  public int Height => _textureA.Height;
  
  /// <summary>
  /// Swap source and target buffers.
  /// </summary>
  public void Swap()
  {
    _current = _current == PingPongBufferType.A ? PingPongBufferType.B : PingPongBufferType.A;
  }


  /// <summary>
  /// Clear the current source to transparent.
  /// </summary>
  public void Clear()
  {
    _graphicsDevice.SetRenderTarget(Source);
    _graphicsDevice.Clear(Color.Transparent);
    _graphicsDevice.SetRenderTarget(null);
  }

  /// <summary>
  /// Clear both buffers.
  /// </summary>
  public void ClearAll()
  {
    _graphicsDevice.SetRenderTarget(_textureA);
    _graphicsDevice.Clear(Color.Transparent);

    _graphicsDevice.SetRenderTarget(_textureB);
    _graphicsDevice.Clear(Color.Transparent);

    _graphicsDevice.SetRenderTarget(null);
  }

  /// <summary>
  /// Resize both buffers.
  /// </summary>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <param name="format"></param>
  public void Resize(int width, int height, SurfaceFormat format = SurfaceFormat.Color)
  {
    _textureA?.Dispose();
    _textureB?.Dispose();
    _textureA = CreateRenderTarget(width, height, format);
    _textureB = CreateRenderTarget(width, height, format);
  }

  /// <summary>
  /// First assign a texture to the source and target buffers.
  /// </summary>
  /// <param name="texture"></param>
  public void Assign(Texture2D texture)
  {
    Resize(texture.Width, texture.Height, texture.Format);
    DrawToRenderTarget(texture, Source);
    DrawToRenderTarget(texture, Target);
    _hasBeenInitialized = true;
  }

  /// <summary>
  /// draw to the render target.
  /// </summary>
  /// <param name="texture"></param>
  /// <param name="renderTarget"></param>
  private void DrawToRenderTarget(Texture2D texture, RenderTarget2D renderTarget)
  {
    _graphicsDevice.SetRenderTarget(renderTarget);
    _graphicsDevice.Clear(Color.Transparent);

    _spriteBatch.Begin();
    _spriteBatch.Draw(texture, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
    _spriteBatch.End();

    _graphicsDevice.SetRenderTarget(null);
  }
  
  /// <summary>
  /// Check if the buffer has been initialized.
  /// </summary>
  public bool HasSource => _hasBeenInitialized;
  
  /// <summary>
  /// Dispose the buffer.
  /// </summary>
  public void Dispose()
  {
    _graphicsDevice = null;
    _textureA?.Dispose();
    _textureB?.Dispose();
    _spriteBatch?.Dispose();
    _spriteBatch = null;
    _textureA = null;
    _textureB = null;
  }
}
