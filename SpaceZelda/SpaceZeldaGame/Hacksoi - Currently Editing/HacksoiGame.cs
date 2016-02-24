using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using System.Collections.Generic;
using Util;
using SpaceZelda;
using FarseerPhysics.Collision.Shapes;
using System;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using TiledSharp;

// TODO: Add zoom feature (basicEffect.View * scale)

namespace Hacksoi
{
    public class HacksoiGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private KeyboardState oldKeyState;
        private SpriteFont font;
        
        private Vector2 cameraPosition;
        private Vector2 screenCenter;

        private BasicEffect basicEffect;
        private RasterizerState rasterizerState;

        private World world;

        private DebugViewXNA debugView;

        private TiledMap map;

        private Body playerBody;

        private bool isGrounded;

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
            centerWindow();
            
            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            batch = new SpriteBatch(graphics.GraphicsDevice);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.Projection = Matrix.CreateOrthographic(graphics.GraphicsDevice.Viewport.Width, -graphics.GraphicsDevice.Viewport.Height, 0.1f, 1000f);
            basicEffect.View = Matrix.Identity;
            basicEffect.World = Matrix.Identity;
            basicEffect.TextureEnabled = true;

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;

            font = Content.Load<SpriteFont>("Font");

            LoadWorld();
        }

        private void centerWindow()
        {
            Window.Position = new Point(GraphicsDevice.DisplayMode.Width / 2 - Window.ClientBounds.Width / 2, 
                GraphicsDevice.DisplayMode.Height / 2 - Window.ClientBounds.Height / 2);
        }

        private void LoadWorld()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            world = new World(new Vector2(0, 9.81f));

            debugView = new DebugViewXNA(world);
            debugView.LoadContent(graphics.GraphicsDevice, Content);

            List<Body> bodies = new List<Body>();
            foreach (var objLayer in map.Map.ObjectGroups)
            {
                foreach (var obj in objLayer.Objects)
                {
                    Body body = U.InsertBody(obj, world);
                    body.UserData = new UserData();
                    bodies.Add(body);
                }
            }
            IterateBodies(bodies);
        }

        private void IterateBodies(List<Body> bodies)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                Body body = bodies[i];
                UserData userData = (UserData)body.UserData;
                TmxObject tmxObj = userData.Get<TmxObject>("tmxObj");
                if (tmxObj.Name.Equals("Player"))
                {
                    CreatePlayer(body);
                }
            }
        }

        private void CreatePlayer(Body body)
        {
            playerBody = body;
            playerBody.BodyType = BodyType.Dynamic;
            playerBody.FixedRotation = true;

            FixShape(playerBody);

            // This must be set after the shape change.
            playerBody.Friction = 0;

            Fixture foot = AttachFoot(playerBody);
            foot.OnCollision += FootCollision;
            foot.OnSeparation += EndFootCollision;
        }

        private static void FixShape(Body body)
        {

        }

        private static Fixture AttachFoot(Body body)
        {
            Vector2 size = GetSizeOf(body);
            float xOffset = size.X / 2.25f, yOrigin = size.Y / 2, yOffset = size.X / 6f;

            Vertices footVertices = new Vertices();
            footVertices.Add(new Vector2(-xOffset, yOrigin + yOffset));
            footVertices.Add(new Vector2(xOffset, yOrigin + yOffset));
            footVertices.Add(new Vector2(xOffset, yOrigin - yOffset));
            footVertices.Add(new Vector2(-xOffset, yOrigin - yOffset));
            PolygonShape foot = new PolygonShape(footVertices, 1f);

            Fixture footFixture = body.CreateFixture(foot);
            footFixture.IsSensor = true;

            return footFixture;
        }

        /// <summary>
        /// Returns the size of the body USING the Body's TmxObject in its UserData.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private static Vector2 GetSizeOf(Body body)
        {
            Util.BodyUserData userData = (Util.BodyUserData)body.UserData;
            return ConvertUnits.ToSimUnits(new Vector2((float)userData.TmxObject.Width, (float)userData.TmxObject.Height));
        }

        public bool FootCollision(Fixture f1, Fixture f2, Contact contact)
        {
            isGrounded = true;
            return true;
        }

        public void EndFootCollision(Fixture f1, Fixture f2)
        {
            isGrounded = false;
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
                cameraPosition.X -= cameraSpeed;

            if (state.IsKeyDown(Keys.Right))
                cameraPosition.X += cameraSpeed;

            if (state.IsKeyDown(Keys.Up))
                cameraPosition.Y -= cameraSpeed;

            if (state.IsKeyDown(Keys.Down))
                cameraPosition.Y += cameraSpeed;
            
            basicEffect.View = Matrix.CreateTranslation(new Vector3(-cameraPosition - screenCenter, -1f));

            PlayerInput(state);

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = state;
        }

        private void PlayerInput(KeyboardState state)
        {
            Vector2 vel = playerBody.GetLinearVelocityFromLocalPoint(Vector2.Zero);
            float desiredVel = 0;

            float maxVel = 3.5f;
            if(state.IsKeyDown(Keys.D))
                desiredVel += maxVel;
            if (state.IsKeyDown(Keys.A))
                desiredVel -= maxVel;

            float velChange = desiredVel - vel.X;
            float impulse = playerBody.Mass * velChange;
            playerBody.ApplyLinearImpulse(new Vector2(impulse, 0));

            if (isGrounded && state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                playerBody.ApplyLinearImpulse(new Vector2(0, -3.75f));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, basicEffect, null);
            map.Draw(batch);
            batch.End();

            Matrix debugProjection = Matrix.CreateOrthographic(ConvertUnits.ToSimUnits(graphics.GraphicsDevice.Viewport.Width), 
                ConvertUnits.ToSimUnits(-graphics.GraphicsDevice.Viewport.Height), ConvertUnits.ToSimUnits(0.1f), ConvertUnits.ToSimUnits(1000f));
            Matrix _debugView = Matrix.CreateTranslation(ConvertUnits.ToSimUnits(new Vector3(cameraPosition - screenCenter, -1f)));
            debugView.RenderDebugData(debugProjection, _debugView);

            debugView.BeginCustomDraw(debugProjection, _debugView);
            debugView.DrawPoint(playerBody.Position, 0.05f, Color.Blue);
            debugView.EndCustomDraw();

            base.Draw(gameTime);
        }
    }
}