using Hallowed.Core.Services;
using IServiceProvider = System.IServiceProvider;

namespace Hallowed.Core.Context;

public interface IGameContext
{
  public IServiceProvider ServiceProvider { get; }
  public IGameVariableContext GameVariables { get; }
  public T GetService<T>() where T : class, IService;
  public T GetService<T>(string name) where T : class, IService;
}
