using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Hallowed.Rendering;

/// <summary>
/// The viewport class that contain ui elements and other containers. 
/// </summary>
public class Viewport : Container 
{

   public override void Update(GameTime gameTime)
   {
      foreach (var child in Children)
      {
         if (child is UiElement uiElement)
         {
            if (!uiElement.IsStatic) uiElement.Update(gameTime);
         }
         else
         {
            child.Update(gameTime);
         }
      }
   }
}

public abstract class UiElement : Container
{
   public bool IsStatic = true;

   // if its static, you use this function to manually request a repaint.
   void RequestRepaint()
   {
      
   }
}
