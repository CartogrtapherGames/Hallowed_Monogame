using System;
using System.Collections.Generic;
using System.IO;
using Hallowed.IO;

namespace Hallowed.Data;

/// <summary>
/// The class that manage all the data of the game.
/// </summary>
/// <remarks> The class is marked as partial so you can add your own data without changing the original code. </remarks>
/// <param name="directory"></param>
public partial class DataManager(string directory)
{
  public string Directory { get; set; } = directory;
    
  // Store Lazy instances, not the data itself
  private readonly Dictionary<string, Lazy<BaseData>> cache = new Dictionary<string, Lazy<BaseData>>();

  public DataSystem DataSystem => LoadData<DataSystem>("Data.json");

  private T LoadData<T>(string filename) where T : BaseData
  {
    var key = Path.HasExtension(filename) ? Path.GetFileNameWithoutExtension(filename) : filename;

    if(key is null) throw new ArgumentNullException(nameof(filename));
    if (!cache.TryGetValue(key, out var value))
    {
            value = new Lazy<BaseData>(() => DataLoader.LoadJson<T>(Path.Combine(Directory, filename)));
            cache[key] = value;
    }
        
    return (T)value.Value;
  }
    
  // Optional: Check if loaded without triggering load
  public bool IsLoaded(string filename)
  {
    var key = Path.HasExtension(filename) ? Path.GetFileNameWithoutExtension(filename) : filename;
    if (key is null) throw new ArgumentNullException(nameof(filename));
    return cache.ContainsKey(key) && cache[key].IsValueCreated;
  }
}

[System.Serializable]
public class BaseData {
  public string name;
}


[System.Serializable]
public class DataSystem : BaseData
{
  public string Version { get; init; }
  public bool UseGum {get; init;}
}
