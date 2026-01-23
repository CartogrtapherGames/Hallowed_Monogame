using Hallowed.Core.Context;

namespace Hallowed.Core.Models.Actions;

public abstract class ActionBase
{
  public abstract string Name { get;}
  
  public abstract void OnAction(IGameContext gameContext);
}
