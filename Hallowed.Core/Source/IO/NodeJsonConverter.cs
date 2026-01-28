using Hallowed.Core.Models.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hallowed.Core.IO;

/// <summary>
/// Custom JSON converter for NodeBase that uses NodeRegistry for polymorphic deserialization.
/// Reads the "type" field from JSON and creates the correct concrete node type.
/// </summary>
public class NodeJsonConverter : JsonConverter<NodeBase>
{
  // Shared registry instance - created once and reused
  private static readonly NodeRegistry Registry = new NodeRegistry();
    
  public override NodeBase ReadJson(JsonReader reader, Type objectType, NodeBase? existingValue, 
    bool hasExistingValue, JsonSerializer serializer)
  {
    var jsonObject = JObject.Load(reader);
        
    // Read the "type" field from JSON
    var typeToken = jsonObject["type"];
    if (typeToken == null)
      throw new JsonSerializationException("Node must have a 'type' field");
        
    var nodeType = typeToken.ToObject<NodeType>();
        
    // Use the registry to create the correct node type
    NodeBase node;
    try
    {
      node = Registry.CreateNode(nodeType);
    }
    catch (ArgumentException ex)
    {
      throw new JsonSerializationException($"Failed to create node of type {nodeType}: {ex.Message}", ex);
    }
        
    // Populate the node's properties from JSON
    serializer.Populate(jsonObject.CreateReader(), node);
        
    return node;
  }

  public override void WriteJson(JsonWriter writer, NodeBase? value, JsonSerializer serializer)
  {
    if (value == null)
    {
      writer.WriteNull();
      return;
    }
        
    // Serialize the node normally - the Type property will be included automatically
    var jsonObject = JObject.FromObject(value, serializer);
    jsonObject.WriteTo(writer);
  }
}
