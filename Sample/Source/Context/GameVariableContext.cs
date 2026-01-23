using Hallowed.Core.Context;
using Hallowed.Core.Models;

namespace Sample.Context;

/// <summary>
///  the game context that wraps both global and local variables and offers utility method for it.
/// </summary>
public class GameVariableContext : IGameVariableContext
{

  public GameVariables LocalVariables { get; private set; }
  public GameVariables GlobalVariables { get; private set; }

  public GameVariableContext()
  {
    LocalVariables = new GameVariables();
    GlobalVariables = new GameVariables();
  }
  
  public void SetLocalVariable<T>(string name, T value)
  {
    LocalVariables.Set(name, value);
  }
  
  public T GetLocalVariable<T>(string name)
  {
    return LocalVariables.Get<T>(name);
  }
  
  public void SetGlobalVariable<T>(string name, T value)
  {
    GlobalVariables.Set(name, value);
  }
  
  public T GetGlobalVariable<T>(string name)
  {
    return GlobalVariables.Get<T>(name);
  }

  public T Find<T>(string name)
  {
    if (LocalVariables.HasVariable(name))
    {
      return LocalVariables.Get<T>(name);
    } else if  (GlobalVariables.HasVariable(name))
    {
      return GlobalVariables.Get<T>(name);
    }
    return default;
  }
  public bool HasVariable(string name)
  {
    return GlobalVariables.HasVariable(name) ||  LocalVariables.HasVariable(name);
  }

  public string Save()
  {
    
  }
}
