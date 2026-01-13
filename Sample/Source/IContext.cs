using Microsoft.Xna.Framework;

namespace Sample.Source;

public interface IContext
{
  ScriptContext Parent { get; set; }

  void Initialize();

  void Update(GameTime gameTime);
}
