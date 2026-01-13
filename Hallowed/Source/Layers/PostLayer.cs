using System.Collections.Generic;
using Hallowed.Graphics;
using Microsoft.Xna.Framework;

namespace Hallowed.Layers;

/// <summary>
/// Abstraction layer for post processing effects, it act as a way to render post process and clear them.
/// </summary>
public class PostLayer : SceneLayer
{

  private List<IPostProcess> postProcesses;

  public override void Initialize()
  {
    base.Initialize();
    postProcesses = [];
  }

  public void Add(IPostProcess postProcess)
  {
    postProcesses.Add(postProcess);
  }

  public void Remove(IPostProcess postProcess)
  {
    postProcesses.Remove(postProcess);
    postProcess.Dispose();
  }
  public override void Update(GameTime gameTime)
  {
    foreach (var postProcess in postProcesses)
    {
      postProcess.Update(gameTime);
    }
  }
  public override void Draw(GameTime gameTime)
  {
    foreach (var postProcess in postProcesses)
    {
      postProcess.Draw(gameTime);
    }
  }
}
