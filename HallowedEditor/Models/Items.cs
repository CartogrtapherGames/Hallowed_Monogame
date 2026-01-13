using CommunityToolkit.Mvvm.ComponentModel;

namespace HallowedEditor.Models;

public partial class Item : ObservableObject
{
  [ObservableProperty]
  private int id;
    
  [ObservableProperty]
  private string name = "New Item";
    
  [ObservableProperty]
  private string description = "";
    
  [ObservableProperty]
  private string iconPath = "";
    
  [ObservableProperty]
  private bool isKeyItem = false;
    
  [ObservableProperty]
  private int price = 0;
    
  [ObservableProperty]
  private bool consumable = true;
    
  [ObservableProperty]
  private int hpRestore = 0;
    
  [ObservableProperty]
  private int mpRestore = 0;
    
  [ObservableProperty]
  private string effectScript = "";
    
  public Item()
  {
  }
    
  public Item(int id)
  {
    Id = id;
    Name = $"Item {id:D4}";
    Price = 10;
    Consumable = true;
  }
}
