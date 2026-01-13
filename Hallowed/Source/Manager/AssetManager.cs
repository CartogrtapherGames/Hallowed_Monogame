using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hallowed.Core;
using Hallowed.IO;
using Hallowed.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Hallowed.Manager;

/// <summary>
///  The static class that handle the content management of the scene.
///  It add extra QOL features to the default ContentManager.
/// </summary>
public partial class AssetManager(ContentManager content, string rootDirectory = "") : SimpleGameComponent
{

  public string RootDirectory { get; set; } = rootDirectory;
  public string AudioDirectory { get; set; } = "Audio";
  public string SpriteDirectory { get; set; } = "Sprites";
  
  public string DataDirectory { get; set; } = "Data";

  public Dictionary<string, string[]> ContentManifest => contentManifest;
  
  public bool Enabled { get; set; }

  ContentManager Content { get; set; } = content;
  Dictionary<string, Audio> audioCache = new Dictionary<string, Audio>();
  HashSet<string> loadedAssets = new HashSet<string>();
  Dictionary<string, string[]> contentManifest = new Dictionary<string, string[]>();
  
  // Unified queue system
  readonly Queue<Action> mainThreadQueue = new Queue<Action>();
  readonly Queue<Func<Task>> backgroundThreadQueue = new Queue<Func<Task>>();
  bool isProcessingAsync = false;
  bool isChunkLoading = false;
  int assetsPerFrame = 1;
  int loadedCount = 0;


  public event EventHandler OnLoadingComplete;

  public override void Initialize()
  {
  }

  #region Queue and Loading

  /// <summary>
  /// Queue an asset to be loaded later.
  /// </summary>
  public void Queue<T>(string directory, string filename) where T : class
  {
    backgroundThreadQueue.Enqueue(async () =>
    {
      if (typeof(T) == typeof(Audio))
      {
        var path = $"{RootDirectory}/{AudioDirectory}/{NormalizePath(directory, filename)}.ogg";
        var key = NormalizePath(directory, filename);

        if (audioCache.ContainsKey(key))
        {
          loadedCount++;
          return;
        }

        var audio = await Task.Run(() => new Audio(path));
        mainThreadQueue.Enqueue(() =>
        {
          audioCache[key] = audio;
          loadedCount++;
        });
      }
      else
      {
        // ContentManager calls must be on main thread
        mainThreadQueue.Enqueue(() =>
        {
          LoadViaContent<T>(directory, filename);
          loadedCount++;
        });
      }
    });
  }

  public void Queue<T>(params (string directory, string filename)[] assets) where T : class
  {
    foreach (var (directory, filename) in assets)
    {
      Queue<T>(directory, filename);
    }
  }

  public void Queue<T>(IEnumerable<(string directory, string filename)> assets) where T : class
  {
    foreach (var (directory, filename) in assets)
    {
      Queue<T>(directory, filename);
    }
  }

  public void QueueSprites(params string[] path)
  {
    foreach (var asset in path)
    {
      var subdirectory = Path.GetDirectoryName(asset);
      var directory = NormalizePath(SpriteDirectory, subdirectory);
      var filename = Path.GetFileNameWithoutExtension(asset);
      Queue<Texture2D>(directory, filename);
    }
  }

  public void QueueAudio(params string[] path)
  {
    foreach (var asset in path)
    {
      var subdirectory = Path.GetDirectoryName(asset);
      var directory = NormalizePath(AudioDirectory, subdirectory);
      var filename = Path.GetFileNameWithoutExtension(asset);
      Queue<Audio>(directory, filename);
    }
  }

  /// <summary>
  /// Load all queued assets.
  /// </summary>
  /// <param name="loadInBackground">If true, loads gradually over frames. If false, loads immediately.</param>
  /// <param name="numbOfAssetPerFrame">How many assets to load per frame when using background loading.</param>
  public void LoadQueuedAssets(bool loadInBackground = false, int numbOfAssetPerFrame = 1)
  {
    if (loadInBackground)
    {
      // Enable chunk loading
      isChunkLoading = true;
      this.assetsPerFrame = numbOfAssetPerFrame;
      loadedCount = 0;
      if (!isProcessingAsync && backgroundThreadQueue.Count > 0)
      {
        isProcessingAsync = true;
        _ = ProcessAssetAsync();
      }
    }
    else
    {
      // Load everything immediately
      while (backgroundThreadQueue.Count > 0)
      {
        var task = backgroundThreadQueue.Dequeue();
        task().Wait();
        while (mainThreadQueue.Count > 0)
        {
          mainThreadQueue.Dequeue()();
        }
      }
      loadedCount = 0;
      OnLoadingComplete?.Invoke(this, EventArgs.Empty);
    }
  }

  async Task ProcessAssetAsync()
  {
    while (backgroundThreadQueue.Count > 0)
    {
      var decodeTask = backgroundThreadQueue.Dequeue();
      await decodeTask();
    }
    isProcessingAsync = false;
  }

  /// <summary>
  /// Check if all queued assets are loaded.
  /// </summary>
  public bool IsLoadingComplete() =>
    mainThreadQueue.Count == 0 && backgroundThreadQueue.Count == 0 && !isProcessingAsync;

  /// <summary>
  /// Get loading progress (0.0 to 1.0).
  /// </summary>
  public float GetLoadProgress()
  {
    int totalQueued = backgroundThreadQueue.Count + mainThreadQueue.Count;
    var total = loadedCount + totalQueued;
    return total == 0 ? 1f : (float)loadedCount / total;
  }

  public override void Update(GameTime gameTime)
  {
    if (!isChunkLoading)
      return;

    // Process main thread work (GPU uploads, ContentManager calls)
    int loaded = 0;
    while (mainThreadQueue.Count > 0 && loaded < assetsPerFrame)
    {
      mainThreadQueue.Dequeue()();
      loaded++;
    }

    // Auto-disable when everything is done
    if (IsLoadingComplete())
    {
      isChunkLoading = false;
      loadedCount = 0;
      OnLoadingComplete?.Invoke(this, EventArgs.Empty);
    }
  }

  #endregion

  #region Load Methods

  public Task LoadAsync<T>(string directory, string filename) where T : class
  {
    return Task.Run(() => Load<T>(directory, filename));
  }

  public T Load<T>(string directory, string filename) where T : class
  {
    if (typeof(T) == typeof(Audio))
    {
      return LoadAudio(directory, filename) as T;
    }
    else
    {
      return LoadViaContent<T>(directory, filename);
    }
  }

  public T Load<T>(string filename) where T : class
  {
    if (typeof(T) == typeof(Audio))
    {
      return LoadAudio("", filename) as T;
    }
    else
    {
      return LoadViaContent<T>("", filename);
    }
  }


  // TODO : improve the integrations for JSON loading
  public T LoadJson<T>(string filename) where T : class
  {
    var file = NormalizePath(DataDirectory, filename);
    return DataLoader.LoadJson<T>(filename);
  }

  public void LoadManifest()
  {
    contentManifest = LoadJson<Dictionary<string, string[]>>("ContentManifest.json");
  }

  public string[] GetManifestEntry(string key)
  {
    // Check if manifest itself is null
    ArgumentNullException.ThrowIfNull(contentManifest, nameof(contentManifest));

    // Check if key is null or empty
    ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

    // Check if key exists in manifest
    return !contentManifest.TryGetValue(key, out var entry) ? throw new KeyNotFoundException($"The manifest does not contain the key '{key}'") : entry;

  }
  #endregion


  #region Extra Loading Methods

  public Texture2D LoadSprite(string filename, string subDirectory = "")
  {
    var path = NormalizePath(SpriteDirectory, NormalizePath(subDirectory, filename));
    return Content.Load<Texture2D>(path);
  }

  public Texture2D LoadTitle(string filename)
  {
    return LoadSprite(filename, "Title");
  }

  public Texture2D LoadSystem(string filename, string subDirectory = "")
  {
    return LoadSprite(filename, NormalizePath("System", subDirectory));
  }

  public void QueueAssetByExtension(string asset)
  {
    var extension = Path.GetExtension(asset);
    var filename = Path.GetFileNameWithoutExtension(asset);
    var directory = Path.GetDirectoryName(asset);

    switch (extension)
    {
      case ".png":
      case ".jpg":
        Queue<Texture2D>(directory, filename);
        break;
      case ".ogg":
        Queue<Audio>(directory, filename);
        break;
    }
  }

  public static string GetFilenameFromPath(string path)
  {
    return Path.GetFileNameWithoutExtension(path);
  }

  #endregion

  #region Load All

  public Dictionary<string, T> LoadAll<T>(string directory) where T : class
  {
    var fullPath = Path.Combine(Content.RootDirectory, directory);
    var dir = new DirectoryInfo(fullPath);

    if (!dir.Exists)
      throw new DirectoryNotFoundException($"Directory not found: {fullPath}");

    var result = new Dictionary<string, T>();

    // Filter by type-specific extensions
    string searchPattern = typeof(T) == typeof(Audio) ? "*.ogg" : "*.*";
    var files = dir.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

    foreach (var fileInfo in files)
    {
      var key = Path.GetFileNameWithoutExtension(fileInfo.Name);

      if (string.IsNullOrEmpty(key))
        continue;

      try
      {
        if (typeof(T) == typeof(Audio))
        {
          result[key] = LoadAudio(directory, key) as T;
        }
        else
        {
          result[key] = LoadViaContent<T>(directory, key);
        }
      }
      catch (Exception ex)
      {
        // Log but don't crash if one file fails
        Console.WriteLine($"Failed to load {fileInfo.Name}: {ex.Message}");
      }
    }

    return result;
  }

  #endregion


  #region Check Loaded Assets

  public bool IsLoaded<T>(string directory, string filename) where T : class
  {
    var key = NormalizePath(directory, filename);
    return typeof(T) == typeof(Audio) ? audioCache.ContainsKey(key) : loadedAssets.Contains(key);
  }

  public bool IsLoaded(string directory, string filename)
  {
    var key = NormalizePath(directory, filename);
    return audioCache.ContainsKey(key) || loadedAssets.Contains(key);
  }

  public IEnumerable<string> GetLoadedAudio()
  {
    return audioCache.Keys;
  }

  public IEnumerable<string> GetLoadedAssets()
  {
    return loadedAssets;
  }

  public IEnumerable<string> GetAllLoaded()
  {
    return audioCache.Keys.Concat(loadedAssets);
  }

  #endregion

  #region Internal Loading

  T LoadViaContent<T>(string directory, string filename)
  {
    var path = $"{NormalizePath(directory, filename)}";
    loadedAssets.Add(path);
    return Content.Load<T>(path);
  }

  Audio LoadAudio(string directory, string filename)
  {
    var path = $"{RootDirectory}/{AudioDirectory}/{NormalizePath(directory, filename)}.ogg";
    var key = NormalizePath(directory, filename);

    if (audioCache.TryGetValue(key, out var audio))
      return audio;

    audio = new Audio(path);
    audioCache.Add(key, audio);
    return audio;
  }

  string NormalizePath(string directory, string filename)
  {
    directory = (directory ?? string.Empty).Trim().Trim('/', '\\');
    filename = (filename ?? string.Empty).Trim().Trim('/', '\\');

    return directory.Length == 0
      ? filename
      : Path.Combine(directory, filename).Replace('\\', '/');
  }

  #endregion

  #region Dispose

  public override void Dispose()
  {
    base.Dispose();
    foreach (var audio in audioCache.Values)
    {
      audio.Dispose();
    }
    audioCache.Clear();
    loadedAssets.Clear();

    GC.SuppressFinalize(this);
  }

  #endregion

}
