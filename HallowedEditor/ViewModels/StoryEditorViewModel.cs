using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hallowed.Core.Models.Nodes;


namespace Hallowed.Editor.ViewModels;

public partial class StoryEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private TreeItemViewModel? selectedItem;
    
    [ObservableProperty]
    private object? currentNodeEditor;
    
    private readonly List<NodeBase> allNodes = new();
    
    public StoryEditorViewModel()
    {
        InitializeSampleStory();
    }
    
    private void InitializeSampleStory()
    {
        Foo();
        // Create sample nodes
        var node1 = new NodeLinear
        {
            Id = "greeting",
            editorData = new NodeLinear.EditorData
            {
                Text = "Welcome to the Enchanted Forest!",
                NextNode = Guid.Empty
            }
        };
        
        var node2 = new NodeLinear
        {
            Id = "forest_path",
            editorData = new NodeLinear.EditorData
            {
                Text = "You see two paths ahead...",
                NextNode = Guid.Empty
            }
        };
        
        var node3 = new NodeLinear
        {
            Id = "ending",
            editorData = new NodeLinear.EditorData
            {
                Text = "Your adventure begins...",
                NextNode = Guid.Empty
            }
        };
        
        // Link them: node1 -> node2 -> node3
        node1.editorData.NextNode = node2.Guid;
        node2.editorData.NextNode = node3.Guid;
        
        // Add to tracking
        allNodes.Add(node1);
        allNodes.Add(node2);
        allNodes.Add(node3);
        
        // Build tree structure
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
        
        chapter1.Children.Add(new TreeItemViewModel 
        { 
            Name = "Greeting",
            IsFolder = false,
            Node = node1
        });
        
        chapter1.Children.Add(new TreeItemViewModel 
        { 
            Name = "Forest Path",
            IsFolder = false,
            Node = node2
        });
        
        chapter1.Children.Add(new TreeItemViewModel 
        { 
            Name = "Ending",
            IsFolder = false,
            Node = node3
        });

        rootFolder.Children.Add(chapter1);
        Items.Add(rootFolder);
    }

    void Foo()
    {
        // Test the registry
        var registry = new NodeRegistry();

        Console.WriteLine($"Registered {registry.Count} node types:");
        foreach (var nodeType in registry.GetAllNodeTypes())
        {
            Console.WriteLine($"  - {nodeType}");
        }

// Test creating nodes
        var linearNode = registry.CreateNode(NodeType.Linear);
        Console.WriteLine($"Created: {linearNode.GetType().Name} (Type: {linearNode.Type})");

        var choiceNode = registry.CreateNode(NodeType.Choice);
        Console.WriteLine($"Created: {choiceNode.GetType().Name} (Type: {choiceNode.Type})");

        var actionNode = registry.CreateNode(NodeType.Action);
        Console.WriteLine($"Created: {actionNode.GetType().Name} (Type: {actionNode.Type})");
    }
    
    public ObservableCollection<TreeItemViewModel> Items { get; } = new();

    partial void OnSelectedItemChanged(TreeItemViewModel? value)
    {
        if (value?.Node is NodeLinear linearNode)
        {
            var vm = new NodeLinearViewModel(linearNode);
            var linearNodes = allNodes.OfType<NodeLinear>().ToList();
            vm.SetAvailableNodes(linearNodes);
            CurrentNodeEditor = vm;
        }
        else
        {
            CurrentNodeEditor = null;
        }
    }

    [RelayCommand]
    private void AddFolder()
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
    private void AddStory()
    {
        if (SelectedItem == null || !SelectedItem.IsFolder) return;

        // Create new node
        var newNode = new NodeLinear
        {
            Id = $"node_{DateTime.Now:HHmmss}",
            editorData = new NodeLinear.EditorData
            {
                Text = "Enter dialogue...",
                NextNode = Guid.Empty
            }
        };
        
        // Add to tracking list
        allNodes.Add(newNode);

        // Add to tree
        var newItem = new TreeItemViewModel
        {
            Name = newNode.Id,
            IsFolder = false,
            Node = newNode
        };
        
        SelectedItem.Children.Add(newItem);
        SelectedItem.IsExpanded = true;
        
        // Refresh current editor's dropdown (if one is open)
        if (CurrentNodeEditor is NodeLinearViewModel currentVm)
        {
            var linearNodes = allNodes.OfType<NodeLinear>().ToList();
            currentVm.SetAvailableNodes(linearNodes);
        }
    }
    
    [RelayCommand]
    private void DeleteStory()
    {
        if (SelectedItem?.Node == null) return;
        
        // Remove from tracking list
        allNodes.Remove(SelectedItem.Node);
        
        // Remove from tree
        foreach (var root in Items)
        {
            if (RemoveFromTree(root, SelectedItem))
                break;
        }
        
        // Clear editor
        CurrentNodeEditor = null;
        SelectedItem = null;
    }
    
    private bool RemoveFromTree(TreeItemViewModel parent, TreeItemViewModel toRemove)
    {
        if (parent.Children.Remove(toRemove))
            return true;
        
        foreach (var child in parent.Children)
        {
            if (RemoveFromTree(child, toRemove))
                return true;
        }
        
        return false;
    }

    // Public method for view to call after load
    public void SelectFirstNode()
    {
        var firstNode = FindFirstNode(Items);
        if (firstNode != null)
        {
            SelectedItem = firstNode;
        }
    }
    
    private TreeItemViewModel? FindFirstNode(ObservableCollection<TreeItemViewModel> items)
    {
        foreach (var item in items)
        {
            if (!item.IsFolder && item.Node != null)
                return item;
            
            var found = FindFirstNode(item.Children);
            if (found != null)
                return found;
        }
        return null;
    }
}

public partial class TreeItemViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isFolder;
    
    [ObservableProperty]
    private string name = "";
    
    [ObservableProperty]
    private NodeBase? node;

    public ObservableCollection<TreeItemViewModel> Children { get; } = new();
}
