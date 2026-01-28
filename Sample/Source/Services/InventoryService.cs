using System.Collections.Generic;
using System.Linq;
using Hallowed.Core.Models.Objects;
using Hallowed.Core.Services;

namespace Sample.Services;

public class InventoryService(IGameServiceProvider provider) : GameServiceBase(provider), IInventoryService
{

  public int MaxStackSize => 99;
  
  List<GameItem> items;

  public override void Initialize()
  {
    items = [];
  }
  
  public bool HasItem(string itemId)
  {
    return items.Any(item => item.Id == itemId);
  }
  public void AddItem(string itemId, int amount = 1)
  {
    if (HasItem(itemId))
    {
      var item = GetItem(itemId);
      if (item.Amount < ItemMaxStackSize(itemId) )
      {
        item.Amount += amount;
      } 
      // else dont add as it at max stack but should add a UI ping to tell the system that you cant add more.
    }
    else
    {
      var item = CreateItem(itemId);
      if (!item.Consumable)
      {
        item.Amount = 1;
      }
      else
      {
        item.Amount +=  amount;
      }
      items.Add(item);
    }
  }

  GameItem CreateItem(string itemId)
  {
    // TODO : gotta implement the loading of data.
    var item = new GameItem();
    return item;
  }
  public void RemoveItem(string itemId, int amount = 1)
  {
    var item = GetItem(itemId);
    item.Amount -= amount;
    if (item.Amount <= 0)
    {
      items.Remove(item);
    }
  }
  
  public int ItemMaxStackSize(string itemId)
  {
    var item = GetItem(itemId);
    if (item == null) 
      throw new System.ArgumentException("Item not found");
    return item.Consumable ? MaxStackSize : 1;
  }


  public GameItem GetItem(string itemId)
  {
    return items.FirstOrDefault(item => item.Id == itemId);
  }
}
