using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HallowedEditor.Source;
using HallowedEditor.Views;

namespace HallowedEditor.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string projectName = "Fable Maker";
    
    [ObservableProperty]
    private string currentViewTitle = "📖 Story Editor";
    
    [ObservableProperty]
    private string currentViewSubtitle = "Story editing will go here";
    
    [ObservableProperty]
    private object? currentView;
    
    // Navigation manager
    public NavigationManager Navigation { get; }
    
    // ViewModels
    public SystemViewModel SystemViewModel { get; } = new SystemViewModel();
    public ItemsViewModel ItemsViewModel { get; } = new ItemsViewModel();
    
    public MainWindowViewModel()
    {
        Navigation = new NavigationManager();
        Navigation.NavigationChanged += OnNavigationChanged;
    }
    
    private void OnNavigationChanged(string title, string subtitle)
    {
        CurrentViewTitle = title;
        CurrentViewSubtitle = subtitle;
        
        // Switch views based on title
        CurrentView = title switch
        {
            "🔧 System" => new SystemView { DataContext = SystemViewModel },
            "💎 Items" => new ItemsView { DataContext = ItemsViewModel },
            "📖 Story Editor" => null,    // TODO: Create StoryEditorView
            "🎭 Actors" => null,          // TODO: Create ActorsView
            "📦 Content Manager" => null, // TODO: Create ContentView
            _ => null
        };
    }
    
    // File commands
    [RelayCommand]
    private void NewProject() => Console.WriteLine("New Project");
    
    [RelayCommand]
    private void OpenProject() => Console.WriteLine("Open Project");
    
    [RelayCommand]
    private void Save() => Console.WriteLine("Save");
    
    [RelayCommand]
    private void SaveAs() => Console.WriteLine("Save As");
    
    [RelayCommand]
    private void Exit() => Environment.Exit(0);
    
    // Edit commands
    [RelayCommand]
    private void Undo() => Console.WriteLine("Undo");
    
    [RelayCommand]
    private void Redo() => Console.WriteLine("Redo");
    
    [RelayCommand]
    private void OpenPreferences() => Console.WriteLine("Preferences");
    
    // Tools commands
    [RelayCommand]
    private void ValidateNodes() => Console.WriteLine("Validate");
    
    [RelayCommand]
    private void ExportProject() => Console.WriteLine("Export");
    
    // Help commands
    [RelayCommand]
    private void OpenDocumentation() => Console.WriteLine("Docs");
    
    [RelayCommand]
    private void ShowAbout() => Console.WriteLine("About");
    
    [RelayCommand]
    private void ShowStoryEditor() => Navigation.NavigateToStory();

    [RelayCommand]
    private void ShowActor() => Navigation.NavigateToActors();

    [RelayCommand]
    private void ShowContentManager() => Navigation.NavigateToContent();
}