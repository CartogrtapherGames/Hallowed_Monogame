using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Rendering;

public interface IRenderable
{
  Container Parent { get; set; }
  bool Visible { get; set; }
  void Update(GameTime gameTime);
  void Draw(SpriteBatch spriteBatch);
}
