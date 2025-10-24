using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Hallowed.Rendering;

/// <summary>
/// The abstract base class for all renderable objects.
/// Every object that can be rendered should inherit from this class.
/// </summary>
public abstract class DisplayObject : IDisposable
{

  private bool _isDirty;
  private Vector2 _position;
  private Vector2 _scale;
  private float _rotation;
  private Vector2 _origin;
  private bool _visible;
  private bool _isDisposed;

  public bool IsDisposed => _isDisposed;

  /// <summary>
  /// the world matrix of the object.
  /// </summary>
  public Matrix WorldTransform { get; private set; }

  protected DisplayObject()
  {
    InitMembers();
    Parent = null;
    Children = [];
    WorldTransform = Matrix.Identity;
  }

  private void InitMembers()
  {
    _position = Vector2.Zero;
    _scale = Vector2.One;
    _rotation = 0;
    _origin = Vector2.Zero;
    _visible = true;
    _isDirty = true;
    _isDisposed = false; // we initially make it dirty on initialization
  }

  /// <summary>
  /// The Display object parent.
  /// </summary>
  public DisplayObject Parent { get; private set; }

  /// <summary>
  /// The objects children.
  /// </summary>
  public  List<DisplayObject> Children { get; protected set; }

  /// <summary>
  /// return whether or not the object will render to the screen. 
  /// </summary>
  public bool Visible
  {
    get => _visible;
    set
    {
      _visible = value;
      MarkDirty();
    }
  }

  public Vector2 Position
  {
    get => _position;
    set
    {
      _position = value;
      MarkDirty();
    }
  }

  /// <summary>
  /// The display object x position.
  /// <remarks>
  /// This property is an alias for setting the object X position.
  /// </remarks>
  /// </summary>
  public float X { get => _position.X; set => Position = new Vector2(value, Position.Y); }

  /// <summary>
  /// The display object y position.
  /// <remarks> This property is an alias for setting the object Y position.</remarks>
  /// </summary>
  public float Y { get => _position.Y; set => Position = new Vector2(Position.X, value); }

  /// <summary>
  /// The display object scale.
  /// </summary>
  public Vector2 Scale
  {
    get => _scale;

    set
    {
      _scale = value;
      MarkDirty();
    }
  }

  /// <summary>
  /// The display object rotation.
  /// </summary>
  public float Rotation
  {
    get => _rotation;
    set
    {
      _rotation = value;
      MarkDirty();
    }
  }

  /// <summary>
  /// The display object origin.
  /// </summary>
  public Vector2 Origin
  {
    get => _origin;
    set
    {
      _origin = value;
      MarkDirty();
    }
  }

  public virtual float Width
  {
    get => MathF.Abs(Scale.X * GetBounds().Width);
    set
    {
      float localWidth = GetBounds().Width;
      float sign = MathF.Sign(Scale.X);
      if (sign == 0) sign = 1;
      if (localWidth != 0)
      {
        Scale = new Vector2((value / localWidth) * sign, Scale.Y);
      }
      else
      {
        Scale = new Vector2(sign, Scale.Y);
      }
    }
  }

  public virtual float Height
  {
    get => MathF.Abs(Scale.Y * GetBounds().Height);
    set
    {
      float localHeight = GetBounds().Height;
      float sign = MathF.Sign(Scale.Y);
      if (sign == 0) sign = 1;

      if (localHeight != 0)
      {
        Scale = new Vector2(Scale.X, (value / localHeight) * sign);
      }
      else
      {
        Scale = new Vector2(Scale.X, sign);
      }
    }
  }

  // has to be implemented by the child class.
  public abstract Rectangle GetBounds();

  public Rectangle GetWorldBounds()
  {
    var local = GetBounds();

    // Transform bounds by WorldTransform
    Vector2[] corners =
    [
      new Vector2(local.Left, local.Top),
      new Vector2(local.Right, local.Top),
      new Vector2(local.Right, local.Bottom),
      new Vector2(local.Left, local.Bottom)
    ];

    float minX = float.MaxValue, minY = float.MaxValue;
    float maxX = float.MinValue, maxY = float.MinValue;

    foreach (var corner in corners)
    {
      var transformed = Vector2.Transform(corner, WorldTransform);
      minX = MathF.Min(minX, transformed.X);
      minY = MathF.Min(minY, transformed.Y);
      maxX = MathF.Max(maxX, transformed.X);
      maxY = MathF.Max(maxY, transformed.Y);
    }

    return new Rectangle(
      (int)minX, (int)minY,
      (int)(maxX - minX), (int)(maxY - minY)
    );
  }

  /// <summary>
  /// update the object and all of its children.
  /// </summary>
  /// <param name="gameTime"></param>
  public virtual void Update(GameTime gameTime)
  {
    if (_isDirty) UpdateWorldTransform();
    foreach (var child in Children)
    {
      child.Update(gameTime);
    }
  }

  /// <summary>
  /// draw the object and all of its children.
  /// </summary>
  /// <remarks>
  /// if an object or its children visible flag is set to false it wont
  /// be draw but still be updated.
  /// </remarks>
  /// <param name="spriteBatch"></param>
  public virtual void Draw(SpriteBatch spriteBatch)
  {
    if (!Visible) return;
    foreach (var child in Children)
    {
      child.Draw(spriteBatch);
    }
  }

  #region AddChild and RemoveChild overloads

  /// <summary>
  /// add a child to the end of the children list.
  /// </summary>
  /// <param name="child"></param>
  public virtual void AddChild(DisplayObject child)
  {
    if (child == null || child.IsDisposed)
      throw new InvalidOperationException("Cannot add a disposed child.");

    Children.Add(child);
    MarkDirty(); // in this case we marking it as dirty to force a recompute.
  }

  /// <summary>
  /// add a collection of children to the end of the children list.
  /// </summary>
  /// <param name="children"> the collection of display object to add</param>
  public virtual void AddChild(IEnumerable<DisplayObject> children)
  {
    foreach (var child in children)
    {
      AddChild(child);
    }
  }

  /// <summary>
  /// add a collection of children to the end of the children list.
  /// </summary>
  /// <param name="children"> the collection of display object to add</param>
  public virtual void AddChild(params DisplayObject[] children)
  {
    foreach (var child in children)
    {
      AddChild(child);
    }
  }

  /// <summary>
  /// Remove the specified child from the children list.
  /// </summary>
  /// <param name="child"> the child to remove </param>
  /// <param name="dispose"> whether or not to dispose the child</param>
  public virtual void RemoveChild(DisplayObject child, bool dispose = false)
  {
    if (!Children.Remove(child)) return;
    child.Parent = null;
    if (dispose) child.Dispose();
    if (!child.IsDisposed) child.MarkDirty();
    MarkDirty();
  }

  /// <summary>
  /// Remove the specified children from the children list.
  /// </summary>
  /// <param name="children"> the collection of children to remove</param>
  /// <param name="dispose"> whether or not to dispose the children</param>
  public virtual void RemoveChild(IEnumerable<DisplayObject> children, bool dispose = false)
  {
    foreach (var child in children)
    {
      RemoveChild(child, dispose);
    }
  }

  /// <summary>
  /// Remove the specified children from the children list.
  /// </summary>
  /// <param name="children"> the collection of childre to remove</param>
  /// <param name="dispose"> whether or not to dispose the children</param>
  public virtual void RemoveChild(bool dispose = false, params DisplayObject[] children)
  {
    foreach (var child in children)
    {
      RemoveChild(child);
    }
  }

  #endregion


  /// <summary>
  /// Mark the object and all its children as dirty and force a recompute of the their transforms.
  /// <remarks>
  ///  changing the transform of the object will trigger a recompute the transform.
  /// </remarks>
  /// </summary>
  public void MarkDirty()
  {
    _isDirty = true;
    foreach (var child in Children)
    {
      child.MarkDirty();
    }
  }

  #region Protected Methods

  protected virtual void UpdateWorldTransform()
  {
    Matrix localTransform =
      Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0) *
      Matrix.CreateScale(Scale.X, Scale.Y, 1) *
      Matrix.CreateRotationZ(Rotation) *
      Matrix.CreateTranslation(Position.X, Position.Y, 0);
    if (Parent != null)
    {
      WorldTransform = localTransform * Parent.WorldTransform;
    }
    else
    {
      // in this case, the object is the root of the scene graph
      WorldTransform = localTransform;
    }
    _isDirty = false;
  }

  protected void DecomposeMatrix2D(Matrix matrix, out Vector2 position, out float rotation, out Vector2 scale)
  {
    position = new Vector2(matrix.M41, matrix.M42);
    scale.X = new Vector2(matrix.M11, matrix.M12).Length();
    scale.Y = new Vector2(matrix.M21, matrix.M22).Length();
    rotation = (float)Math.Atan2(matrix.M12, matrix.M11);
  }

  #endregion

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);

  }

  protected virtual void Dispose(bool disposing)
  {
    if (_isDisposed) return;

    if (disposing)
    {
      Parent = null;
      foreach (var child in Children)
      {
        child.Dispose();
      }
      Children.Clear();
    }

    _isDisposed = true;
  }

  ~DisplayObject()
  {
    Dispose(false);
  }

}
