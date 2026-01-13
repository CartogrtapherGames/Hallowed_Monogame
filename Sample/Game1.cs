using System;
using Hallowed.Core;
using Hallowed.Graphics;
using Hallowed.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sample;

public class Game1 : Game
{
  GraphicsDeviceManager _graphics;
  SpriteBatch _spriteBatch;
  protected Container root;
  protected Sprite sprite;
  public Game1()
  {

    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    root = new Container();
    base.Initialize();
    
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    sprite = new Sprite(Content.Load<Texture2D>("dummy"));
    root.AddChild(sprite);
 //   sprite.Position = new Vector2(Display.Width / 2f, Display.Height / 2f);
    // TODO: use this.Content to load your game content here
    // Check if position is on-screen
    Console.WriteLine($"Sprite Position: {sprite.Position}");
    Console.WriteLine($"Screen Size: {GraphicsDevice.Viewport.Width}x{GraphicsDevice.Viewport.Height}");
    Console.WriteLine($"Sprite Texture: {sprite.Texture}");
    Console.WriteLine($"Texture Size: {sprite.Texture?.Width}x{sprite.Texture?.Height}");
    Console.WriteLine($"Sprite Color: {sprite.Color}");
  }

  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();

    // TODO: Add your update logic here

    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.CornflowerBlue);
    _spriteBatch.Begin();
    sprite.Draw(_spriteBatch);
    //root.Draw(_spriteBatch);
    _spriteBatch.End();
    // TODO: Add your drawing code here

    base.Draw(gameTime);
  }
}
