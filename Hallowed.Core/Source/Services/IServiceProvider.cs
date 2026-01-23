namespace Hallowed.Core.Services;

public interface IServiceProvider : IDisposable
{
  void Initialize();
  void Update(float deltaSeconds);
  void Shutdown();
}
