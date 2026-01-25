namespace Hallowed.Core.Services;

public interface IInventoryService : IService
{
  
  public bool HasItem(string itemId);
  
  public void  AddItem(string itemId, int amount);
  public void RemoveItem(string itemId, int amount);
}
