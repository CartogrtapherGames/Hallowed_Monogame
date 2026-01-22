using CommunityToolkit.Mvvm.ComponentModel;

namespace Hallowed.Editor.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
  
  public virtual bool IsLoaded { get; protected set; }
  
  public abstract void LoadFromProject();
  
  public abstract void SaveToProject();
}
