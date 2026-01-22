using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hallowed.Editor.Services;
using Hallowed.Editor.Source;
using Hallowed.Editor.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Hallowed.Editor.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{

  [ObservableProperty]
  object? currentView;

  [ObservableProperty]
  string currentViewSubtitle = "Story editing will go here";

  [ObservableProperty]
  string currentViewTitle = "📖 Story Editor";
  [ObservableProperty]
  string projectName = "Fable Maker";

  public static string ProjectPath = "";

  public bool IsProjectOpen => ProjectManager.IsProjectOpen;
  
  public MainWindowViewModel()
  {
    Navigation = new NavigationManager();
    Navigation.NavigationChanged += OnNavigationChanged;
 // Notify UI to update
    ShowEmptyView();
  }
  
  private void ShowEmptyView()
  {
    CurrentView = new EmptyView { DataContext = this };
    CurrentViewTitle = "";
    CurrentViewSubtitle = "";
    OnPropertyChanged(nameof(IsProjectOpen));
  //  ProjectName = "No Project Loaded";
  }

  // Navigation manager
  public NavigationManager Navigation { get; }

  // ViewModels
  public SystemViewModel SystemViewModel { get; } = new SystemViewModel();
  public ItemsViewModel ItemsViewModel { get; } = new ItemsViewModel();
  public StoryEditorViewModel StoryEditorViewModel { get; } = new StoryEditorViewModel();

  public VariablesViewModel VariablesViewModel { get; } = new VariablesViewModel();

  void OnNavigationChanged(string title, string subtitle)
  {
    CurrentViewTitle = title;
    CurrentViewSubtitle = subtitle;

    // Switch views based on title
    CurrentView = title switch
    {
      "🔧 System" => new SystemView { DataContext = SystemViewModel },
      "💎 Items" => new ItemsView { DataContext = ItemsViewModel },
      "📖 Story Editor" => new StoryEditorView { DataContext = StoryEditorViewModel },
      "📊 Variables" => CreateVariableView(),
      "🎭 Actors" => null,          // TODO: Create ActorsView
      "📦 Content Manager" => null, // TODO: Create ContentView
      _ => null
    };
  }


  VariablesView CreateVariableView()
  {
    VariablesViewModel.LoadFromProject();
    return new VariablesView {DataContext =  VariablesViewModel};
  }
  
  
  // File commands
  [RelayCommand]
  void NewProject()
  {
    Console.WriteLine("New Project");
    OnProjectSetup();
  }

  void OnProjectSetup()
  {
    OnPropertyChanged(nameof(IsProjectOpen));
    CurrentView = new StoryEditorView { DataContext = StoryEditorViewModel };
  }
  [RelayCommand]
  private async Task OpenProject()
  {
    var window = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
      ? desktop.MainWindow
      : null;
  
    if (window == null) return;
  
    var fileTypeFilter = new FilePickerFileType("Hallowed Project Files")
    {
      Patterns = new[] { "*.hlwproj" }
    };
  
    var options = new FilePickerOpenOptions
    {
      Title = "Open Hallowed Project",
      AllowMultiple = false,
      FileTypeFilter = new[] { fileTypeFilter, FilePickerFileTypes.All }
    };
  
    var result = await window.StorageProvider.OpenFilePickerAsync(options);
  
    if (result.Count > 0)
    {
      var filePath = result[0].Path.LocalPath;
    
      // Use ProjectManager to open the project
      bool success = await ProjectManager.OpenProjectFromFile(filePath);
    
      if (success)
      {
        // Project opened successfully
        Console.WriteLine($"Opened project: {ProjectManager.ProjectName}");
      
        // TODO: Trigger any post-load logic
        // e.g., LoadProjectData(), RefreshUI(), etc.
        VariablesViewModel.LoadFromProject();
        OnProjectSetup();
      }
      else
      {
        // Show error dialog to user
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
          "Error Opening Project", 
          "Failed to open project. The project folder may not exist or the file may be corrupted.",
          ButtonEnum.Ok,
          Icon.Error);
      
        await messageBox.ShowWindowDialogAsync(window);
      }
    }
  }
  
  [RelayCommand]
  void Save()
  {
    if (ProjectManager.IsProjectOpen)
    {
      VariablesViewModel.SaveToProject();
    }
  }

  [RelayCommand]
  private async Task SaveAs()
  {
    var window = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
      ? desktop.MainWindow
      : null;
    
    if (window == null) return;
    
    var fileTypeFilter = new FilePickerFileType("Hallowed Project Files")
    {
      Patterns = new[] { "*.hlwproj" }
    };
    
    var options = new FilePickerSaveOptions
    {
      Title = "Save Hallowed Project As",
      DefaultExtension = "hlwproj",
      SuggestedFileName = ProjectName,
      FileTypeChoices = new[] { fileTypeFilter }
    };
    
    var result = await window.StorageProvider.SaveFilePickerAsync(options);
    
    if (result != null)
    {
      var filePath = result.Path.LocalPath;
      // TODO: Save project to filePath
      VariablesViewModel.SaveToProject();
     // ProjectName = Path.GetFileNameWithoutExtension(filePath);
    }
  }

  [RelayCommand]
  void Exit()
  {
    Environment.Exit(0);
  }

  // Edit commands
  [RelayCommand]
  void Undo()
  {
    Console.WriteLine("Undo");
  }

  [RelayCommand]
  void Redo()
  {
    Console.WriteLine("Redo");
  }

  [RelayCommand]
  void OpenPreferences()
  {
    Console.WriteLine("Preferences");
  }

  // Tools commands
  [RelayCommand]
  void ValidateNodes()
  {
    Console.WriteLine("Validate");
  }

  [RelayCommand]
  void ExportProject()
  {
    Console.WriteLine("Export");
  }

  // Help commands
  [RelayCommand]
  void OpenDocumentation()
  {
    Console.WriteLine("Docs");
  }

  [RelayCommand]
  void ShowAbout()
  {
    Console.WriteLine("About");
  }

  [RelayCommand]
  void ShowStoryEditor()
  {
    Navigation.NavigateToStory();
  }

  [RelayCommand]
  void ShowActor()
  {
    Navigation.NavigateToActors();
  }

  [RelayCommand]
  void ShowContentManager()
  {
    Navigation.NavigateToContent();
  }


}
