using Hallowed.Core.Context;
using Hallowed.Core.Services;


namespace Sample.Context;

public class GameContext : IGameContext
{

  public IGameServiceProvider ServiceProvider { get; }
  public IGameVariableContext GameVariables { get; }

  public GameContext(IGameServiceProvider serviceProvider)
  {
    ServiceProvider = serviceProvider;
    GameVariables = new GameVariableContext();
  }
  public T GetService<T>() where T : class, IService
  {
    return ServiceProvider.GetService<T>();
  }
  public T GetService<T>(string name) where T : class, IService
  {
    return  ServiceProvider.GetService<T>(name); 
  }
}
