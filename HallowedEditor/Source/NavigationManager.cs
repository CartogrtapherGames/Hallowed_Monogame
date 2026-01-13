using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HallowedEditor.Source;

public partial class NavigationManager : ObservableObject
{
    public ObservableCollection<NavItem> Items { get; }
    
    public event Action<string, string>? NavigationChanged;
    
    public NavigationManager()
    {
        Items = new ObservableCollection<NavItem>
        {
            new NavItem 
            { 
                Icon = "📖", 
                Label = "Story", 
                IsSelected = true,
                SelectCommand = new RelayCommand(() => NavigateTo("Story", "📖 Story Editor", "Story editing will go here"))
            },
            new NavItem
            {
                Icon  = "🎭",
                Label = "Actors",
                IsSelected = false,
                SelectCommand = new RelayCommand(() => NavigateTo("Actors", "🎭 Actors", "Actors editing will go here"))
            },
            new NavItem
            {
                Icon = "💎",
                Label = "Items",
                IsSelected = false,
                SelectCommand = new RelayCommand(() => NavigateTo("Items", "💎 Items", "Manage your game items"))
            },
            new NavItem 
            { 
                Icon = "📦", 
                Label = "Content", 
                IsSelected = false,
                SelectCommand = new RelayCommand(() => NavigateTo("Content", "📦 Content Manager", "Content management will go here"))
            },
            new NavItem
            {
                Icon = "🔧",
                Label = "System",
                IsSelected = false,
                SelectCommand = new RelayCommand(() => NavigateTo("System", "🔧 System", "System management will go here"))
            }
        };
    }
    
    private void NavigateTo(string label, string title, string subtitle)
    {
        var navItem = FindNavByName(label);
        UpdateSelection(Items.IndexOf(navItem));
        NavigationChanged?.Invoke(title, subtitle);
    }
    
    private void UpdateSelection(int selectedIndex)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].IsSelected = (i == selectedIndex);
        }
    }
    
    private NavItem FindNavByName(string label)
    {
        return Items.FirstOrDefault(item => 
                string.Equals(item.Label, label, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Navigation item '{label}' not found.");
    }
    
    public void NavigateToStory() => NavigateTo("Story", "📖 Story Editor", "Story editing will go here");
    public void NavigateToActors() => NavigateTo("Actors", "🎭 Actors", "Actors editing will go here");
    public void NavigateToItems() => NavigateTo("Items", "💎 Items", "Manage your game items");
    public void NavigateToContent() => NavigateTo("Content", "📦 Content Manager", "Content management will go here");
    public void NavigateToSystem() => NavigateTo("System", "🔧 System", "System settings");
}
