using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Hallowed.Scenes;

public class SceneManager : IGameComponent
{

  //TODO : we dont need a stack since we only keep one scene at a time.
  private Stack<SceneBase> _scenes;
  
  private Game _game;
  
  public SceneManager(Game game)
  {
    _game = game;
  }
  
  public void Initialize()
  {
  }
  
  public SceneBase CurrentScene => _scenes.Peek();


  public void Goto(SceneBase scene, bool clearStack = false)
  {
    if (CurrentScene == scene) throw new Exception("The scene is already active.");
    // todo : implement Stack if necessary.
  }


  public void Terminate()
  {
    
  }
}
