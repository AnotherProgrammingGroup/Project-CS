using Artemis;
using Artemis.System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceZelda.Components;
using System;

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
            Entity tiledMapEntity = entityWorld.CreateEntity();
            tiledMapEntity.Tag = "map";
            tiledMapEntity.AddComponent(new TransformComponent());
            tiledMapEntity.AddComponent(new TiledMapComponent("Content/test.tmx", ""));
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
