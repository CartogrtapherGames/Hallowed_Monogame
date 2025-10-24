using System;
using Hallowed.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Graphics;

/// <summary>
/// Wrapper class for allow easy texture manipulation.
/// <remarks>
/// This class is optional and not required for creating sprites.
/// </remarks>
/// <code>
///  var texture = new TextureWrapper(Content.Load("texture"));
///  var sprite = new Sprite(texture);
///  var texture2 = Content.Load("texture2");
///  texture.blit(texture2, new Rectangle(0, 0, 100, 100), new Rectangle(0, 0, 100, 100);
/// </code>
/// </summary>
public class TextureWrapper : IDisposable
{

  private readonly GraphicsDevice _device;
  private readonly SpriteBatch _spriteBatch;
  private readonly PingPongBuffer _buffer;
  private Texture2D _texture;

  /// <summary>
  /// The texture wrapper constructor.
  /// </summary>
  /// <param name="device"> The graphic device</param>
  /// <param name="width"> the texture wrapper width (automatically adjust when assigning a texture)</param>
  /// <param name="height">the texture wrapper height (automatically adjust when assigning a texture)</param>
  public TextureWrapper(GraphicsDevice device, int width, int height)
  {
    _device = device;
    _spriteBatch = new SpriteBatch(device);
    _buffer = new PingPongBuffer(device, width, height);
  }
  
  /// <summary>
  /// The texture wrapper constructor.
  /// </summary>
  /// <param name="device"> The graphic device</param>
  /// <param name="texture"> the texture to wrap</param>
  public TextureWrapper(GraphicsDevice device, Texture2D texture) : this(device, texture.Width, texture.Height)
  {
    _texture = texture;
  }

  /// <summary>
  /// The texture
  /// </summary>
  public Texture2D Texture
  {
    get => _texture;
    set
    {
      _texture = value;
      _buffer.Assign(_texture);
    }
  }
  
  /// <summary>
  /// the texture width.
  /// </summary>
  public int Width => _texture.Width;
  
  /// <summary>
  /// the texture height.
  /// </summary>
  public int Height => _texture.Height;

  /// <summary>
  ///  copy a region of one texture to another.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="sourceRect"></param>
  /// <param name="destRect"></param>
  public void Blit(Texture2D source, Rectangle sourceRect, Rectangle destRect)
  {
    if (_texture == null) return;

    var renderTarget = _buffer.Target;

    _device.SetRenderTarget(renderTarget);
    _device.Clear(Color.Transparent);

    _spriteBatch.Begin();
    _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
    _spriteBatch.Draw(source, destRect, sourceRect, Color.White);
    _spriteBatch.End();

    _device.SetRenderTarget(null);
    _buffer.Swap();
    _texture = _buffer.Source;
  }

  public void Dispose()
  {
    _buffer?.Dispose();
    _texture?.Dispose();
    _spriteBatch?.Dispose();
  }
}
