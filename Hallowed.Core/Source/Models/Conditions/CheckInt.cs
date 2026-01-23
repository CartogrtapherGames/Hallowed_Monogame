using Hallowed.Core.Context;

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

  internal EditorData editorData { get; }
  internal class EditorData
  {
    public string Variable {get; set;} = "";
    public Operand Operand {get; set;} = Operand.EqualTo;
    public int ValueCondition {get; set;} = 0;
  }
  public override bool IsFulfilled(IGameContext gameContext)
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
      _ => throw new ArgumentOutOfRangeException()
    };
  }
}
