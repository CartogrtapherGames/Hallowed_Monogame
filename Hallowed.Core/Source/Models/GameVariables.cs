using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hallowed.Core.Models;

public enum VariableType
{
  Int,
  Float,
  Double,
  Bool,
  String,
}
[Serializable]
public class VariableContainer
{

  public required string Name { get; set; }
  public required VariableType Type { get; set; }
  public required JToken Value { get; set; }
}

/// <summary>
/// The class that handle game variables. 
/// </summary>
public class GameVariables : BaseData
{

  List<VariableContainer> variables = [];

  // Map C# types to PropertyType enum
  static readonly Dictionary<Type, VariableType> TypeMap = new Dictionary<Type, VariableType>
  {
    { typeof(int), VariableType.Int },
    { typeof(float), VariableType.Float },
    { typeof(double), VariableType.Double },
    { typeof(bool), VariableType.Bool },
    { typeof(string), VariableType.String },
  };


  public GameVariables(string name)
  {
    Id = name;
  }
  
  public void Add<T>(string name, JToken value)
  {
    if (HasVariable(name)) return;
        
    var container = new VariableContainer()
    {
      Name = name,
      Type = GetVariableType<T>(),
      Value = value
    };
    variables.Add(container);
  }
  
  /// <summary>
  /// set the existing variable.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="value"></param>
  /// <typeparam name="T"></typeparam>
  /// <exception cref="KeyNotFoundException"></exception>
  /// <exception cref="ArgumentException"></exception>
  public void Set<T>(string name, T value)
  {
    var container = variables.FirstOrDefault(v => v.Name == name);
        
    if (container == null)
      throw new KeyNotFoundException($"Variable '{name}' not found");
            
    var providedType = GetVariableType<T>();

    if (providedType != container.Type)
    {
      throw new ArgumentException(
        $"Type mismatch for variable '{name}': " +
        $"expected {container.Type}, got {providedType}"
      );
    }

    if (value != null) 
      container.Value = JToken.FromObject(value);
  }

  public void Set(string name, JToken value)
  {
    var container = variables.FirstOrDefault(v => v.Name == name)
      ?? throw new KeyNotFoundException($"Variable '{name}' not found");

    container.Value = value;
  }


  /// <summary>
  /// fetch an variable.
  /// </summary>
  /// <param name="name"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  /// <exception cref="InvalidCastException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  public T Get<T>(string name)
  {
    var container = variables.FirstOrDefault(v => v.Name == name);
        
    if (container == null)
      throw new KeyNotFoundException($"Variable '{name}' not found");
            
    var expectedType = GetVariableType<T>();

    if (expectedType != container.Type)
    {
      throw new InvalidCastException(
        $"Variable '{name}' is type {container.Type}, cannot cast to {expectedType}"
      );
    }

    var value = container.Value.ToObject<T>();
    return value ?? throw new InvalidOperationException($"Variable '{name}' deserialized to null");
  }

  /// <summary>
  /// serialize variables to json string
  /// </summary>
  /// <returns></returns>
  public string Serialize()
  {
    var settings = new JsonSerializerSettings
    {
      ContractResolver = new CamelCasePropertyNamesContractResolver(),
      Formatting = Formatting.Indented,
      NullValueHandling = NullValueHandling.Ignore
    };

    return JsonConvert.SerializeObject(variables, settings);
  }

  /// <summary>
  /// deserialize variables 
  /// </summary>
  /// <param name="json"></param>
  /// <exception cref="InvalidOperationException"></exception>
  public void Deserialize(string json)
  {
    var result = JsonConvert.DeserializeObject<List<VariableContainer>>(json);
    variables = result ?? throw new InvalidOperationException("Failed to deserialize variables");
  }

  public bool HasVariable(string name)
  {
    return variables.Any(v => v.Name == name);
  }

  VariableType GetVariableType<T>()
  {
    var type = typeof(T);
    if (TypeMap.TryGetValue(type, out var propType))
      return propType;

    throw new NotSupportedException($"Type {type.Name} is not supported");
  }



}
