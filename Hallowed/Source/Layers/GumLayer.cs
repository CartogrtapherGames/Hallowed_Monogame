using System;
using System.Collections.Generic;
using System.Linq;
using Gum.DataTypes;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Mvvm;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum;

namespace Hallowed.Layers;


/// <summary>
/// The scene layer that handle and manage Gum screens.
/// </summary>
public class GumLayer(string projectPath) : SceneLayer
{
  protected GraphicalUiElement RootElement { get; set; }
  protected GumService GumService { get; set; }
  protected GumProjectSave GumProject { get; set; }
  public string GumProjectPath { get; set; } = projectPath;


  readonly Dictionary<Button, Func<bool>> enabledButtonStates = new Dictionary<Button, Func<bool>>();
  
  public override void Initialize()
  {
    base.Initialize();
    GumService = GumService.Default;
    if (!GumService.IsInitialized)
      GumService.Initialize(GameEngine, GumProjectPath);
    
    GumProject = ObjectFinder.Self.GumProjectSave;
  }

  public override void Update(GameTime gameTime)
  {
    GumService?.Update(gameTime);
    foreach (var kvp in enabledButtonStates)
    {
      kvp.Key.IsEnabled = kvp.Value();
    }
  }
  
  public override void Draw(GameTime gameTime)
  {
    GumService?.Draw();
  }
  

  public override void UnloadContent()
  {
    base.UnloadContent();
    RootElement?.RemoveFromRoot();
  }

  public ScreenSave GetScreen(string name)
  {
    return GumProject?.Screens.Find((screen) => screen.Name == name);
  }

  public T GetElement<T>(string name) where T : FrameworkElement
  {
    return RootElement?.GetFrameworkElementByName<T>(name);
  }

  public T GetGraphicalElement<T>(string name) where T : GraphicalUiElement
  {
    return RootElement?.GetGraphicalUiElementByName(name) as T;
  }

  public List<GraphicalUiElement> GetElementChildren<T>(string name) where T : FrameworkElement
  {
    var element = GetElement<T>(name);
    return element?.Visual.Children.ToList() ?? [];
  }

  /// <summary>
  /// Gets all children of a specific type from a framework element.
  /// </summary>
  /// <typeparam name="T">The type of graphical UI element to find.</typeparam>
  /// <param name="parent">The parent framework element.</param>
  /// <returns>A list of children matching the type.</returns>
  protected List<T> GetElementChildrenOfType<T>(FrameworkElement parent) where T : GraphicalUiElement
  {
    var children = parent?.Visual.Children;
    var list = new List<T>();

    if (children == null)
      return list;

    foreach (var child in children)
    {
      if (child is T typedChild)
      {
        list.Add(typedChild);
      }
    }
    return list;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      RootElement?.RemoveFromRoot();
      RootElement = null;
      enabledButtonStates.Clear();
    }

    base.Dispose(disposing);
  }

  #region Fluent API Methods

  public GumLayer LoadScreen(string screenName)
  {
    var screen = GetScreen(screenName);
    if (screen != null)
    {
      RootElement = screen.ToGraphicalUiElement();
      RootElement.AddToRoot();
    }
    return this;
  }

  public GumLayer SetButton(string buttonName, Action callBack)
  {
    var button = GetElement<Button>(buttonName);
    if (button != null)
    {
      button.Click += (_, __) => callBack();
    }
    return this;
  }

  public GumLayer SetButton(string buttonName, Action<EventArgs> callBack)
  {
    var button = GetElement<Button>(buttonName);
    if (button != null)
    {
      button.Click += (_, args) => callBack(args);
    }
    return this;
  }

  public GumLayer SetButton(string buttonName, Action callBack, Func<bool> enabledCallback)
  {
    SetButton(buttonName, callBack);
    var button = GetElement<Button>(buttonName);
    if (enabledCallback != null) enabledButtonStates.Add(button, enabledCallback);
    return this;
  }

  #endregion

}
