using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Hallowed.Editor.ViewModels;

public partial class StoryEditorViewModel : ObservableObject
{

  [ObservableProperty]
  TreeItemViewModel? selectedItem;

  public StoryEditorViewModel()
  {
    // Sample data
    var rootFolder = new TreeItemViewModel
    {
      Name = "Stories",
      IsFolder = true,
      IsExpanded = true
    };

    var chapter1 = new TreeItemViewModel
    {
      Name = "Chapter 1",
      IsFolder = true,
      IsExpanded = true
    };
    chapter1.Children.Add(new TreeItemViewModel { Name = "Intro Scene", IsFolder = false });
    chapter1.Children.Add(new TreeItemViewModel { Name = "Village", IsFolder = false });

    var chapter2 = new TreeItemViewModel
    {
      Name = "Chapter 2",
      IsFolder = true
    };
    chapter2.Children.Add(new TreeItemViewModel { Name = "Forest Path", IsFolder = false });

    rootFolder.Children.Add(chapter1);
    rootFolder.Children.Add(chapter2);

    Items.Add(rootFolder);
  }
  public ObservableCollection<TreeItemViewModel> Items { get; } = new ObservableCollection<TreeItemViewModel>();

  [RelayCommand]
  void AddFolder()
  {
    if (SelectedItem == null || !SelectedItem.IsFolder) return;

    var newFolder = new TreeItemViewModel
    {
      Name = "New Folder",
      IsFolder = true
    };
    SelectedItem.Children.Add(newFolder);
    SelectedItem.IsExpanded = true;
  }

  [RelayCommand]
  void AddStory()
  {
    if (SelectedItem == null || !SelectedItem.IsFolder) return;

    var newStory = new TreeItemViewModel
    {
      Name = "New Story",
      IsFolder = false
    };
    SelectedItem.Children.Add(newStory);
    SelectedItem.IsExpanded = true;
  }

  [RelayCommand]
  void DeleteItem()
  {
    // TODO: Implement delete logic
  }
}
public partial class TreeItemViewModel : ObservableObject
{

  [ObservableProperty]
  bool isExpanded;

  [ObservableProperty]
  bool isFolder;
  [ObservableProperty]
  string name = "";

  public ObservableCollection<TreeItemViewModel> Children { get; } = new ObservableCollection<TreeItemViewModel>();
}
