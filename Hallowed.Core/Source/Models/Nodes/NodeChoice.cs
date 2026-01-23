using Hallowed.Core.Models.Conditions;

namespace Hallowed.Core.Models.Nodes;

public class NodeChoice : NodeBase
{


  internal EditorData editorData;
  public class EditorData
  {
    public required string Text { get; set; }
    public required List<ChoiceStruct> Choices { get; set; }
  }
  
  public override string Text()
  {
    return editorData.Text;
  }
  public override string FetchNextNode(int index = -1)
  {
    return editorData.Choices[index].Text;
  }
}


public class ChoiceStruct
{
 
  public string Text { get; set; }
  public List<ConditionBase> Conditions { get; set; }
}
