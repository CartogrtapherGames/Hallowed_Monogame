using System;
using System.Collections.Generic;
using Hallowed.Core;
using Hallowed.Graphics;
using Hallowed.Layers;
using Hallowed.Manager;
using Hallowed.Scenes;

namespace Sample.Scenes;

/// <summary>
/// The title screen of the game.
/// </summary>
public class TitleScreen : SceneBase
{

  Sprite backgroundSprite;
  GumLayer uiLayer;
  Audio titleMusic;

  public override void Initialize()
  {
    base.Initialize();

    InitializeBackground();
    InitializeUiLayer();
    InitializeButtons();
  }

  void InitializeBackground()
  {
    backgroundSprite = new Sprite(); 
    RootLayer.AddChild(backgroundSprite);
  }

  void InitializeUiLayer()
  {
    AddLayer(new GumLayer(Paths.GumProject));
    uiLayer = GetLayer<GumLayer>().LoadScreen("TitleScreen");
    uiLayer.DrawOrder = 101; // we always make the UI layer draw on top of everything else.
  }
  
  void InitializeButtons()
  {
    uiLayer
      .SetButton("CommandNewGame", OnCommandNewGame)
      .SetButton("CommandOptions", OnCommandOptions, OnCommandOptionsEnabled)
      .SetButton("CommandExit", OnCommandExit);
    uiLayer.Show();
  }


  public static List<string> GetRequiredAssets() => [];
  
  public override void Start()
  {
    base.Start();
    titleMusic.Play();
  }

  public override void LoadContent()
  {
    base.LoadContent();
    var texture = AssetManager.LoadSprite("Canyon");
   // var wrapper = new TextureWrapper(GraphicsDevice, texture);
   // wrapper.Blur(2, 2);
   backgroundSprite.Texture = texture;

   titleMusic = AssetManager.Load<Audio>("", "Theme1"); //new Audio("Content/Audio/Theme1.ogg");
  }
  
  
  void OnCommandNewGame()
  {
    // in this case I would setup everything and stop music etc.
    SceneManager.Goto(new SplashScreen(), TransitionType.FadeBlackSlow);
  //  SceneManager.Goto(new SceneMain(), TransitionType.FadeBlackSlow);
  }

  void OnCommandOptions()
  {
    uiLayer.Hide();
    SceneManager.Snap();
   // SceneManager.Blur();
    //  SceneManager.Push(new SceneOptions());
  }

  void OnCommandExit()
  {
    FadeSprite.Completed += OnFadeCompleteForExit;
    FadeOut(2f);
  }

  void OnFadeCompleteForExit(object sender, EventArgs args)
  {
    // Unsubscribe immediately
    FadeSprite.Completed -= OnFadeCompleteForExit;
    OnGameExit();
  }

   void OnGameExit()
  {
    GameEngine.Exit();
  }
   
   bool OnCommandOptionsEnabled()
   {
     return false;
   }

   public override void UnloadContent()
   {
     base.UnloadContent();
   }
}
