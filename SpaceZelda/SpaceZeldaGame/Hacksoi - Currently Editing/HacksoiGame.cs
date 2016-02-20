using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using System.Collections.Generic;
using Util;
using SpaceZeldaGame;

// TODO: Add zoom feature (view * scale)

namespace Hacksoi
{
    public class HacksoiGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private KeyboardState oldKeyState;
        private SpriteFont font;

        private Matrix projection;
        private Matrix view;
        private Vector2 cameraPosition;
        private Vector2 screenCenter;

        private BasicEffect basicEffect;
        private RasterizerState rasterizerState;

        private World world;

        private Body playerBody;

        private DebugViewXNA debugView;

        private TiledMap map;

        public HacksoiGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            map = new TiledMap(Content, "Content/HacksoiContent/test.tmx", "HacksoiContent/");

            graphics.PreferredBackBufferWidth = map.Map.Width * map.Map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Map.Height * map.Map.TileHeight;
            graphics.ApplyChanges();

            projection = Matrix.CreateOrthographic(graphics.GraphicsDevice.Viewport.Width, -graphics.GraphicsDevice.Viewport.Height, 0.1f, 1000f);
            //projection = Matrix.CreateOrthographicOffCenter(0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0, 0.1f, 1000f);
            view = Matrix.Identity;
            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            batch = new SpriteBatch(graphics.GraphicsDevice);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.Projection = projection;
            basicEffect.View = view;
            basicEffect.World = Matrix.Identity;
            basicEffect.TextureEnabled = true;

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;

            font = Content.Load<SpriteFont>("Font");

            LoadWorld();
        }

        private void LoadWorld()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            world = new World(new Vector2(0, 9.81f));
            debugView = new DebugViewXNA(world);
            debugView.LoadContent(graphics.GraphicsDevice, Content);

            List<TmxObjectLayerUtil.TmxObjectBody> objectBodies = TmxObjectLayerUtil.InsertObjects(map.Map, world);
            foreach (var objBod in objectBodies)
            {
                if(objBod.TmxObject.Name.Equals("Player"))
                {
                    playerBody = objBod.Body;
                    break;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();
            
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            
            float cameraSpeed = 3.5f;
            if (state.IsKeyDown(Keys.Left))
                cameraPosition.X += cameraSpeed;

            if (state.IsKeyDown(Keys.Right))
                cameraPosition.X -= cameraSpeed;

            if (state.IsKeyDown(Keys.Up))
                cameraPosition.Y += cameraSpeed;

            if (state.IsKeyDown(Keys.Down))
                cameraPosition.Y -= cameraSpeed;
            
            view = Matrix.CreateTranslation(new Vector3(cameraPosition - screenCenter, -1f));

            float torqueAmt = 2;
            if (state.IsKeyDown(Keys.A))
                playerBody.ApplyTorque(-torqueAmt);

            if (state.IsKeyDown(Keys.D))
                playerBody.ApplyTorque(torqueAmt);

            if (state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                playerBody.ApplyLinearImpulse(new Vector2(0, -2.5f));

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            basicEffect.View = view;

            batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, basicEffect, null);
            map.Draw(batch);
            batch.End();

            Matrix debugProjection = Matrix.CreateOrthographic(ConvertUnits.ToSimUnits(graphics.GraphicsDevice.Viewport.Width), 
                ConvertUnits.ToSimUnits(-graphics.GraphicsDevice.Viewport.Height), ConvertUnits.ToSimUnits(0.1f), ConvertUnits.ToSimUnits(1000f));
            Matrix _debugView = Matrix.CreateTranslation(ConvertUnits.ToSimUnits(new Vector3(cameraPosition - screenCenter, -1f)));
            debugView.RenderDebugData(debugProjection, _debugView);

            base.Draw(gameTime);
        }
    }
}