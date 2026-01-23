using Hallowed.Core.Context;
using Newtonsoft.Json;

namespace Hallowed.Core.Models.Nodes;


public enum NodeType
{
  Linear,
  Choice,
  Action
}


public abstract class NodeBase
{

  public Guid Guid { get; init; } = Guid.NewGuid();
  public string Id { get; set; } = "new node";
  public abstract NodeType Type { get;}
  
  public abstract string Text();
  public abstract string FetchNextNode(int index = -1);
}
