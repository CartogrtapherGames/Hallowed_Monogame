using System.Reflection;

namespace Hallowed.Core.Models.Nodes;

/// <summary>
/// Registry that automatically discovers and manages all node types in the assembly.
/// Scans for classes marked with [NodeType] attribute and provides factory methods.
/// </summary>
public class NodeRegistry
{
    private readonly Dictionary<NodeType, Type> nodeTypes = new();
    
    /// <summary>
    /// Creates a registry and scans the assembly for node types.
    /// </summary>
    public NodeRegistry()
    {
        ScanAndRegister();
    }
    
    /// <summary>
    /// Scans the Hallowed.Core assembly for classes marked with [NodeType] attribute.
    /// </summary>
    private void ScanAndRegister()
    {
        var assembly = typeof(NodeBase).Assembly;
        
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(NodeBase)));
        
        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<NodeTypeAttribute>();
            
            if (attr == null)
            {
                // Warn about nodes without the attribute
                Console.WriteLine($"Warning: {type.Name} inherits NodeBase but has no [NodeType] attribute");
                continue;
            }
            
            // Validate parameterless constructor
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                Console.WriteLine($"Error: Node type {type.Name} must have a parameterless constructor");
                continue;
            }
            
            // Register the node type
            if (nodeTypes.ContainsKey(attr.Type))
            {
                Console.WriteLine($"Warning: Duplicate node type {attr.Type} - {type.Name} conflicts with {nodeTypes[attr.Type].Name}");
                continue;
            }
            
            nodeTypes[attr.Type] = type;
            Console.WriteLine($"Registered node: {attr.Type} -> {type.Name}");
        }
    }
    
    /// <summary>
    /// Creates a new instance of a node based on its NodeType enum value.
    /// </summary>
    /// <param name="type">The NodeType enum value</param>
    /// <returns>A new instance of the corresponding node class</returns>
    /// <exception cref="ArgumentException">Thrown if the node type is not registered</exception>
    public NodeBase CreateNode(NodeType type)
    {
        if (!nodeTypes.TryGetValue(type, out var nodeClass))
        {
            throw new ArgumentException($"Unknown node type: {type}. Did you forget to add [NodeType({type})] to the class?");
        }
        
        return (NodeBase)Activator.CreateInstance(nodeClass)!;
    }
    
    /// <summary>
    /// Gets the Type (class) associated with a NodeType enum value.
    /// </summary>
    public Type? GetNodeClass(NodeType type)
    {
        nodeTypes.TryGetValue(type, out var nodeClass);
        return nodeClass;
    }
    
    /// <summary>
    /// Checks if a NodeType is registered.
    /// </summary>
    public bool HasNodeType(NodeType type) => nodeTypes.ContainsKey(type);
    
    /// <summary>
    /// Gets all registered NodeType enum values.
    /// </summary>
    public IEnumerable<NodeType> GetAllNodeTypes() => nodeTypes.Keys;
    
    /// <summary>
    /// Gets the count of registered node types.
    /// </summary>
    public int Count => nodeTypes.Count;
}