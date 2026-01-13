using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HallowedEditor.Source;

public partial class NavItem : ObservableObject
{
  public string Icon { get; set; }
  public string Label { get; set; }
    
  [ObservableProperty]
  private bool _isSelected;
    
  public required IRelayCommand SelectCommand { get; set; }
}
