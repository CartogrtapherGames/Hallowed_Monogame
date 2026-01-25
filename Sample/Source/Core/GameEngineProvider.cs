using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Hallowed.Core.Services;
using Sample.Context;
using Sample.Services;

namespace Sample.Core;

public class GameEngineProvider : IGameServiceProvider
{
  
  public IReadOnlyDictionary<string, GameServiceBase> Services => services;

  public GameContext Context { get; private set; }
  readonly Dictionary<string, GameServiceBase> services = new Dictionary<string, GameServiceBase>();
  
  public void Initialize()
  {
    Context = new GameContext(this);
    CreateServices();
    InitializeAllServices();
  }

  void CreateServices()
  {
    RegisterService("Inventory", new InventoryService(this));
  }

  void InitializeAllServices()
  {
    foreach (var service in Services.Values)
    {
      service.Initialize();
    }
  }
  
  public void Update(float deltaSeconds)
  {
    foreach (var service in Services)
    {
      if (!service.Value.Enabled) return;
      service.Value.Update(deltaSeconds);
    }
  }
  public void Shutdown()
  {
    throw new System.NotImplementedException();
  }
  public T GetService<T>(string name) where T : class, IService
  {
    if(!services.TryGetValue(name, out var service))
      throw new KeyNotFoundException(name);
    return service as T;
  }
  public T GetService<T>() where T : class, IService
  {
    foreach (var service in Services.Values)
    {
      if (service is T service1) return service1;
    }
    throw new KeyNotFoundException(typeof(T).Name);
  }
  
  public void RegisterService(string name, IService service)
  {
    RegisterService(name, service as GameServiceBase);
  }


  public void RegisterService(string name, GameServiceBase service)
  {
    if(!services.TryAdd(name, service)) 
      throw new Exception($"Service with name {name} already exists");
  }
  
  public void RemoveService(string name)
  {
    if(!services.ContainsKey(name))
      throw new KeyNotFoundException(name);
    services.Remove(name);
  }
  
  public void Dispose()
  {
    foreach (var service in Services.Values)
    {
      service.Dispose();
    }
    services.Clear();
  }
}
