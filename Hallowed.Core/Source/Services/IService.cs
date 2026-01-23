namespace Hallowed.Core.Services;

public interface IService : IDisposable
{
  IServiceProvider Provider {get; set;}
  public Task Initialize();
  public void Update(float deltaSeconds) {}
}
