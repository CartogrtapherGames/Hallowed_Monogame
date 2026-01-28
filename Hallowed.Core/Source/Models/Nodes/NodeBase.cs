using Hallowed.Core.Context;
using Newtonsoft.Json;

namespace Hallowed.Core.Models.Nodes;


public enum NodeType
{
  Linear,
  Choice,
  Action
}


public abstract class NodeBase : BaseData 
{
  public abstract NodeType Type { get;}
  
  public abstract string Text();
  public abstract Guid FetchNextNode(int index = -1);
}
