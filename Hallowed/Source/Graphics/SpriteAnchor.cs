using Microsoft.Xna.Framework;

namespace Hallowed.Graphics;

/// <summary>
/// Provides common anchor point presets for sprites.
/// Anchors are normalized (0-1 range) positions within a sprite.
/// </summary>
public static class SpriteAnchor
{
  /// <summary>
  /// Top-left corner (0, 0) - Default anchor point
  /// </summary>
  public static readonly Vector2 TopLeft = new Vector2(0f, 0f);
  
  /// <summary>
  /// Top-center (0.5, 0)
  /// </summary>
  public static readonly Vector2 TopCenter = new Vector2(0.5f, 0f);
  
  /// <summary>
  /// Top-right corner (1, 0)
  /// </summary>
  public static readonly Vector2 TopRight = new Vector2(1f, 0f);
  
  /// <summary>
  /// Center-left (0, 0.5)
  /// </summary>
  public static readonly Vector2 CenterLeft = new Vector2(0f, 0.5f);
  
  /// <summary>
  /// Center (0.5, 0.5) - Useful for rotation and centering
  /// </summary>
  public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
  
  /// <summary>
  /// Center-right (1, 0.5)
  /// </summary>
  public static readonly Vector2 CenterRight = new Vector2(1f, 0.5f);
  
  /// <summary>
  /// Bottom-left corner (0, 1)
  /// </summary>
  public static readonly Vector2 BottomLeft = new Vector2(0f, 1f);
  
  /// <summary>
  /// Bottom-center (0.5, 1) - Useful for characters standing on ground
  /// </summary>
  public static readonly Vector2 BottomCenter = new Vector2(0.5f, 1f);
  
  /// <summary>
  /// Bottom-right corner (1, 1)
  /// </summary>
  public static readonly Vector2 BottomRight = new Vector2(1f, 1f);
}
