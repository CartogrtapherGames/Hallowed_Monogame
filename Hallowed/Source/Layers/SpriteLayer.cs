using System.Collections.Generic;
using Hallowed.Core;
using Hallowed.Ui;
using Microsoft.Xna.Framework;

namespace Hallowed.Layers;

/// <summary>
/// The layer class that group and handle sprites on the screen.
/// </summary>
public class SpriteLayer : SceneLayer
{
  
  public Container Root { get; protected set; }

  public List<DisplayObject> Children => Root.Children;
  
  public override void Initialize()
  {
    base.Initialize();
    Root = new Container();
  }
  
  public override void Update(GameTime gameTime)
  {
    Root.Update(gameTime);
  }
  public override void Draw(GameTime gameTime)
  {
    SpriteBatch.Begin();
    Root.Draw(SpriteBatch);
    SpriteBatch.End();
  }
  
  public void AddChild(DisplayObject child)
  {
    Root.AddChild(child);
  }
  
  public void Add(IEnumerable<DisplayObject> children)
  {
    Root.AddChild(children);
  }

  public void AddChild(params DisplayObject[] children)
  {
    Root.AddChild(children);
  }
  
  public void RemoveChild(DisplayObject child)
  {
    Root.RemoveChild(child);
  }

  public void Remove(IEnumerable<DisplayObject> children)
  {
    Root.RemoveChild(children);
  }

  public void RemoveChild(params DisplayObject[] children)
  {
    Root.RemoveChild(children);
  }
}
