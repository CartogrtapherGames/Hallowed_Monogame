namespace Hallowed.Core.Services;

public interface IInventoryService : IService
{
  
  public bool HasItem(string itemId);
  
  public void  AddItem(string itemId);
  public void RemoveItem(string itemId);
}
