using Hallowed.Core.Services;


namespace Hallowed.Core.Context;

public interface IGameContext
{
  public IGameServiceProvider ServiceProvider { get; }
  public IGameVariableContext GameVariables { get; }
  public T GetService<T>() where T : class, IService;
  public T GetService<T>(string name) where T : class, IService;
}
