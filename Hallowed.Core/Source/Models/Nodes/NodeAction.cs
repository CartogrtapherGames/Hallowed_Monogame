using Hallowed.Core.Context;
using Hallowed.Core.Models.Actions;
using Newtonsoft.Json;

namespace Hallowed.Core.Models.Nodes;

[NodeType(NodeType.Action)]
public class NodeAction : NodeBase
{

  public override NodeType Type  => NodeType.Action;
  
  [JsonProperty]
  internal EditorData editorData { get; set; } = new EditorData();
  internal class EditorData
  {
    public string Text { get; set; } = "";
    public List<ActionBase> Actions { get; set; } = [];
    public string NextNode { get; set; } = "";
  }


  public override string Text()
  {
    return editorData.Text;
  }

  public override Guid FetchNextNode(int index = -1)
  {
    throw new NotImplementedException();
  }

  public void OnExecute(IGameContext context)
  {
    foreach (var action in editorData.Actions)
    {
      action.OnAction(context);
    }
  }
}
