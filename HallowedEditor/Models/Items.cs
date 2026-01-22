using CommunityToolkit.Mvvm.ComponentModel;

namespace Hallowed.Editor.Models;

public partial class Item : ObservableObject
{

  [ObservableProperty]
  bool consumable = true;

  [ObservableProperty]
  string description = "";

  [ObservableProperty]
  string effectScript = "";

  [ObservableProperty]
  int hpRestore;

  [ObservableProperty]
  string iconPath = "";
  [ObservableProperty]
  int id;

  [ObservableProperty]
  bool isKeyItem;

  [ObservableProperty]
  int mpRestore;

  [ObservableProperty]
  string name = "New Item";

  [ObservableProperty]
  int price;

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
