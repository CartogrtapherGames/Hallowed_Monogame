using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sample.Source.Context;

public class EngineContext : IContext
{

  public ScriptContext Parent { get; set; }

  public void Initialize()
  {
    Characters = [];
    Variables = [];
  }

  public void Update(GameTime gameTime)
  {
    throw new System.NotImplementedException();
  }

  public List<GameCharacter> Characters { get; private set; }

  public List<GameVariable> Variables { get; private set; }

}
public class GameCharacter
{
  public string Id { get; set; }

  public string DisplayName { get; set; }

  public Vector2 Position { get; set; }

}
public class GameVariable
{
  public string Name { get; set; }

  public int Value { get; set; }
}
