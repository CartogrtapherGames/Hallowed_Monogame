namespace Hallowed.Core.Services;

public interface IGameServiceProvider : IDisposable
{
  void Initialize();
  void Update(float deltaSeconds);
  void Shutdown();

  T GetService<T>(string name) where T : class, IService;
  T GetService<T>() where T : class, IService;
  
  void RegisterService(string name,IService service);
  void RemoveService(string name);
  
}
