using System;
using Microsoft.Xna.Framework;

namespace Hallowed.Graphics;

// used to differentiate the added child.
public interface IPostProcess : IDisposable
{
  void Update(GameTime gameTime);
  void Draw(GameTime gameTime);
}
