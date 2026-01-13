using System;
using Hallowed.Core;
using Hallowed.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Layers;

/// <summary>
/// Abstract base class for scene layers. Layers represent distinct rendering/logic components
/// within a scene (e.g., background, game world, UI, particles).
/// This allows for modular scene composition and multiple rendering passes.
/// </summary>
public abstract class SceneLayer : IDisposable, IRenderableGameTime
{
  /// <summary>
  /// Reference to the parent scene that owns this layer.
  /// </summary>
  protected SceneBase Scene { get; private set; }

  /// <summary>
  /// Quick access to the GameEngine instance.
  /// </summary>
  protected GameEngine GameEngine => Scene.GameEngine;

  /// <summary>
  /// Quick access to the GraphicsDevice.
  /// </summary>
  protected GraphicsDevice GraphicsDevice => Scene.GraphicsDevice;

  /// <summary>
  /// Quick access to the ContentManager.
  /// </summary>
  protected ContentManager Content => Scene.Content;

  /// <summary>
  /// Quick access to the SpriteBatch.
  /// </summary>
  protected SpriteBatch SpriteBatch => Scene.SpriteBatch;

  /// <summary>
  /// Quick access to the GameServiceContainer.
  /// </summary>
  protected GameServiceContainer Services => Scene.Services;

  /// <summary>
  /// Determines whether this layer should be rendered.
  /// When false, Draw() will not be called.
  /// </summary>
  public bool IsVisible { get; set; } = true;

  /// <summary>
  /// Determines whether this layer should be updated.
  /// When false, Update() will not be called.
  /// </summary>
  public bool IsEnabled { get; set; } = true;

  public bool IsDirty { get; set; } = false;
  
  /// <summary>
  /// Controls the draw order of this layer. Lower values are drawn first (background),
  /// higher values are drawn last (foreground).
  /// Default is 0.
  /// </summary>
  public int DrawOrder
  {
    get => drawOrder;
    set
    {
      if (drawOrder != value)
      {
        drawOrder = value;
        Scene?.MarkLayersDirty();  // Notify scene
      }
    }
  }
  private int drawOrder;
  
  private bool disposed;

  /// <summary>
  /// Internal method called by SceneBase to set the parent scene reference.
  /// This is called before Initialize().
  /// </summary>
  /// <param name="scene">The parent scene.</param>
  internal void SetScene(SceneBase scene)
  {
    Scene = scene;
  }

  /// <summary>
  /// Called once when the layer is added to a scene.
  /// Override this to initialize your layer (load resources, set up state, etc.).
  /// </summary>
  public virtual void Initialize()
  {
  }

  /// <summary>
  /// Called when the scene's LoadContent is invoked.
  /// Override this to load layer-specific content.
  /// </summary>
  public virtual void LoadContent()
  {
  }

  /// <summary>
  /// Called when the scene's UnloadContent is invoked or when the layer is removed.
  /// Override this to clean up resources.
  /// </summary>
  public virtual void UnloadContent()
  {
  }

  /// <summary>
  /// Called every frame when the layer is enabled (IsEnabled = true).
  /// Override this to implement your layer's update logic.
  /// </summary>
  /// <param name="gameTime">Provides a snapshot of timing values.</param>
  public abstract void Update(GameTime gameTime);

  /// <summary>
  /// Called every frame when the layer is visible (IsVisible = true).
  /// Override this to implement your layer's rendering logic.
  /// </summary>
  /// <param name="gameTime">Provides a snapshot of timing values.</param>
  public abstract void Draw(GameTime gameTime);

  #region Fluent API Helper Methods

  /// <summary>
  /// Sets the draw order and returns this layer for method chaining.
  /// </summary>
  /// <param name="order">The draw order value.</param>
  /// <returns>This layer instance for chaining.</returns>
  public SceneLayer SetDrawOrder(int order)
  {
    DrawOrder = order;
    return this;
  }

  /// <summary>
  /// Shows this layer (sets IsVisible to true) and returns this layer for method chaining.
  /// </summary>
  /// <returns>This layer instance for chaining.</returns>
  public SceneLayer Show()
  {
    IsVisible = true;
    return this;
  }

  /// <summary>
  /// Hides this layer (sets IsVisible to false) and returns this layer for method chaining.
  /// </summary>
  /// <returns>This layer instance for chaining.</returns>
  public SceneLayer Hide()
  {
    IsVisible = false;
    return this;
  }

  /// <summary>
  /// Enables this layer (sets IsEnabled to true) and returns this layer for method chaining.
  /// </summary>
  /// <returns>This layer instance for chaining.</returns>
  public SceneLayer Enable()
  {
    IsEnabled = true;
    return this;
  }

  /// <summary>
  /// Disables this layer (sets IsEnabled to false) and returns this layer for method chaining.
  /// </summary>
  /// <returns>This layer instance for chaining.</returns>
  public SceneLayer Disable()
  {
    IsEnabled = false;
    return this;
  }

  #endregion

  /// <summary>
  /// Disposes of this layer.
  /// </summary>
  public virtual void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Disposes of this layer.
  /// </summary>
  /// <param name="disposing">true if disposing of managed resources</param>
  protected virtual void Dispose(bool disposing)
  {
    if (disposed) return;
    if (disposing) UnloadContent();
    disposed = true;
  }
}
