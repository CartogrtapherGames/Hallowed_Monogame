using Hallowed.Core.Context;

namespace Hallowed.Core.Models.Actions;

public class SetGlobalVariable : ActionBase
{

  public override string Name => nameof(SetGlobalVariable);
  
  internal EditorData editorData { get; set; } = new EditorData();
  
  internal class EditorData
  {
    public List<VariableContainer> Variables { get; set; }
  }
  
  public override void OnAction(IGameContext gameContext)
  {
    var gameVariableContext = gameContext.GameVariables;
    foreach (var variableContainer in editorData.Variables )
    {
      gameVariableContext.SetGlobalVariable(variableContainer.Name, variableContainer.Value);
    }
  }
}
