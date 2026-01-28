using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Hallowed.Core.Models.Nodes;

namespace Hallowed.Editor.ViewModels;

public partial class NodeLinearViewModel : ObservableObject
{
  private readonly NodeLinear node;
    
  public Guid Guid => node.Guid;
    
  [ObservableProperty]
  private string id;
    
  [ObservableProperty]
  private string text;
    
  [ObservableProperty]
  private Guid nextNodeGuid;  // ← Changed back to Guid
    
  public ObservableCollection<NodeReference> AvailableNodes { get; } = new();
    
  public NodeLinearViewModel(NodeLinear node)
  {
    this.node = node;
    id = node.Id;
    text = node.editorData.Text;
    nextNodeGuid = node.editorData.NextNode;  // ← This should work if NextNode is Guid
  }
    
  partial void OnIdChanged(string value) => node.Id = value;
  partial void OnTextChanged(string value) => node.editorData.Text = value;
  partial void OnNextNodeGuidChanged(Guid value) => node.editorData.NextNode = value;
    
  public NodeLinear GetNode() => node;
    
  public void SetAvailableNodes(List<NodeLinear> allNodes)
  {
    AvailableNodes.Clear();
    AvailableNodes.Add(new NodeReference { NodeGuid = Guid.Empty, DisplayName = "(None)" });
        
    foreach (var n in allNodes)
    {
      if (n.Guid != node.Guid)
      {
        var displayName = string.IsNullOrWhiteSpace(n.Id) 
          ? $"[Unnamed] {n.Guid.ToString().Substring(0, 8)}" 
          : n.Id;
                
        AvailableNodes.Add(new NodeReference { NodeGuid = n.Guid, DisplayName = displayName });
      }
    }
  }
}

public class NodeReference
{
  public Guid NodeGuid { get; set; }  // ← Changed back to Guid
  public string DisplayName { get; set; } = "";
}
