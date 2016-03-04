using Artemis;
using Artemis.System;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceZelda.Components;
using SpaceZelda.Utilities;

namespace SpaceZelda
{
    public class SpaceZeldaGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        EntityWorld entityWorld;

        public SpaceZeldaGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            entityWorld = new EntityWorld();

            EntitySystem.BlackBoard.SetEntry("ContentManager", this.Content);
            EntitySystem.BlackBoard.SetEntry("GraphicsDevice", this.GraphicsDevice);
            EntitySystem.BlackBoard.SetEntry("SpriteBatch", this.spriteBatch);

            entityWorld.InitializeAll(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadTiledMapUtility.LoadTiledMap(entityWorld, "Content/test.tmx", "");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            entityWorld.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            entityWorld.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
