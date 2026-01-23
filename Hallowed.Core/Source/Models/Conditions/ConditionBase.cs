using Hallowed.Core.Context;

namespace Hallowed.Core.Models.Conditions;

public abstract class ConditionBase
{
  
  public string Name { get; protected set; } = "";
  public abstract bool IsFulfilled(IGameContext gameContext);
  
}
