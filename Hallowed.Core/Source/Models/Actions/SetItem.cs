using Hallowed.Core.Context;
using Hallowed.Core.Services;

namespace Hallowed.Core.Models.Actions;

public enum ItemOperation
{
  Add,
  Remove,
}
public class SetItem : ActionBase
{

  public override string Name => nameof(SetItem);

  internal readonly EditorData editorData = new EditorData();

  internal class EditorData
  {
    public string ItemId { get; set; } = string.Empty;
    public ItemOperation Operation { get; set; } = ItemOperation.Add;
  }

  public override void OnAction(IGameContext gameContext)
  {
    var inventory = gameContext.GetService<IInventoryService>();
    var itemId = editorData.ItemId;
    switch (editorData.Operation)
    {
      case ItemOperation.Add:
        inventory.AddItem(itemId);
        break;
      case ItemOperation.Remove:
        inventory.RemoveItem(itemId);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    
  }
}
