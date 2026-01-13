using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hallowed.Core;
using Hallowed.Graphics;
using Hallowed.Layers;
using Hallowed.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Scenes;

/// <summary>
/// The super class that handle all the logic and rendering for a scene.
/// </summary>
public abstract class SceneBase : IDisposable, ISceneAssets
{
  /// <summary>
  /// The Screen sprite used to handle screen fading
  /// </summary>
  /// <remarks>
  /// to not mistake with scene  transition as it is used while staying in the same scene.
  /// </remarks>
  protected ScreenSprite FadeSprite { get; private set; }
  
  protected RenderTarget2D BackgroundTexture { get; set; }
  
  protected GameTime LastUpdateTime { get; set; }
  protected GameTime LastDrawTime { get; set; }
  
  protected SpriteLayer RootLayer { get; set; } = new SpriteLayer();
  protected PostLayer PostLayer { get; set; } = new PostLayer();
  /// <summary>
  /// The SceneManager service instance which can be used to transition between scenes.
  /// </summary>
  public SceneManager SceneManager { get; internal set; }
  
  public AssetManager AssetManager { get; internal set; }
 
  /// <summary>
  /// Return whether or not this scene is currently active.
  /// </summary>
  public bool IsActive { get; internal set; }

  /// <summary>
  /// Flag to indicate whether this scene should be updated even when it is not active.
  /// This is useful for scenes that need to update their own state even when they are not visible.
  /// the default is false.
  /// </summary>
  public bool UpdateWhenInactive { get; set; } = false;

  /// <summary>
  /// Flag to indicate whether this scene should be drawn even when it is not active.
  /// This is useful for scenes that need to draw their own content even when they are not the active scene.
  /// However, this is recommended to cache the scene to a RenderTarget2D for performance reasons.
  /// The default is false.
  /// </summary>
  public bool DrawWhenInactive { get; set; } = false;


  /// <summary>
  /// The GameEngine instance.
  /// </summary>
  public GameEngine GameEngine { get; internal set; }

  /// <summary>
  /// The ContentManager instance. which is used to load assets.
  /// </summary>
  public ContentManager Content => GameEngine.Content;

  /// <summary>
  /// The GraphicsDevice instance.
  /// </summary>
  public GraphicsDevice GraphicsDevice => GameEngine.GraphicsDevice;
  
  public GraphicsDeviceManager Graphics => GameEngine.Graphics;

  /// <summary>
  /// The game services
  /// </summary>
  public GameServiceContainer Services => GameEngine.Services;

  /// <summary>
  /// The spritebatch instance used to draw content to the screen.
  /// </summary>
  public SpriteBatch SpriteBatch => GameEngine.SpriteBatch;
  
  public List<SceneLayer> Layers { get; private set; } = [];
  readonly Dictionary<string, SceneLayer> namedLayers = new Dictionary<string, SceneLayer>();
  HashSet<string> PersistentLayersQueue = [];
  
  bool layersNeedSorting;
  

  public virtual void Dispose()
  {
    SceneManager?.Dispose();
    ClearLayers();
  }


  public virtual void Initialize()
  {
    AddLayer(RootLayer, "Root");
    AddLayer(PostLayer, "Post");
    PostLayer.DrawOrder = 200;
    FadeSprite = new ScreenSprite(GraphicsDevice);
    PostLayer.Add( FadeSprite);
  }
  
  // called when the scene is ready to launch.
  /// <summary>
  /// the method called once the scene is ready to start and you need specific timing activation
  /// (like animation for openning ui or sequence etc)
  /// </summary>
  public virtual void Start()
  {
    
  }


  public void PreloadAssetsForNextScene(string manifestKey)
  {
    var manifestEntry = SceneManager.GetManifestEntry(manifestKey);
  }
  

  /// <summary>
  /// the class to preload and load your content such as sprite, effects, music.
  /// </summary>
  public virtual void LoadContent()
  {
    foreach (var layer in Layers)
    {
      layer.LoadContent();
    }
  }

  public virtual void UnloadContent()
  {
    foreach (var layer in Layers)
    {
      layer.UnloadContent();
    }
  }

  public virtual void Update(GameTime gameTime)
  {
    LastUpdateTime = gameTime;
    var enabledLayers = Layers.Where(l => l.IsEnabled).ToList();
    foreach (var layer in enabledLayers)
    {
      layer.Update(gameTime);
    }
  }

  private void RearrangeLayer()
  {
    var match = Layers.FirstOrDefault(l => l.IsDirty);
    if (match.IsDirty)
    {
      Layers.OrderBy(layer => layer.DrawOrder);
    }
  }

  public virtual void Draw(GameTime gameTime)
  {
    LastDrawTime = gameTime;

    if (layersNeedSorting)
    {
      Layers.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));
      layersNeedSorting = false;
    }
    foreach (var layer in Layers.Where(l => l.IsVisible))
    {
      layer.Draw(gameTime);
    }
  }

  #region Layer Management

  protected T AddLayer<T>(T layer, string name = null) where T : SceneLayer
  {
    layer.SetScene(this);
    Layers.Add(layer);

    Layers.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));

    if (!string.IsNullOrEmpty(name))
    {
      if (!namedLayers.TryAdd(name, layer))
      {
        throw new Exception($"The layer {name} already exists.");
      }
    }
    layer.Initialize();
    layersNeedSorting = true;
    return layer;
  }

  protected T GetLayer<T>(string name) where T : SceneLayer
  {
    if (namedLayers.TryGetValue(name, out var layer))
    {
      return layer as T;
    }
    return null;
  }

  protected T GetLayer<T>() where T : SceneLayer
  {
    return Layers.OfType<T>().FirstOrDefault();
  }

  protected void RemoveLayer(string name)
  {
    if (namedLayers.TryGetValue(name, out var layer))
    {
      Layers.Remove(layer);
      namedLayers.Remove(name);
      layer.UnloadContent();
      layer.Dispose();
    }
  }

  protected void RemoveLayer(SceneLayer layer)
  {
    if (!Layers.Remove(layer)) return;
    var entry = namedLayers.FirstOrDefault(kvp => kvp.Value == layer);
    if (entry.Value != null)
    {
      namedLayers.Remove(entry.Key);
    }
    layer.UnloadContent();
    layer.Dispose();
  }

  protected void ClearLayers()
  {
    foreach (var layer in Layers)
    {
      layer.UnloadContent();
      layer.Dispose();
    }
    Layers.Clear();
    namedLayers.Clear();
  }

  protected void DontDestroyOnLoad(string layerName)
  {
    PersistentLayersQueue.Add(layerName);
  }
  #endregion

  /// <summary>
  /// Fadeout the screen
  /// </summary>
  /// <param name="duration"></param>
  protected void FadeOut(float duration)
  {
    FadeSprite.FadeOut(duration, Color.Black);
  }

  /// <summary>
  /// Fadeout the screen
  /// </summary>
  /// <param name="duration"></param>
  /// <param name="color"></param>
  protected void FadeOut(float duration, Color color)
  {
    FadeSprite.FadeOut(duration, color);
  }

  /// <summary>
  /// fade in the screen
  /// </summary>
  /// <param name="duration"></param>
  protected void FadeIn(float duration)
  {
    FadeSprite.FadeIn(duration, Color.Black);
  }

  /// <summary>
  /// fadein the screen
  /// </summary>
  /// <param name="duration"></param>
  /// <param name="color"></param>
  protected void FadeIn(float duration, Color color)
  {
    FadeSprite.FadeIn(duration, color);
  }
  
  internal void MarkLayersDirty()
  {
    layersNeedSorting = true;
  }
}
