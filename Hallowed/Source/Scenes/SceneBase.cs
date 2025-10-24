using Hallowed.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Scenes;

interface IScene
{
  void Initialize();
  void LoadContent();
  void Update(GameTime gameTime);
  void Draw(SpriteBatch spriteBatch);
}
public abstract class SceneBase : Container, IScene
{

  public void Initialize()
  {
    throw new System.NotImplementedException();
  }

  public void LoadContent()
  {
    throw new System.NotImplementedException();
  }
}
