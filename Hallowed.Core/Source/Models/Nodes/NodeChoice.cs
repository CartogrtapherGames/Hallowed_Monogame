using Hallowed.Core.Context;
using Hallowed.Core.Models.Conditions;
using Newtonsoft.Json;

namespace Hallowed.Core.Models.Nodes;

[NodeType(NodeType.Choice)]
public class NodeChoice : NodeBase
{

  public override NodeType Type => NodeType.Choice;
  
  [JsonProperty]
  internal EditorData editorData = new EditorData();
  public class EditorData
  {
    public string Text { get; set; } = "";
    
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<ChoiceStruct> Choices { get; set; } = [];
  }
  
  public override string Text()
  {
    return editorData.Text;
  }
  
  public override Guid FetchNextNode(int index = -1)
  {
    throw new NotImplementedException();
    // return editorData.Choices[index].NextNode;
  }

  public bool IsChoiceAvailable(int index, IGameContext gameContext)
  {
    return editorData.Choices[index].IsFullFilled(gameContext);
  }

  public int ChoiceCount()
  {
    return editorData.Choices.Count;
  }

  public ChoiceStruct Choice(int index)
  {
    return editorData.Choices[index];
  }
}


public class ChoiceStruct
{

  public string Text { get; set; } = "";
  public string NextNode { get; set; } = "";
  public List<ConditionBase>? Conditions { get; set; }

  public bool IsFullFilled(IGameContext gameContext)
  {
    // No conditions = always available
    if (Conditions == null || Conditions.Count == 0)
      return true;
  
    // All conditions must be fulfilled
    return Conditions.All(condition => condition.IsFulfilled(gameContext));
  }
}
