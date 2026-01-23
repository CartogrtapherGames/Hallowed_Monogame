using Hallowed.Core.Context;
using Hallowed.Core.Services;
using IServiceProvider = System.IServiceProvider;

namespace Sample.Context;

public class GameContext : IGameContext
{

  public IServiceProvider ServiceProvider { get; }
  public IGameVariableContext GameVariables { get; }
  public T GetService<T>() where T : class, IService
  {
    throw new System.NotImplementedException();
  }
  public T GetService<T>(string name) where T : class, IService
  {
    throw new System.NotImplementedException();
  }
}
