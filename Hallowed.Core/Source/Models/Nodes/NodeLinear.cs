namespace Hallowed.Core.Models.Nodes;

public class NodeLinear : NodeBase
{
  public override NodeType Type => NodeType.Linear;
  
  internal readonly EditorData editorData = new EditorData();

  internal class EditorData
  {
    public string text;
    public string nextNode;
  }
  
  // the system use this as nodes have different ways of displaying the text (such as *condition* node displaying different text based on a variable etc.
  public override string Text()
  {
    return editorData.text;
  }
  
  public override string FetchNextNode(int index = -1)
  {
    return  editorData.nextNode;
  }
  
}
