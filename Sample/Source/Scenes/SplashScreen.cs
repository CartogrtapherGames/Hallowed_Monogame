using System;
using System.Collections.Generic;
using Hallowed.Graphics;
using Hallowed.Manager;
using Hallowed.Scenes;
using Hallowed.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens.Transitions;

namespace Sample.Scenes;

public class SplashScreen : SceneBase
{
    List<Sprite> logos = [];
    int currentIndex;
    float displayTimer;
    const float DisplayDuration = 2f;
    bool waitingForDisplay;
    bool waitingForFadeOutCompletion;
    bool isTransitioning;
    bool finalFadeOutStarted;


    public override void Initialize()
    {
        base.Initialize();
        FadeSprite.Completed += OnFadeCompleted;
        SceneManager.PrintManifest();
    }

    public override void LoadContent()
    {
        base.LoadContent();

        // TODO : later on improve the queue so we dont need to do the path.
        AssetManager.QueueSprites("Canyon");
        AssetManager.QueueAudio("Theme1");

        var manifest = SceneManager.GetManifestEntry("SceneSplashScreen");
        foreach (var path in manifest)
        {
            var filename = AssetManager.GetFilenameFromPath(path);
            logos.Add(CreateCenteredLogo(filename));
        }
        logos[0].Visible = true;
    }

    public override void Start()
    {
        base.Start();
        Console.WriteLine("Loading assets...");
        AssetManager.LoadQueuedAssets(true);
        FadeIn(0.5f);
    }
    
    Sprite CreateCenteredLogo(string texturePath)
    {
        var sprite = new Sprite();
        sprite.Texture = AssetManager.LoadSystem(texturePath);
        sprite.Anchor = SpriteAnchor.Center;
        sprite.Position = Display.Center;
        sprite.Visible = false;
        ScaleSpriteToScreen(sprite, 0.5f, true);
        RootLayer.AddChild(sprite);
        Logger.LogOnce(sprite.Position.ToString());
        return sprite;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (isTransitioning) return;

        if (currentIndex >= logos.Count) return;

        logos[currentIndex].Update(gameTime);

        if (!FadeSprite.IsFading() && !waitingForDisplay)
        {
            waitingForDisplay = true;
            displayTimer = 0f;
        }

        if (!waitingForDisplay) return;
        displayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (displayTimer < DisplayDuration) return;

        waitingForDisplay = false;
        waitingForFadeOutCompletion = true;
        FadeOut(0.5f);
    }
    
    void OnFadeCompleted(object sender, EventArgs e)
    {

        if (!waitingForFadeOutCompletion) return;
        if (isTransitioning) return;
    
        waitingForFadeOutCompletion = false;

        // Hide the current logo ONLY if it's still valid
        if (currentIndex < logos.Count)
        {
            logos[currentIndex].Visible = false;
        }
    
        currentIndex++;

        // If we've shown all logos
        if (currentIndex >= logos.Count)
        {
            // Start a final fade to black, then transition
            if (!finalFadeOutStarted)
            {
                finalFadeOutStarted = true;
                waitingForFadeOutCompletion = true;
                FadeOut(0.5f);  // One more fade to ensure we're fully black
            }
            else
            {
                // NOW we're fully black, safe to transition
                SceneManager.Goto(new TitleScreen(), new FadeTransition(GraphicsDevice, Color.Black, 1f));
                isTransitioning = true;
            }
            return;
        }

        // Show next logo (we know currentIndex is valid here)
        logos[currentIndex].Visible = true;
        FadeIn(0.5f);
    }
    
    /// <summary>
    /// Scale a sprite to a percentage of the screen while maintaining aspect ratio.
    /// </summary>
    /// <param name="sprite">The sprite to scale</param>
    /// <param name="percent">Percentage of screen dimension (0.0 to 1.0)</param>
    /// <param name="basedOnHeight">If true, scale based on height; otherwise width</param>
    void ScaleSpriteToScreen(Sprite sprite, float percent, bool basedOnHeight = false)
    {
        if (sprite?.Texture == null) return;
    
        float scale;
    
        if (basedOnHeight)
        {
            float targetHeight = GraphicsDevice.Viewport.Height * percent;
            scale = targetHeight / sprite.Texture.Height;
        }
        else
        {
            float targetWidth = GraphicsDevice.Viewport.Width * percent;
            scale = targetWidth / sprite.Texture.Width;
        }
    
        sprite.SetScale(scale);
    }


    public override void UnloadContent()
    {
        if (logos != null)
        {
            foreach (var sprite in logos)
            {
                sprite?.Dispose();
            }
            logos.Clear();
        }
        base.UnloadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        if (currentIndex < logos.Count && !finalFadeOutStarted)
        {
            SpriteBatch.Begin();
            logos[currentIndex].Draw(SpriteBatch);
            SpriteBatch.End();
        }
        base.Draw(gameTime);
    }

    public override void Dispose()
    {
        FadeSprite.Completed -= OnFadeCompleted;
        base.Dispose();
    }
}
