using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Hallowed.Core;

/// <summary>
/// Represents a scene graph node that can group and transform multiple <see cref="DisplayObject"/> instances.
/// Containers allow hierarchical transformations, positioning, and rendering order management.
/// </summary>
/// <remarks>
/// A Container can contain both <see cref="Graphics.Sprite"/> and other <see cref="Container"/> objects,
/// forming a recursive display tree. Transformations applied to the Container (such as position,
/// rotation, or scale) affect all of its children.
/// </remarks>
public class Container : DisplayObject
{
  
  // TODO : I gotta check if this is the best way to do this. or even work at all.
  public override Rectangle GetBounds()
  {
    if (this.Children.Count == 0)
      return Rectangle.Empty;
        
    float minX = float.MaxValue, minY = float.MaxValue;
    float maxX = float.MinValue, maxY = float.MinValue;
        
    foreach (var child in this.Children)
    {
      if (child is not DisplayObject { Visible: true } displayChild) continue;
      
      var childBounds = displayChild.GetBounds();
      var childPos = displayChild.Position;
      var childScale = displayChild.Scale;
                
      float childWidth = MathF.Abs(childScale.X * childBounds.Width);
      float childHeight = MathF.Abs(childScale.Y * childBounds.Height);
                
      minX = MathF.Min(minX, childPos.X);
      minY = MathF.Min(minY, childPos.Y);
      maxX = MathF.Max(maxX, childPos.X + childWidth);
      maxY = MathF.Max(maxY, childPos.Y + childHeight);
    }
        
    if (minX == float.MaxValue)
      return Rectangle.Empty;
        
    return new Rectangle(
      (int)minX, (int)minY,
      (int)(maxX - minX), (int)(maxY - minY)
    );
  }
}
