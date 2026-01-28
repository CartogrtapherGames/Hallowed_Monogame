using Newtonsoft.Json;

namespace Hallowed.Core.Models.Nodes;

[NodeType(NodeType.Linear)]
public class NodeLinear : NodeBase
{
  public override NodeType Type => NodeType.Linear;
  
  [JsonProperty]
  internal EditorData editorData = new EditorData();

  internal class EditorData
  {
    public string Text { get; set; } = "";
    public Guid NextNode  { get; set; } =  Guid.Empty;
  }
  
  // the system use this as nodes have different ways of displaying the text (such as *condition* node displaying different text based on a variable etc.
  public override string Text()
  {
    return editorData.Text;
  }

  public override Guid FetchNextNode(int index = -1)
  {
    return  editorData.NextNode;
  }

  
}
