using System;
using Hallowed.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Graphics;

public enum SpriteOrientation
{
  Normal,         // default
  FlipHorizontal, // flip the sprite horizontally
  FlipVertical,   // flip the sprite vertically
  Both            // flip both horizontally and vertically
}
/// <summary>
/// 
/// </summary>
public class Sprite : DisplayObject, IDisposable
{
  
  private Texture2D _texture; // maybe later we could do an bitmap Class? but in same time its not useful?
  private Color _color;
  private SpriteOrientation _orientation;



  Sprite() : base()
  {
    Frame = Rectangle.Empty;
    Color = Color.White;
    Origin = Vector2.Zero;
    _orientation = SpriteOrientation.Normal;
  }

  /// <summary>
  ///  The sprite constructor.
  /// </summary>
  /// <param name="texture"> the sprite texture</param>
  Sprite(Texture2D texture) : this()
  {
    _texture = texture;
  }

  /// <summary>
  /// The sprite texture.
  /// </summary>
  public virtual Texture2D Texture
  {
    get => _texture;
    set
    {
      _texture = value;
      Frame = Texture.Bounds;
    }
  }

  public Color Color
  {
    get => _color;
    set => _color = value;
  }

  public Rectangle Frame { get; set; } // used for animations or texture atlas.

  public SpriteOrientation Orientation
  {
    get => _orientation;
    set => _orientation = value;
  }

  public override Rectangle GetBounds()
  {
    if (Texture == null) return Rectangle.Empty;
    return new Rectangle(
      (int)-Origin.X,
      (int)-Origin.Y,
      Texture.Width,
      Texture.Height
    );
  }

  public void Update(GameTime gameTime)
  {
    base.Update(gameTime);
  }

  public void Draw(SpriteBatch spriteBatch)
  {
    if (Texture == null && !this.Visible) return;
    Vector2 scale;
    DecomposeMatrix2D(WorldTransform, out var position, out var rotation, out scale);
    spriteBatch.Draw(
      Texture,
      position,
      null,
      Color,
      rotation,
      Origin,
      scale,
      GetSpriteEffects(),
      0f
    );
  }

  protected virtual SpriteEffects GetSpriteEffects()
  {
    return _orientation switch
    {
      SpriteOrientation.Normal => SpriteEffects.None,
      SpriteOrientation.FlipHorizontal => SpriteEffects.FlipHorizontally,
      SpriteOrientation.FlipVertical => SpriteEffects.FlipVertically,
      SpriteOrientation.Both => SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
      _ => SpriteEffects.None
    };
  }
  protected override void Dispose(bool disposing)
  {
    _texture?.Dispose();
    base.Dispose(true); // we call it last to make sure that the texture is disposed before the object.
  }
}
