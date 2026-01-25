using System.Threading.Tasks;
using Hallowed.Core.Services;

namespace Sample.Services;

public abstract class GameServiceBase(IGameServiceProvider provider) : IService
{
  
  public IGameServiceProvider Provider { get; set; } = provider;
  public bool Enabled { get; set; } = true;

  public virtual void Initialize()
  {

  }

  public virtual void Update(float delta)
  {
    
  }
  
  public virtual void Dispose()
  {
  }
}
