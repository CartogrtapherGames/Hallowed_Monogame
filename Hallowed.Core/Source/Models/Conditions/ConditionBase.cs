namespace Hallowed.Core.Models.Conditions;

public abstract class ConditionBase
{
  
  public IGameContext GameContext { get; set; }
  
  public string name;
  public abstract bool IsFulfilled();
  
}
