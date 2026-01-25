namespace Hallowed.Core.Services;

public interface IService : IDisposable
{
  IGameServiceProvider Provider {get; set;}
  
  public bool Enabled {get; set;}
  public void Initialize();
  public void Update(float deltaSeconds) {}
}
