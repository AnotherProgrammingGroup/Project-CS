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

        Body playerBody;
        AnimatedSprite playerSprite;
        Vector2 playerOrigin;

        Body groundBody;
        Texture2D groundSprite;
        Vector2 groundOrigin;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            world = new World(new Vector2(0, 9.82f));
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

            Texture2D playerSpriteTexture = Content.Load<Texture2D>("Sprite1");
            playerSprite = new AnimatedSprite(playerSpriteTexture, 4, 4, 100);
            playerOrigin = new Vector2(playerSprite.Width / 2f, playerSprite.Height / 2f);
            Vector2 playerStartPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, -2f);
            playerBody = BodyFactory.CreateRectangle(world, 
                ConvertUnits.ToSimUnits(playerSprite.Width), ConvertUnits.ToSimUnits(playerSprite.Height), 1f, playerStartPosition);
            playerBody.BodyType = BodyType.Dynamic;
            playerBody.Restitution = 0.3f;
            playerBody.Friction = 0.5f;

            groundSprite = Content.Load<Texture2D>("GroundSprite");
            groundOrigin = new Vector2(groundSprite.Width / 2f, groundSprite.Height / 2f);
            Vector2 groundStartPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, 4f);
            groundBody = BodyFactory.CreateRectangle(world, 
                ConvertUnits.ToSimUnits(groundSprite.Width), ConvertUnits.ToSimUnits(groundSprite.Height), 1f, groundStartPosition);
            groundBody.IsStatic = true;
            groundBody.Restitution = 0.3f;
            groundBody.Friction = 0.5f;
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

            playerSprite.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleKeyboard()
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

            if (state.IsKeyDown(Keys.A))
            {
                playerBody.ApplyLinearImpulse(new Vector2(-1, 0));
            }
            if (state.IsKeyDown(Keys.D))
            {
                playerBody.ApplyLinearImpulse(new Vector2(1, 0));
            }
            if (state.IsKeyDown(Keys.W) && oldKeyState.IsKeyUp(Keys.W))
            {
                playerBody.ApplyLinearImpulse(new Vector2(0, -40));
            }

            oldKeyState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, view);
            tiledMap.Draw(spriteBatch);
            playerSprite.Draw(spriteBatch, ConvertUnits.ToDisplayUnits(playerBody.Position) - playerOrigin);
            spriteBatch.Draw(groundSprite, ConvertUnits.ToDisplayUnits(groundBody.Position), null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
