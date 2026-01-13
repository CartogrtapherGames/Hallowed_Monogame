using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hallowed.Graphics;

public class AnimatedSprite : Sprite
{
  
  private Dictionary<string, SpriteAnimationData> animations;
  private SpriteAnimationData current;
  private float elapsed;
  private bool isPlaying;

  private float animationTimer = 0f;
  private int currentFrame = 0;

  public Action onAnimationComplete;
  private bool animationFinished;
  
  
  public IReadOnlyDictionary<string, SpriteAnimationData> Animations => animations;

  public AnimatedSprite() : base()
  {
    animations = new Dictionary<string, SpriteAnimationData>();
  }

  public void AddAnimation(string name, SpriteAnimationData animation)
  {
    if (!animations.TryAdd(name, animation))
      throw new ArgumentException($"The animation {name} already exists.");
  }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    if (isPlaying) UpdateAnimation(gameTime);
  }
  
  public void PlayAnimation(string name)
  {
    if(!animations.TryGetValue(name, out current))
      throw new Exception($"The animation {name} does not exist.");
    animationTimer = 0f;
    currentFrame = 0;
    animationFinished = false;
    isPlaying = true;
  }

  
  protected void UpdateAnimation(GameTime gameTime)
  {
    var frames = current.frames;
    var frameRate = current.frameRate;
    
    if(frames == null || frames.Count == 0) return;
    if (animationFinished && !current.loop) return; // Already finished
    
    animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
    
    var timePerFrame = 1f / frameRate;
    if (animationTimer >= timePerFrame)
    {
      currentFrame++;

      if (currentFrame >= frames.Count)
      {
        if (current.loop)
        {
          currentFrame = 0;
        }
        else
        {
          currentFrame = frames.Count - 1;
          animationFinished = true;
          isPlaying = false;
          onAnimationComplete?.Invoke(); // Trigger event
          return;
        }
      }
        
      animationTimer = 0f;
    }
    
    this.Frame = frames[currentFrame];
  }


  public override Texture2D Texture
  {
    get => base.Texture;
    set
    {
      base.Texture = value;
      UpdateOriginFromAnchor();
    }
  }
}


public struct SpriteAnimationData
{
  public List<Rectangle> frames;
  public float frameRate;
  public bool loop;
  public Action onAnimationEnd;  // TODO : maybe not implementing this in
                                 // the data but seperated because we dont want to
                                 // have to parse string for actions.
}