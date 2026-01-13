using System;
using System.Collections.Generic;
using System.Linq;
using Gum.DataTypes;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Wireframe;
using RenderingLibrary.Graphics;

namespace Hallowed.Ui;

/// <summary>
/// The helper class that helps to access Gum elements.
/// </summary>
/// <param name="gumProject"></param>
/// <param name="uiElement"></param>
public class GumHelper(GumProjectSave gumProject, GraphicalUiElement uiElement)
{
  
  private GumProjectSave gumProject = gumProject;
  private GraphicalUiElement uiElement = uiElement;


  /// <summary>
  /// Get a screen by name
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public ScreenSave GetScreen(string name)
  {
    return this.gumProject.Screens.Find((screen) => screen.Name == name);
  }


  /// <summary>
  /// Fetch a Gum element by name
  /// </summary>
  /// <param name="name"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public T GetElement<T>(string name) where T : FrameworkElement
  {
    return this.uiElement.GetFrameworkElementByName<T>(name);
  }

  /// <summary>
  /// fetch a graphical element by name
  /// </summary>
  /// <param name="name"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public T GetGraphicalElement<T>(string name) where T : GraphicalUiElement
  {
    return this.uiElement.GetGraphicalUiElementByName(name) as T;
  }

  /// <summary>
  /// Set a button click event
  /// </summary>
  /// <param name="name"></param>
  /// <param name="callback"></param>
  public void SetButton(string name, Action callback)
  {
    var button = this.GetElement<Button>(name);
    button.Click += (_, __) => callback();
  }

  /// <summary>
  /// Set a button click event
  /// </summary>
  /// <param name="name"></param>
  /// <param name="callback"></param>
  public void SetButton(string name, Action<EventArgs> callback)
  {
    var button = this.GetElement<Button>(name);
    button.Click += (_, args) => callback(args);
  }

  /// <summary>
  /// Get the children of a Gum element by type
  /// </summary>
  /// <param name="name"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public List<GraphicalUiElement> GetElementChildren<T>(string name) where T : FrameworkElement
  {
    var element = (this.uiElement.GetFrameworkElementByName<T>(name).Visual.Children ?? throw new InvalidOperationException()).ToList();
    return element;
  }
  
  /// <summary>
  /// Get the children of a graphical element by type
  /// </summary>
  /// <param name="obj"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  protected List<T> GetElementChildrenOfType<T>(FrameworkElement obj) where T : GraphicalUiElement
  {
    var children = obj.Visual.Children;
    var list = new List<T>();
    if (children == null) return list;
    foreach (var child in children)
    {
      if (child is T t)
        list.Add(t);
    }
    return list;
  }
}
