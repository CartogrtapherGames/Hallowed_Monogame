using System;
using System.Collections.Generic;
using System.Linq;
using Hallowed.Core;
using Hallowed.Graphics;
using Hallowed.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens.Transitions;

namespace Hallowed.Manager;

/// <summary>
/// Manages the active scenes and transitions between them.
/// </summary>
/// <remarks>
/// This class was originally derived from the SceneManager/ScreenManager in MonoGame.Extended.
/// <br/>
/// See: <see href="https://github.com/craftworkgames/MonoGame.Extended">MonoGame.Extended Repository</see>
/// </remarks>
public class SceneManager : SimpleDrawableGameComponent
{

  #nullable disable
  readonly Stack<SceneBase> stack;
  SceneBase activeScene;
  Transition activeTransition;
  SceneBase[] cachedScenes;
  bool isSceneArrayDirty;
  GraphicsDevice graphicsDevice;
  readonly GameEngine gameEngine;
  // TODO : wouldnt putting this in the assetsManager make more senses??
  Dictionary<string, string[]> manifest;
  
  public RenderTarget2D BackgroundSnapshot { get; private set; }
  
  public IReadOnlyDictionary<string, string[]> Manifest => manifest;
  public GameTime LastUpdateTime { get; set; }
  public GameTime LastDrawTime { get; set; }

  /// <summary>
  /// The current active scene.
  /// </summary>
  public SceneBase ActiveScene => activeScene;

  /// <summary>
  /// The stack of scenes
  /// </summary>
  public IReadOnlyList<SceneBase> Scenes
  {
    get
    {
      if (!isSceneArrayDirty) return cachedScenes;
      cachedScenes = stack.Reverse().ToArray();
      isSceneArrayDirty = false;
      return cachedScenes;

    }
  }
  
  public SceneManager(GameEngine gameEngine, GraphicsDevice graphicsDevice)
  {
    stack = new Stack<SceneBase>();
    cachedScenes = [];
    isSceneArrayDirty = true;
    this.graphicsDevice = graphicsDevice;
    this.gameEngine = gameEngine;
  }

  /// <summary>
  /// Push a new scene onto the stack and make it the active scene.
  /// </summary>
  /// <param name="scene"> the scene to push</param>
  /// @todo : add the Transition Type system
  public void Push(SceneBase scene)
  {
    PreloadScene(scene);
    ActivateScene(scene);
    // activeScene.Start();
  }

  public void SetManifest(Dictionary<string, string[]> fileManifest)
  {
    manifest = fileManifest;
  }

  public string[] GetManifestEntry(string key)
  {
    // Check if manifest itself is null
    ArgumentNullException.ThrowIfNull(manifest, nameof(manifest));

    // Check if key is null or empty
    ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

    // Check if key exists in manifest
    if (!manifest.TryGetValue(key, out var entry))
      throw new KeyNotFoundException($"The manifest does not contain the key '{key}'");

    return entry;
  }

  public void PrintManifest()
  {
    foreach (var (key, value) in manifest)
    {
      Console.WriteLine($"{key} : {string.Join(", ", value)}");
    }
  }

  /// <summary>
  /// Push a new scene onto the stack and make it the active scene while using a transition.
  /// </summary>
  /// <param name="scene">The scene to push</param>
  /// <param name="transition"> The transition </param>
  public void Push(SceneBase scene, TransitionType transition)
  {
    if (activeTransition != null)
      return;

    PreloadScene(scene);

    var transitionInstance = transition.Create(graphicsDevice);
    activeTransition = transitionInstance;

    activeTransition.StateChanged += (EventHandler)((sender, args) => ActivateScene(scene));
    activeTransition.Completed += (EventHandler)((sender, args) =>
    {
      activeTransition.Dispose();
      activeTransition = null;

    });
  }


  /// <summary>
  /// Pop the top scene off the stack and unload it.
  /// </summary>
  public void Pop()
  {
    SceneBase result;
    if (!stack.TryPop(out result))
      return;
    result.IsActive = false;
    result.UnloadContent();
    result.Dispose();
    isSceneArrayDirty = true;
    if (!stack.TryPeek(out activeScene))
      return;
    activeScene.IsActive = true;
  } // TODO : test this to make sure it doesnt explode the scene lmao

  /// <summary>
  /// Pop the top scene off the stack and unload it while using a transition.
  /// </summary>
  /// <param name="transition"></param>
  public void Pop(TransitionType transition)
  {
    if (activeTransition != null)
      return;
    var transitionInstance = transition.Create(graphicsDevice);
    activeTransition = transitionInstance;
    activeTransition.StateChanged += (EventHandler)((sender, args) => Pop());
    activeTransition.Completed += (EventHandler)((sender, args) =>
    {
      activeTransition.Dispose();
      activeTransition = (Transition)null;
    });
  }

  /// <summary>
  /// Pop the last active scene and replace it with the specified scene.
  /// </summary>
  /// <remarks>
  /// To clear the stack use <see cref="ClearStack"/> before calling Goto.
  /// </remarks>
  /// <param name="scene">the next scene</param>
  public void Goto(SceneBase scene)
  {
    Pop();
    Push(scene);
  }

  /// <summary>
  /// Pop the last active scene and replace it with the specified scene while using a transition.
  /// </summary>
  /// <param name="scene"> The next scene</param>
  /// <param name="transition">the transition to use</param>
  /// <remarks>
  /// To clear the stack use <see cref="ClearStack"/> before calling Goto.
  /// </remarks>
  public void Goto(SceneBase scene, TransitionType transition)
  {
    if (activeTransition != null)
      return;

    PreloadScene(scene);

    var transitionInstance = transition.Create(graphicsDevice);
    activeTransition = transitionInstance;

    activeTransition.StateChanged += (EventHandler)((sender, args) => Goto(scene));
    activeTransition.Completed += (EventHandler)((sender, args) =>
    {
      activeTransition.Dispose();
      activeTransition = null;
    });
  }

  public void Goto(SceneBase scene, Transition transition)
  {
    if (activeTransition != null) return;

    PreloadScene(scene);

    activeTransition = transition;
    activeTransition.StateChanged += (EventHandler)((sender, args) => Goto(scene));
    activeTransition.Completed += (EventHandler)((sender, args) =>
    {
      activeTransition.Dispose();
      activeTransition = null;
    });
  }

  /// <summary>
  /// Clear the entire scene stack.
  /// </summary>
  ///<remarks>
  /// This does not unload the current scene until you push or goto a new scene.
  ///</remarks>
  public void ClearStack()
  {
    while (stack.TryPop(out var result))
    {
      result.IsActive = false;
      result.UnloadContent();
      result.Dispose();
    }
    activeScene = (SceneBase)null;
    isSceneArrayDirty = true;
  }

  /// <summary>
  /// Initialize the service
  /// </summary>
  public override void Initialize()
  {
    base.Initialize();
    activeScene?.Initialize();
  }

  /// <summary>
  /// Load the content for the scene.
  /// </summary>
  protected override void LoadContent()
  {
    base.LoadContent();

    if (!EffectManager.IsInitialized)
      EffectManager.LoadContent(gameEngine.Content);

    activeScene?.LoadContent();
  }

  /// <summary>
  /// Unload the content for the scene.
  /// </summary>
  protected override void UnloadContent()
  {
    base.UnloadContent();
    activeScene?.UnloadContent();
  }

  /// <summary>
  /// Update the active scene and any scene marked as UpdateWhenInactive.
  /// </summary>
  /// <remarks>
  ///  Do take in consideration of the cost of using updateWhenInactive on to many scene.
  /// </remarks>
  /// <param name="gameTime"></param>
  public override void Update(GameTime gameTime)
  {
    LastUpdateTime = gameTime;
    var scenes = Scenes;
    foreach (var scene in scenes)
    {
      if (scene.IsActive || scene.UpdateWhenInactive)
      {
        scene.Update(gameTime);
      }
    }
    activeTransition?.Update(gameTime);
  }

  /// <summary>
  /// Draw the active scene and any scene marked as DrawWhenInactive.
  /// </summary>
  /// <remarks>
  ///  Do take in consideration of the cost of using drawWhenInactive on to many scene.
  /// </remarks>
  /// <param name="gameTime"></param>
  public override void Draw(GameTime gameTime)
  {
    LastDrawTime = gameTime;
    var scenes = Scenes;
    foreach (var scene in scenes)
    {
      if (scene.IsActive || scene.DrawWhenInactive)
        scene.Draw(gameTime);
    }
    activeTransition?.Draw(gameTime);
  }
  
  /// <summary>
  /// Snap the current scene to a render target
  /// </summary>
  /// <param name="queueList"> the designed scene object</param>
  public void Snap(IEnumerable<IRenderable> queueList)
  {
    int w = graphicsDevice.Viewport.Width;
    int h = graphicsDevice.Viewport.Height;

    // Ensure RT exists and has the right size
    if (BackgroundSnapshot == null ||
        BackgroundSnapshot.Width != w ||
        BackgroundSnapshot.Height != h)
    {
      BackgroundSnapshot?.Dispose();
      BackgroundSnapshot = new RenderTarget2D(graphicsDevice, w, h);
    }

    // (Optional) ensure LastDrawTime is valid
    if (LastDrawTime == null)
      return; // or throw, or skip gametime draws

    // Set as current RT
    graphicsDevice.SetRenderTarget(BackgroundSnapshot);
    graphicsDevice.Clear(Color.Transparent);

    gameEngine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

    foreach (var renderable in queueList)
    {
      switch (renderable)
      {
        case IRenderableSpriteBatch sb:
          sb.Draw(gameEngine.SpriteBatch);
          break;

        case IRenderableGameTime gt:
          gt.Draw(LastDrawTime);
          break;
      }
    }
    gameEngine.SpriteBatch.End();
    // Restore main screen
    graphicsDevice.SetRenderTarget(null);
  }

  public void Snap(params IRenderable[] queueList) => Snap(new List<IRenderable>(queueList));

  public void Snap()
  {
    var queue = ActiveScene.Layers.Where(layer => layer.IsVisible);
    Snap(queue);
  }

  public override void Dispose()
  {
    base.Dispose();
    BackgroundSnapshot?.Dispose();
  }

  public void ForceSetGraphicDevice(GraphicsDevice gd)
  {
    graphicsDevice = gd;
  }

  /// <summary>
  /// Preload a scene without activating it or adding it to the stack.
  /// </summary>
  void PreloadScene(SceneBase scene)
  {
    ArgumentNullException.ThrowIfNull((object)scene, nameof(scene));

    // Only initialize if not already initialized
    if (scene.SceneManager != null) return;

    scene.SceneManager = this;
    scene.GameEngine = gameEngine;
    scene.AssetManager = gameEngine.assetManager;
    scene.Initialize();
    scene.LoadContent();
  }

  /// <summary>
  /// Activate a preloaded scene and add it to the stack.
  /// </summary>
  void ActivateScene(SceneBase scene)
  {
    if (activeScene != null)
      activeScene.IsActive = false;

    scene.IsActive = true;
    stack.Push(scene);
    activeScene = scene;
    isSceneArrayDirty = true;
    activeScene.Start(); // we start the scene here.
  }
}
