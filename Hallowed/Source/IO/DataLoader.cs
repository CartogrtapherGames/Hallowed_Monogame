using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using JsonException = Newtonsoft.Json.JsonException;

namespace Hallowed.IO;

/// <summary>
/// the static class that allow loading and serializing files into multiple format such as JSON
/// </summary>
public static class DataLoader
{
  public static string RootDirectory { get; set; } = string.Empty;
  
  /// <summary>
  /// will dynamically load a json of a given name.
  /// </summary>
  /// <param name="filename"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  /// <exception cref="JsonSerializationException"></exception>
  /// <exception cref="JsonException"></exception>
  /// <exception cref="FileNotFoundException"></exception>
  /// <exception cref="Exception"></exception>
  public static T LoadJson<T>(string filename)
  {
    if (!Path.HasExtension(filename))
    {
      const string extension = ".json";
      Path.ChangeExtension(filename, extension);
    }
    try
    {
      using var stream = TitleContainer.OpenStream(RootDirectory + "/" + filename);
      using var reader = new StreamReader(stream);
      var jsonString = reader.ReadToEnd();
      var result = JsonConvert.DeserializeObject<T>(jsonString) ?? throw new JsonSerializationException($"Failed to deserialize {filename} to type {typeof(T).Name}");
      if (result == null)
      {
        throw new JsonSerializationException($"Failed to deserialize {filename} to type {typeof(T).Name}");
      }
      return result;
    }
    catch (JsonException ex)
    {
      throw new JsonException($"JSON parsing error in {filename}: {ex.Message}", ex);

    }
    catch (FileNotFoundException)
    {
      throw new FileNotFoundException($"Could not find file: {filename}");
    }
    catch (Exception ex)
    {
      throw new Exception($"Error loading {filename}: {ex.Message}", ex);
    }
  }

  // TODO : implement?
  public static void SaveToJson<T>(string filename, T obj)
  {
    
  }
}

