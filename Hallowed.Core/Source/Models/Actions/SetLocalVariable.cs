using Hallowed.Core.Context;

namespace Hallowed.Core.Models.Actions;

public class SetLocalVariable : ActionBase
{

  public override string Name => nameof(SetLocalVariable);

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
      gameVariableContext.SetLocalVariable(variableContainer.Name, variableContainer.Value);
    }
  }
}
