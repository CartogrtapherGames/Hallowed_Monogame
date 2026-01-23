namespace Hallowed.Core.Services;

public interface IService : IDisposable
{
  IServiceProvider Provider {get; set;}
  public void Initialize();
  public void Update(float deltaSeconds);
}
