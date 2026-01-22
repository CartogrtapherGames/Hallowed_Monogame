using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hallowed.Editor.Models;

namespace Hallowed.Editor.ViewModels;

public partial class ItemsViewModel : ObservableObject
{

  [ObservableProperty]
  Item? selectedItem;

  public ItemsViewModel()
  {
    // Populate with sample items
    Items.Add(new Item(1)
      { Name = "Health Potion", Description = "Restores 50 HP", Price = 50, HpRestore = 50, Consumable = true });
    Items.Add(new Item(2)
      { Name = "Mana Potion", Description = "Restores 30 MP", Price = 40, MpRestore = 30, Consumable = true });
    Items.Add(new Item(3) { Name = "Old Key", Description = "A rusty old key", IsKeyItem = true });
    Items.Add(new Item(4) { Name = "Map Fragment", Description = "Part of an ancient map", IsKeyItem = true });
    Items.Add(new Item(5) { Name = "Antidote", Description = "Cures poison", Price = 25, Consumable = true });

    SelectedItem = Items.FirstOrDefault();
  }
  public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

  [RelayCommand]
  void AddNewItem()
  {
    var newId = Items.Count > 0 ? Items.Max(i => i.Id) + 1 : 1;
    var newItem = new Item(newId);
    Items.Add(newItem);
    SelectedItem = newItem;
  }

  [RelayCommand]
  void DeleteItem()
  {
    if (SelectedItem != null)
    {
      var currentIndex = Items.IndexOf(SelectedItem);
      Items.Remove(SelectedItem);

      // Recalculate all IDs to remove gaps
      for (var i = 0; i < Items.Count; i++)
      {
        Items[i].Id = i + 1;
      }

      // Select the item at the same position, or the last one if we deleted the last item
      if (Items.Count > 0)
      {
        SelectedItem = currentIndex < Items.Count
          ? Items[currentIndex]
          : Items[Items.Count - 1];
      }
      else
      {
        SelectedItem = null;
      }
    }
  }
}
