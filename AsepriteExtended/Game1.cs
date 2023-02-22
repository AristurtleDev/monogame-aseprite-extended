using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Aseprite;
using MonoGame.Aseprite.Content.Processors;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace AsepriteExtended;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    MonoGame.Extended.Sprites.SpriteSheet _spriteSheet;
    MonoGame.Extended.Sprites.AnimatedSprite _idleAnimation;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        AsepriteFile aseFile = AsepriteFile.Load(Path.Combine(Content.RootDirectory, "adventurer.aseprite"));
        _spriteSheet = SpriteSheetProcessorExtended.Process(GraphicsDevice, aseFile);
        _idleAnimation = new AnimatedSprite(_spriteSheet, "idle");
    }

    protected override void Update(GameTime gameTime)
    {
        _idleAnimation.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _idleAnimation.Draw(_spriteBatch, new Vector2(150, 150), 0.0f, new Vector2(9, 9));

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
