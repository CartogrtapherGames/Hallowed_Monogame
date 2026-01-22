namespace Hallowed.Core.Models.Nodes;


public enum NodeType
{
  Linear
}


public abstract class NodeBase
{

  public Guid Guid { get; init; } = Guid.NewGuid();
  public string Id { get; set; } = "new node";
  public NodeType Type { get; protected set; }


  public abstract string Text();
  public abstract string NextNode(int index = -1);
}
