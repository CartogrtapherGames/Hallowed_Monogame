using System;
using System.Collections.Generic;
using Hallowed.Manager;
using Hallowed.Rendering.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Graphics;

/// <summary>
/// Wrapper class to simplify texture manipulation.
/// </summary>
/// <remarks>
/// This class is optional and not required for creating sprites. However,
/// it provides many useful methods to assist with texture manipulation.
/// </remarks>
/// <code>
/// var textureWrapper = new TextureWrapper(Content.Load("texture"));
/// var sprite = new Sprite(textureWrapper.Texture);
/// var texture2 = Content.Load("texture2");
/// textureWrapper.Blit(texture2, new Rectangle(0, 0, 100, 100), new Rectangle(0, 0, 100, 100));
/// </code>
public class TextureWrapper : IDisposable
{
  private static VertexBuffer quadVertexBuffer;
  private static int bufferWidth = -1;
  private static int bufferHeight = -1;
  private readonly PingPongBuffer buffer;

  private readonly GraphicsDevice device;
  private readonly SpriteBatch spriteBatch;
  private Texture2D texture;

  /// <summary>
  /// Initializes a new instance of the <see cref="TextureWrapper"/> class.
  /// </summary>
  /// <param name="device">The graphics device used to create the texture.</param>
  /// <param name="width">
  /// The initial width of the texture wrapper. This value is automatically adjusted when assigning a new texture.
  /// </param>
  /// <param name="height">
  /// The initial height of the texture wrapper. This value is automatically adjusted when assigning a new texture.
  /// </param>
  public TextureWrapper(GraphicsDevice device, int width, int height)
  {
    this.device = device;
    spriteBatch = new SpriteBatch(device);
    buffer = new PingPongBuffer(device, width, height);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TextureWrapper"/> class with the specified texture.
  /// </summary>
  /// <param name="device">The graphics device used to manage the texture.</param>
  /// <param name="texture">The texture to wrap.</param>
  public TextureWrapper(GraphicsDevice device, Texture2D texture) : this(device, texture.Width, texture.Height)
  {
    this.texture = texture;
  }

  /// <summary>
  /// The texture
  /// </summary>
  public Texture2D Texture
  {
    get => texture;
    set
    {
      texture = value;
      buffer.Assign(texture);
    }
  }

  /// <summary>
  /// the texture width.
  /// </summary>
  public int Width => texture.Width;

  /// <summary>
  /// the texture height.
  /// </summary>
  public int Height => texture.Height;

  public void Dispose()
  {
    buffer?.Dispose();
    texture?.Dispose();
    spriteBatch?.Dispose();
  }

  /// <summary>
  ///  copy a region of one texture to the current texture.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="sourceRect"></param>
  /// <param name="destRect"></param>
  public void Blit(Texture2D source, Rectangle sourceRect, Rectangle destRect)
  {
    if (texture == null) return;

    var renderTarget = buffer.Target;

    device.SetRenderTarget(renderTarget);
    device.Clear(Color.Transparent);

    spriteBatch.Begin();
    spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    spriteBatch.Draw(source, destRect, sourceRect, Color.White);
    spriteBatch.End();

    device.SetRenderTarget(null);
    buffer.Swap();
    texture = buffer.Source;
  }

  /// <summary>
  /// Blur the current texture.
  /// </summary>
  /// <remarks> this is a costly operation and should be used with moderation </remarks>
  /// <param name="blurAmount"></param>
  /// <param name="passes"></param>
  public void Blur(float blurAmount = 1f, int passes = 1)
  {
    
    if (texture == null || EffectManager.BlurEffect == null)
      return;
    

    InitializeBlur(device, Width, Height);

    var blurEffect = EffectManager.BlurEffect;
    var pixelSize = new Vector2(blurAmount / Width, blurAmount / Height);
    var viewProjection = Matrix.CreateOrthographicOffCenter(0, Width, 0, Height, -1, 1);

    var current = texture;

    for (int i = 0; i < passes; i++)
    {
      // Horizontal blur
      device.SetRenderTarget(buffer.Target);
      device.Clear(Color.Transparent);

      blurEffect.CurrentTechnique = blurEffect.Techniques["BlurHorizontal"];
      blurEffect.Parameters["g_ColorMap"]?.SetValue(current);
      blurEffect.Parameters["g_PixelSize"]?.SetValue(pixelSize);
      blurEffect.Parameters["g_WorldViewProj"]?.SetValue(viewProjection);
      blurEffect.Parameters["g_Color"]?.SetValue(Vector4.One);

      device.SetVertexBuffer(quadVertexBuffer);
      blurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
      device.SetVertexBuffer(null);

      device.SetRenderTarget(null);
      buffer.Swap();
      current = buffer.Source;

      // Vertical blur
      device.SetRenderTarget(buffer.Target);
      device.Clear(Color.Transparent);

      blurEffect.CurrentTechnique = blurEffect.Techniques["BlurVertical"];
      blurEffect.Parameters["g_ColorMap"]?.SetValue(current);
      blurEffect.Parameters["g_PixelSize"]?.SetValue(pixelSize);
      blurEffect.Parameters["g_WorldViewProj"]?.SetValue(viewProjection);
      blurEffect.Parameters["g_Color"]?.SetValue(Vector4.One);

      device.SetVertexBuffer(quadVertexBuffer);
      blurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
      device.SetVertexBuffer(null);

      device.SetRenderTarget(null);
      buffer.Swap();
      current = buffer.Source;
    }
    texture = buffer.Source;

  }


  /// <summary>
  /// create a deep clone of the current texture.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public Texture2D Clone()
  {
    if(texture == null) throw new InvalidOperationException("Texture is null");
    
    var clonedTexture = new RenderTarget2D(device, texture.Width, texture.Height);
    device.SetRenderTarget(clonedTexture);
    device.Clear(Color.Transparent);
    
    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
    spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
    spriteBatch.End();
    
    device.SetRenderTarget(null);
    return clonedTexture;
  }

  /// <summary>
  /// create a TextureWrapper with a deep clone of the current texture.
  /// </summary>
  /// <returns></returns>
  public TextureWrapper CloneTextureWrapper()
  {
    return new TextureWrapper(device, Clone());
  }
  
  /// <summary>
  /// return an array of frames Rectangle Data for animation based on a range of frames.
  /// </summary>
  /// <param name="frameSize"> the frame size</param>
  /// <param name="startIndex"></param>
  /// <param name="endIndex"></param>
  /// <returns></returns>
  public List<Rectangle> GetFrameRanges(Vector2 frameSize, int startIndex, int endIndex)
  {
    var frames = new List<Rectangle>();
    var columns = (int)(Width / frameSize.X);

    for (var i = startIndex; i <= endIndex; i++)
    {
      var row = i / columns;
      var col = i % columns;
      frames.Add(new Rectangle(
        (int)(col * frameSize.X),
        (int)(row * frameSize.Y),
        (int)frameSize.X,
        (int)frameSize.Y
      ));
    }
    return frames;
  }

  /// <summary>
  /// returns an array of frames Rectangle Data for animation based on a range of frames.
  /// </summary>
  /// <param name="frameWidth"></param>
  /// <param name="frameHeight"></param>
  /// <param name="startIndex"></param>
  /// <param name="endIndex"></param>
  /// <returns></returns>
  public List<Rectangle> GetFrameRanges(int frameWidth, int frameHeight, int startIndex, int endIndex)
  {
    var frameSize = new Vector2(frameWidth, frameHeight);
    return GetFrameRanges(frameSize, startIndex, endIndex);
  }
  
  static void InitializeBlur(GraphicsDevice device, int width, int height)
     {
       // Recreate if size changed
       if (quadVertexBuffer == null || bufferWidth != width || bufferHeight != height)
       {
         quadVertexBuffer?.Dispose();
   
         bufferWidth = width;
         bufferHeight = height;
   
         var vertices = new VertexPositionTexture[6];
         // Triangle 1
         vertices[0] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
         vertices[1] = new VertexPositionTexture(new Vector3(width, height, 0), new Vector2(1, 0));
         vertices[2] = new VertexPositionTexture(new Vector3(width, 0, 0), new Vector2(1, 1));
         // Triangle 2
         vertices[3] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
         vertices[4] = new VertexPositionTexture(new Vector3(0, height, 0), new Vector2(0, 0));
         vertices[5] = new VertexPositionTexture(new Vector3(width, height, 0), new Vector2(1, 0));
   
         quadVertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 6, BufferUsage.WriteOnly);
         quadVertexBuffer.SetData(vertices);
       }
     }
}
