using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Hallowed.Editor.Source;

public partial class NavItem : ObservableObject
{

  [ObservableProperty]
  bool _isSelected;
  public string Icon { get; set; }
  public string Label { get; set; }

  public required IRelayCommand SelectCommand { get; set; }
}
