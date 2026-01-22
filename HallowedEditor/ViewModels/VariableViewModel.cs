using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hallowed.Core.IO;
using Hallowed.Core.Models;
using Hallowed.Editor.Services;
using Newtonsoft.Json;

namespace Hallowed.Editor.ViewModels;

public partial class VariablesViewModel : ViewModelBase
{
  [ObservableProperty]
  bool isLocalTabSelected = true;

  [ObservableProperty]
  VariableItem? selectedGlobalVariable;


  [ObservableProperty]
  VariableItem? selectedLocalVariable;

  public VariablesViewModel()
  {
    SelectedLocalVariable = LocalVariables.FirstOrDefault();
    SelectedGlobalVariable = GlobalVariables.FirstOrDefault();
  }
  
  public ObservableCollection<VariableItem> LocalVariables { get; private set; } = new ObservableCollection<VariableItem>();
  public ObservableCollection<VariableItem> GlobalVariables { get; private set; } = new ObservableCollection<VariableItem>();

  public Array VariableTypes { get; } = Enum.GetValues<VariableType>();

  [RelayCommand]
  void AddLocalVariable()
  {
    var newId = LocalVariables.Count > 0 ? LocalVariables.Max(v => v.Id) + 1 : 1;
    var newVar = new VariableItem
    {
      Id = newId,
      Name = "new_variable",
      Type = VariableType.Int,
      Value = "0"
    };
    LocalVariables.Add(newVar);
    SelectedLocalVariable = newVar;
  }

  [RelayCommand]
  void AddGlobalVariable()
  {
    var newId = GlobalVariables.Count > 0 ? GlobalVariables.Max(v => v.Id) + 1 : 1;
    var newVar = new VariableItem
    {
      Id = newId,
      Name = "new_global",
      Type = VariableType.Int,
      Value = "0"
    };
    GlobalVariables.Add(newVar);
    SelectedGlobalVariable = newVar;
  }

  [RelayCommand]
  void DeleteLocalVariable()
  {
    if (SelectedLocalVariable != null)
    {
      var currentIndex = LocalVariables.IndexOf(SelectedLocalVariable);
      LocalVariables.Remove(SelectedLocalVariable);

      // Recalculate IDs
      for (var i = 0; i < LocalVariables.Count; i++)
      {
        LocalVariables[i].Id = i + 1;
      }

      SelectedLocalVariable = currentIndex < LocalVariables.Count
        ? LocalVariables[currentIndex]
        : LocalVariables.LastOrDefault();
    }
  }

  [RelayCommand]
  void DeleteGlobalVariable()
  {
    if (SelectedGlobalVariable != null)
    {
      var currentIndex = GlobalVariables.IndexOf(SelectedGlobalVariable);
      GlobalVariables.Remove(SelectedGlobalVariable);

      // Recalculate IDs
      for (var i = 0; i < GlobalVariables.Count; i++)
      {
        GlobalVariables[i].Id = i + 1;
      }

      SelectedGlobalVariable = currentIndex < GlobalVariables.Count
        ? GlobalVariables[currentIndex]
        : GlobalVariables.LastOrDefault();
    }
  }
  public override void LoadFromProject()
  {
    if (IsLoaded) return;
    
    LocalVariables = LoadVariableCollection("localVariables.json");
    GlobalVariables = LoadVariableCollection("globalVariables.json");
  }

  ObservableCollection<VariableItem> LoadVariableCollection(string filename)
  {
    if (!ProjectManager.DataFileExists(filename))
    {
      return [];
    }

    try
    {
      var filePath = ProjectManager.GetDataPath(filename);
      var json = File.ReadAllText(filePath);
        
      return JsonConvert.DeserializeObject<ObservableCollection<VariableItem>>(json) 
        ?? new ObservableCollection<VariableItem>();
    }
    catch (JsonException ex)
    {
      // Log the error or notify user
      Debug.WriteLine($"Failed to deserialize {filename}: {ex.Message}");
      return new ObservableCollection<VariableItem>();
    }
  }
  
  public override void SaveToProject()
  {
    SaveVariableCollection("localVariables.json", LocalVariables);
    SaveVariableCollection("globalVariables.json", GlobalVariables);
  }
  
  void SaveVariableCollection(string filename, ObservableCollection<VariableItem> collection)
  {
    string filePath = ProjectManager.GetDataPath(filename);
    var json = JsonConvert.SerializeObject(collection, JsonConfig.Settings);
    File.WriteAllText(filePath, json);
  }
}
public partial class VariableItem : ObservableObject
{

  [ObservableProperty]
  string description = "";
  [ObservableProperty]
  int id;

  [ObservableProperty]
  string name = "";

  [ObservableProperty]
  VariableType type = VariableType.Int;

  [ObservableProperty]
  string value = "";
}
