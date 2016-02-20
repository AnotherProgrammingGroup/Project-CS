using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using System.Collections.Generic;
using Util;
using SpaceZeldaGame;
using FarseerPhysics.Collision.Shapes;
using System;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

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
            
            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            batch = new SpriteBatch(graphics.GraphicsDevice);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.Projection = Matrix.CreateOrthographic(graphics.GraphicsDevice.Viewport.Width, -graphics.GraphicsDevice.Viewport.Height, 0.1f, 1000f);
            //basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0, 0.1f, 1000f);
            basicEffect.View = Matrix.Identity;
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

            List<TmxObjectLayerUtil.TmxObjectAndBody> tmxObjectAndBodies = TmxObjectLayerUtil.InsertObjects(map.Map, world);
            for (int i = 0; i < tmxObjectAndBodies.Count; i++)
            {
                TmxObjectLayerUtil.TmxObjectAndBody tmxObjAndBody = tmxObjectAndBodies[i];
                if(tmxObjAndBody.TmxObject.Name.Equals("Player"))
                {
                    adjustBody(ref tmxObjAndBody);
                    playerBody = tmxObjAndBody.Body;
                    break;
                }
            }
            
        }

        /// <summary>
        /// Adjusts the body to a shape that supports platforming physics.
        /// </summary>
        /// <param name="tmxObjAndBody"></param> The body's shape must be a polygon.
        private void adjustBody(ref TmxObjectLayerUtil.TmxObjectAndBody tmxObjAndBody)
        {
            Body body = tmxObjAndBody.Body;
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;

            float width = ConvertUnits.ToSimUnits(tmxObjAndBody.TmxObject.Width);
            float height = ConvertUnits.ToSimUnits(tmxObjAndBody.TmxObject.Height);
            CircleShape circle = new CircleShape(width / 2, 1f);
            // position is relative to the Body's position
            circle.Position = new Vector2(0, height / 2);

            // with the addition of the circle, we the player's overall shape is now too tall
            // by an amount of the circle's radius. So, we have to replace the original rectangular
            // shape with a new, smaller one.
            Fixture originalFixture = body.FixtureList[0];
            Vertices vertices = ((PolygonShape)(originalFixture.Shape)).Vertices;
            Vertices newVertices = new Vertices();
            foreach (var vec in vertices)
            {
                Vector2 newVec = vec;
                if (newVec.Y < 0)
                {
                    newVec.Y += circle.Radius;
                }
                newVertices.Add(newVec);
            }
            PolygonShape newShape = new PolygonShape(newVertices, 1f);

            Vertices footVertices = new Vertices();
            footVertices.Add(new Vector2(-width / 4, circle.Position.Y + circle.Radius + circle.Radius / 4f));
            footVertices.Add(new Vector2(width / 4, circle.Position.Y + circle.Radius + circle.Radius / 4f));
            footVertices.Add(new Vector2(width / 4, circle.Position.Y + circle.Radius - circle.Radius / 4f));
            footVertices.Add(new Vector2(-width / 4, circle.Position.Y + circle.Radius - circle.Radius / 4f));
            PolygonShape foot = new PolygonShape(footVertices, 1f);

            body.DestroyFixture(originalFixture);
            body.CreateFixture(newShape);
            body.CreateFixture(circle);
            Fixture footFixture = body.CreateFixture(foot, "foot");
            footFixture.IsSensor = true;
            footFixture.OnCollision += FootCollision;
            footFixture.OnSeparation += EndFootCollision;
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
            
            if(state.IsKeyDown(Keys.D))
                desiredVel += 5;
            if (state.IsKeyDown(Keys.A))
                desiredVel -= 5;

            float velChange = desiredVel - vel.X;
            float impulse = playerBody.Mass * velChange;
            playerBody.ApplyLinearImpulse(new Vector2(impulse, 0));

            if (isGrounded && state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                playerBody.ApplyLinearImpulse(new Vector2(0, -2.5f));
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

            //debugView.BeginCustomDraw(debugProjection, _debugView);
            //debugView.DrawPoint(playerBody.Position, 0.05f, Color.Blue);
            //debugView.EndCustomDraw();

            base.Draw(gameTime);
        }
    }
}