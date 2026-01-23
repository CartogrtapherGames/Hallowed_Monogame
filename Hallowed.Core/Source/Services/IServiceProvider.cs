namespace Hallowed.Core.Services;

public interface IServiceProvider : IDisposable
{
  Task Initialize();
  void Update(float deltaSeconds);
  void Shutdown();

  IService GetService(string name);
  IService GetService<T>() where T : IService;
  
  void RegisterService(IService service);
}
