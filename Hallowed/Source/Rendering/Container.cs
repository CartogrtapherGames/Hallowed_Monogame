using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Rendering;


public class Container : DisplayObject
{

  // TODO : I gotta check if this is the best way to do this. or even work at all.
  public override Rectangle GetBounds()
  {
    if (Children.Count == 0)
      return Rectangle.Empty;
        
    float minX = float.MaxValue, minY = float.MaxValue;
    float maxX = float.MinValue, maxY = float.MinValue;
        
    foreach (var child in Children)
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
