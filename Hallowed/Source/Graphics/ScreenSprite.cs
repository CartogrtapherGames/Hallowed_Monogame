using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState;

namespace Hallowed.Graphics;

public enum FadeState
{
  None,
  FadeIn,
  FadeOut,
}

// inspired by Monogame.Extended
/// <summary>
/// The class that handle the screen fade effect.
/// It is the same implementation as the Transition class in Monogame.Extended.
/// but it was changed to be toggling the fading and fadeout by the user.
/// all credit goes to the original author.
/// </summary>
public class ScreenSprite : IPostProcess
{

  public FadeState FadeState { get; set; } = FadeState.None;

  public float Value => MathHelper.Clamp(currentSeconds / halfDuration, 0f, 1f);

  public event EventHandler Completed;

  private float Duration { get; set; }

  private Color Color { get; set; }

  private readonly float halfDuration;
  private float currentSeconds;
  private GraphicsDevice graphicsDevice;
  private SpriteBatch spriteBatch;

  public ScreenSprite(GraphicsDevice graphicsDevice, float defaultDuration = 1f)
  {
    this.graphicsDevice = graphicsDevice;
    spriteBatch = new SpriteBatch(graphicsDevice);
    Duration = defaultDuration;
    halfDuration = Duration / 2f;
    currentSeconds = 0;
    Color = Color.Black;
  }

  public void FadeIn(float duration, Color color)
  {
    Duration = duration;
    FadeState = FadeState.FadeIn;
    Color = color;
  }

  public void FadeOut(float duration, Color color)
  {

    Duration = duration;
    FadeState = FadeState.FadeOut;
    Debug.Print(FadeState.ToString());
    Color = color;
  }

  public bool IsFading() => FadeState != FadeState.None;

  public void Update(GameTime gameTime)
  {
    if (!IsFading()) return;
    var elapsedSeconds = gameTime.GetElapsedSeconds();
    if (FadeState == FadeState.FadeIn) ProcessFadeIn(elapsedSeconds);
    if (FadeState == FadeState.FadeOut) ProcessFadeOut(elapsedSeconds);
  }

  private void ProcessFadeIn(float elapsedSeconds)
  {
    currentSeconds -= elapsedSeconds;
    if ((double)currentSeconds > 0.0) return;
    var completed = Completed;
    FadeState = FadeState.None;
    completed?.Invoke(this, EventArgs.Empty);

  }

  private void ProcessFadeOut(float elapsedSeconds)
  {
    currentSeconds += elapsedSeconds;
    if ((double)currentSeconds < halfDuration) return;
    var completed = Completed;
    FadeState = FadeState.None;
    completed?.Invoke(this, EventArgs.Empty);

  }


  public void Draw(GameTime gameTime)
  {
    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    var viewport = graphicsDevice.Viewport;
    var width = (double)viewport.Width;
    viewport = graphicsDevice.Viewport;
    var height = (double)viewport.Height;
    var color = Color * Value;
    spriteBatch.FillRectangle(0.0f, 0.0f, (float)width, (float)height, color);
    spriteBatch.End();
  }

  public void Dispose()
  {
    spriteBatch.Dispose();
  }

  protected float FadeSpeed()
  {
    return 3f;
  }

  protected float SlowFadeSpeed()
  {
    return 6f;
  }
}
