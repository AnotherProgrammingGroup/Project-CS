using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SpaceZeldaGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldKeyState;

        World world;
        Matrix view;
        Vector2 screenCenter;
        Vector2 camera;

        TiledMap tiledMap;

        Player player;

        public Game()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            world = new World(new Vector2(0, 9.82f)); // this is a test comment
        }

        protected override void Initialize()
        {
            view = Matrix.Identity;
            camera = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(16f);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            tiledMap = new TiledMap(Content, "Content/test.tmx");

            player = new Player(new Vector2(100, 100));
            player.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            view = Matrix.CreateTranslation(
                new Vector3(camera - screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));

            player.Update(gameTime);
            base.Update(gameTime);
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (state.IsKeyDown(Keys.A))
            {
                camera.X += 1.5f;
            }
            if (state.IsKeyDown(Keys.D))
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

            oldKeyState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, view);
            tiledMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
