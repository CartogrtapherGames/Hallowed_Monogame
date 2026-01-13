using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Manager;

/// <summary>
/// the class that load global effects
/// </summary>
public static partial class EffectManager
{
  
  public static bool IsInitialized { get; private set; } = false;
  public static Effect BlurEffect { get; set; }

  public static void LoadContent(ContentManager content)
  {
    BlurEffect = content.Load<Effect>("Effect/GaussianBlur");
    IsInitialized = true;
  }

  public static void Dispose()
  {
    BlurEffect?.Dispose();
  }
}
