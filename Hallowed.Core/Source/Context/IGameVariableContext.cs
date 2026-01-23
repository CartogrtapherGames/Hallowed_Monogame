using Hallowed.Core.Models;

namespace Hallowed.Core.Context;

public interface IGameVariableContext
{
  public void SetLocalVariable<T>(string name, T value);
  public T GetLocalVariable<T>(string name);
  public void SetGlobalVariable<T>(string name, T value);
  public T GetGlobalVariable<T>(string name);
  
  public GameVariables LocalVariables { get; }
  public GameVariables GlobalVariables { get; }
  
  /// <summary>
  ///  Fetch a variable by checking its name from both the global and local scope.
  /// </summary>
  /// <param name="name"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public T Find<T>(string name);
  public bool HasVariable(string name);
}
