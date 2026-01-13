using System;
using Hallowed.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens.Transitions;

namespace Hallowed.Graphics;

/// <summary>
/// The class that handle all the transition type that can be used in the game.
/// </summary>
/// <remarks>
///  this class is marked as partial to allow adding new transition type without changing the original code.
/// </remarks>
public partial class TransitionType : SmartEnum<TransitionType, string>
{

  private readonly Func<GraphicsDevice, Transition> factory;

  private TransitionType(string name, Func<GraphicsDevice, Transition> factory) : base(name) => this.factory = factory;

  public Transition Create(GraphicsDevice graphicsDevice)
  {
    ArgumentNullException.ThrowIfNull(graphicsDevice, nameof(graphicsDevice));
    return factory(graphicsDevice);
  }

  // --- Defined Transitions ---

  public static readonly TransitionType FadeBlackFast = new("FadeBlackFast",
    gd => new FadeTransition(gd, Color.Black, 0.5f));

  public static readonly TransitionType FadeBlackSlow = new("FadeBlackSlow",
    gd => new FadeTransition(gd, Color.Black, 2.0f));

  public static readonly TransitionType FadeWhite = new("FadeWhite",
    gd => new FadeTransition(gd, Color.White, 1.0f));
}
