using Hallowed.Core.Models.Nodes;
using Newtonsoft.Json;

namespace Hallowed.Core.IO;

/// <summary>
/// Provides methods for serializing and deserializing nodes to/from JSON.
/// Uses NodeJsonConverter for polymorphic type handling.
/// </summary>
public static class NodeSerializer
{
  /// <summary>
  /// Deserialize a single node from JSON string.
  /// </summary>
  /// <param name="json">JSON string representing a node</param>
  /// <returns>The deserialized node (NodeLinear, NodeChoice, or NodeAction)</returns>
  public static NodeBase? Deserialize(string json)
  {
    return JsonConvert.DeserializeObject<NodeBase>(json, JsonConfig.Settings);
  }
    
  /// <summary>
  /// Deserialize a list of nodes from JSON string.
  /// Automatically creates the correct concrete type for each node based on its "type" field.
  /// </summary>
  /// <param name="json">JSON array string representing multiple nodes</param>
  /// <returns>List of nodes with correct concrete types</returns>
  public static List<NodeBase> DeserializeList(string json)
  {
    return JsonConvert.DeserializeObject<List<NodeBase>>(json, JsonConfig.Settings) 
      ?? new List<NodeBase>();
  }
    
  /// <summary>
  /// Serialize a single node to JSON string.
  /// </summary>
  /// <param name="node">The node to serialize</param>
  /// <returns>JSON string representation</returns>
  public static string Serialize(NodeBase node)
  {
    return JsonConvert.SerializeObject(node, JsonConfig.Settings);
  }
    
  /// <summary>
  /// Serialize a list of nodes to JSON string.
  /// </summary>
  /// <param name="nodes">The nodes to serialize</param>
  /// <returns>JSON array string</returns>
  public static string SerializeList(List<NodeBase> nodes)
  {
    return JsonConvert.SerializeObject(nodes, JsonConfig.Settings);
  }
}
