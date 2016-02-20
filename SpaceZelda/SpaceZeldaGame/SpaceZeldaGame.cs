using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpaceZeldaGame
{
    public class SpaceZeldaGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldKeyState;

        World world;

        TiledMap tiledMap;

        Player player;
        
        Vector2 cameraPosition;
        Vector2 screenCenter;

        // Imagine BasicEffect as a neat class that holds the View and Projection matrices (http://rbwhitaker.wikidot.com/basic-matrices).
        // You can pass in this basicEffect into the SpriteBatch.Draw() so that Draw() uses
        // the matrices inside basicEffect.
        BasicEffect basicEffect;

        // This is also a parameter for SpriteBatch.Draw(). There exists something called CullMode
        // that specifies what side of the textures you want to Not draw. Although this is a nice optimization, it can
        // be difficult to debug somethings since sometimes, a texture won't render when it actually is
        // rendering, but just the wrong side (I just had this issue). This can be removed later, though.
        RasterizerState rasterizerState;

        public SpaceZeldaGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";

            world = new World(new Vector2(0, 9.82f));
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(16f);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.Projection = Matrix.CreateOrthographic(graphics.GraphicsDevice.Viewport.Width, -graphics.GraphicsDevice.Viewport.Height, 0.1f, 1000f);
            basicEffect.View = Matrix.Identity;
            basicEffect.TextureEnabled = true;

            rasterizerState = new RasterizerState();
            // do not cull anything
            rasterizerState.CullMode = CullMode.None; 

            tiledMap = new TiledMap(Content, "Content/test.tmx");

            player = new Player(new Vector2(0, 0));
            player.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            
            // note that we negate cameraPosition; this makes it more intuitive when we move the camera everywhere else
            basicEffect.View = Matrix.CreateTranslation(new Vector3(-cameraPosition, -1f));

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

            //float cameraSpeed = 5;
            //if (state.IsKeyDown(Keys.Left))
            //    cameraPosition.X -= cameraSpeed;
            //if (state.IsKeyDown(Keys.Right))
            //    cameraPosition.X += cameraSpeed;
            //if (state.IsKeyDown(Keys.Up))
            //    cameraPosition.Y -= cameraSpeed;
            //if (state.IsKeyDown(Keys.Down))
            //    cameraPosition.Y += cameraSpeed;

            // simply set the camera's position to the player's position
            cameraPosition.X = player.sPostion.X;
            cameraPosition.Y = player.sPostion.Y;

            oldKeyState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, basicEffect, null);
            tiledMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
