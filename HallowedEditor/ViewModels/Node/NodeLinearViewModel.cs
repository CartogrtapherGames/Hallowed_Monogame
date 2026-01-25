using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Hallowed.Core.Models.Nodes;

namespace Hallowed.Editor.ViewModels.Node;

public partial class NodeLinearViewModel : ObservableObject
{
    private readonly NodeLinear node;
    
    // Guid is read-only - displayed for reference
    public Guid Guid => node.Guid;
    
    [ObservableProperty]
    private string name;
    
    [ObservableProperty]
    private string text;
    
    [ObservableProperty]
    private Guid nextNodeGuid;
    
    // For selecting the next node
    public ObservableCollection<NodeReference> AvailableNodes { get; } = new();
    
    public NodeLinearViewModel(NodeLinear node)
    {
        this.node = node;
        
        // Load from node
        name = node.Id;
        text = node.editorData.Text;
        nextNodeGuid = node.editorData.NextNode;
    }
    
    // Update node when properties change
    partial void OnNameChanged(string value)
    {
        node.Id = value;
    }
    
    partial void OnTextChanged(string value)
    {
        node.editorData.Text = value;
    }
    
    partial void OnNextNodeGuidChanged(Guid value)
    {
        node.editorData.NextNode = value;
    }
    
    public NodeLinear GetNode() => node;
    
    // Call this to populate the "Next Node" dropdown
    public void SetAvailableNodes(List<NodeLinear> allNodes)
    {
        AvailableNodes.Clear();
        
        // Add "None" option
        AvailableNodes.Add(new NodeReference 
        { 
            Guid = Guid.Empty, 
            DisplayName = "(None - End of story)" 
        });
        
        // Add all other nodes
        foreach (var n in allNodes)
        {
            // Don't allow linking to self
            if (n.Guid != node.Guid)
            {
                var displayName = string.IsNullOrWhiteSpace(n.Id) 
                    ? $"[Unnamed] {n.Guid.ToString().Substring(0, 8)}..." 
                    : n.Id;
                
                AvailableNodes.Add(new NodeReference 
                { 
                    Guid = n.Guid, 
                    DisplayName = displayName
                });
            }
        }
    }
}

// Helper class for displaying nodes in dropdowns
public class NodeReference
{
    public Guid Guid { get; set; }
    public string DisplayName { get; set; } = "";
}
