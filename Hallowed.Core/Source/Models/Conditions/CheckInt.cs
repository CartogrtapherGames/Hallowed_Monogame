using Hallowed.Core.Context;
using Newtonsoft.Json;

namespace Hallowed.Core.Models.Conditions;

// is there an easier way to do this?
public enum Operand
{

  EqualTo,
  GreaterThan,
  LesserThan,
  EqualOrGreaterThan,
  EqualOrLesserThan,

}
/// <summary>
/// check whether the variable match its value (int)
/// </summary>
public class CheckInt : ConditionBase
{

  [JsonProperty]
  internal EditorData editorData { get; } = new EditorData();

  public override string Name { get; protected set; } =  nameof(CheckInt);

  public CheckInt()
  {
  }
  
  internal class EditorData
  {
    public string Variable { get; set; } = "";
    public Operand Operand { get; set; } = Operand.EqualTo;
    public int ValueCondition { get; set; } = 0;
  }
  
  public override bool IsFulfilled(IGameContext gameContext)
  {
    // Check if variable exists first
    if (!gameContext.GameVariables.HasVariable(editorData.Variable))
      return false;

    try
    {
      var variable = gameContext.GameVariables.Find<int>(editorData.Variable);
      var valueCondition = editorData.ValueCondition;

      return editorData.Operand switch
      {
        Operand.EqualTo => variable == valueCondition,
        Operand.GreaterThan => variable > valueCondition,
        Operand.LesserThan => variable < valueCondition,
        Operand.EqualOrGreaterThan => variable >= valueCondition,
        Operand.EqualOrLesserThan => variable <= valueCondition,
        _ => false // Don't throw, just return false for unknown operands
      };
    }
    catch
    {
      // Variable exists but wrong type, or other error
      return false;
    }
  }

  public override string GetDescription()
  {
    var opSymbol = editorData.Operand switch
    {
      Operand.EqualTo => "==",
      Operand.GreaterThan => ">",
      Operand.LesserThan => "<",
      Operand.EqualOrGreaterThan => ">=",
      Operand.EqualOrLesserThan => "<=",
      _ => "?"
    };

    return $"condition type : {Name}, value : {editorData.Variable} {opSymbol} {editorData.ValueCondition}";
  }
}
