namespace Hallowed.Core.Models.Nodes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NodeTypeAttribute : Attribute
{
  public NodeType Type { get; }
  
  public NodeTypeAttribute(NodeType nodeType)
  {
    Type = nodeType;
  }
}
