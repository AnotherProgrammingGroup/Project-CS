using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace SpaceZeldaGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TiledMap tiledMap;

        AnimatedSprite animatedSprite;

        Matrix view;
        Vector2 screenCenter;
        Vector2 camera;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 320;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            view = Matrix.Identity;
            camera = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tiledMap = new TiledMap(Content, "Content/test.tmx");

            Texture2D texture = Content.Load<Texture2D>("Sprite1");
            animatedSprite = new AnimatedSprite(texture, 4, 4);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (state.IsKeyDown(Keys.Left))
            {
                camera.X += 1.5f;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                camera.X -= 1.5f;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                camera.Y += 1.5f;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                camera.Y -= 1.5f;
            }

            view = Matrix.CreateTranslation(
                new Vector3(camera - screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));

            animatedSprite.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, view);
            tiledMap.Draw(spriteBatch);
            animatedSprite.Draw(spriteBatch, (new Vector2(400, 200)));
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
