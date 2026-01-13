using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Hallowed.Graphics;
using Hallowed.IO;
using Hallowed.Manager;
using Hallowed.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens.Transitions;
using MonoGameGum;

namespace Hallowed.Core;

// TODO : Move most of the initialization to the SceneBoot
/// <summary>
/// The main class for the game that handles all game logic and rendering.
/// </summary>
public class GameEngine : Game
{
  /// <summary>
  /// The graphics device manager.
  /// </summary>
  public GraphicsDeviceManager Graphics { get; set; }

  /// <summary>
  /// the sprite batch for rendering most the children.
  /// </summary>
  public SpriteBatch SpriteBatch { get; private set; }

  readonly SceneManager sceneManager;
  readonly SceneBase startupScene;
  public readonly AssetManager assetManager;

  public GameEngine(SceneBase startupScene)
  {

    Graphics = new GraphicsDeviceManager(this);
    
    Content.RootDirectory = "Content";
    DataLoader.RootDirectory = "Content";
    this.startupScene = startupScene;
    IsMouseVisible = true;
    sceneManager = new SceneManager(this, GraphicsDevice);
    Components.Add(sceneManager);
    assetManager = new AssetManager(Content, "Content");
    Components.Add(assetManager);
  }

  protected override void Initialize()
  {

    Display.Initialize(Graphics, GraphicsDevice);
    Display.Resize(1920, 1080);
    sceneManager.ForceSetGraphicDevice(GraphicsDevice);
    // Later on maybe a boot screen might be a good idea.
    base.Initialize();
  }

  protected override void LoadContent()
  {
    SpriteBatch = new SpriteBatch(GraphicsDevice);
    
    assetManager.LoadManifest();
    var resources = assetManager.GetManifestEntry("ScenePlashScreen");
    foreach (var resource in resources)
    {
       assetManager.QueueAssetByExtension(resource);
    }
    assetManager.OnLoadingComplete += OnBackgroundLoadingCompleted;
    
    assetManager.LoadQueuedAssets();
  }

  void OnBackgroundLoadingCompleted(object sender, EventArgs e)
  {

    assetManager.OnLoadingComplete -= OnBackgroundLoadingCompleted;
    sceneManager.Goto(startupScene, new FadeTransition(GraphicsDevice, Color.Black, 1f)); 
  }
}

