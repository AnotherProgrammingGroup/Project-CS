using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using System.Collections.Generic;
using TiledSharp;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private GamePadState _oldPadState;
        private SpriteFont _font;

        private World _world;

        private Body _circleBody;
        private Body _groundBody;

        private Texture2D _circleSprite;
        private Texture2D _groundSprite;

        // Simple camera controls
        private Matrix _projection;
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private Vector2 _groundOrigin;
        private Vector2 _circleOrigin;

        private DebugViewXNA _debugView;
        
        private TmxMap _map;
        private List<Texture2D> _tilesetTextures;


#if !XBOX360
        const string Text = "Press A or D to rotate the ball\n" +
                            "Press Space to jump\n" +
                            "Use arrow keys to move the camera";
#else
                const string Text = "Use left stick to move\n" +
                                    "Use right stick to move camera\n" +
                                    "Press A to jump\n";
#endif

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            _map = new TmxMap("Content/test.tmx");
            _graphics.PreferredBackBufferWidth = _map.Width * _map.TileWidth;
            _graphics.PreferredBackBufferHeight = _map.Height * _map.TileHeight;

            Content.RootDirectory = "Content";

            //Create a world with gravity.
            _world = new World(new Vector2(0, 9.82f));
            _debugView = new DebugViewXNA(_world);
        }

        protected override void LoadContent()
        {
            // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            // Initialize camera controls
            _projection = Matrix.CreateOrthographicOffCenter(0, ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth), ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferHeight), 0, 0.01f, 1000f);
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            _debugView.LoadContent(_graphics.GraphicsDevice, Content);

            _font = Content.Load<SpriteFont>("Font");

            // Load sprites
            _circleSprite = Content.Load<Texture2D>("CircleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = Content.Load<Texture2D>("GroundSprite"); // 512px x 64px =>   8m x 1m

            /* We need XNA to draw the ground and circle at the center of the shapes */
            _groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);
            _circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, -1.5f);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(96 / 2f), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, 1.25f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;
            
            _tilesetTextures = new List<Texture2D>();
            foreach (var tileset in _map.Tilesets)
            {
                _tilesetTextures.Add(Content.Load<Texture2D>(tileset.Name.ToString()));
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleGamePad();
            HandleKeyboard();

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0);

            if (padState.IsConnected)
            {
                if (padState.Buttons.Back == ButtonState.Pressed)
                    Exit();

                if (padState.Buttons.A == ButtonState.Pressed && _oldPadState.Buttons.A == ButtonState.Released)
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));

                _circleBody.ApplyForce(padState.ThumbSticks.Left);
                _cameraPosition.X -= padState.ThumbSticks.Right.X;
                _cameraPosition.Y += padState.ThumbSticks.Right.Y;

                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

                _oldPadState = padState;
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            // Move camera
            if (state.IsKeyDown(Keys.Left))
                _cameraPosition.X += 1.5f;

            if (state.IsKeyDown(Keys.Right))
                _cameraPosition.X -= 1.5f;

            if (state.IsKeyDown(Keys.Up))
                _cameraPosition.Y += 1.5f;

            if (state.IsKeyDown(Keys.Down))
                _cameraPosition.Y -= 1.5f;

            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

            // We make it possible to rotate the circle body
            if (state.IsKeyDown(Keys.A))
                _circleBody.ApplyTorque(-10);

            if (state.IsKeyDown(Keys.D))
                _circleBody.ApplyTorque(10);

            if (state.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                _circleBody.ApplyLinearImpulse(new Vector2(0, -10));

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            ////Draw circle and ground
            //_batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            //_batch.Draw(_circleSprite, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, _circleBody.Rotation, _circleOrigin, 1f, SpriteEffects.None, 0f);
            //_batch.Draw(_groundSprite, ConvertUnits.ToDisplayUnits(_groundBody.Position), null, Color.White, 0f, _groundOrigin, 1f, SpriteEffects.None, 0f);
            //_batch.End();

            //// Display instructions
            //_batch.Begin();
            //_batch.DrawString(_font, Text, new Vector2(14f, 14f), Color.Black);
            //_batch.DrawString(_font, Text, new Vector2(12f, 12f), Color.White);
            //_batch.End();

            _batch.Begin();

            foreach (var layer in _map.Layers)
            {
                int tileIdx = 0;
                foreach (var tile in layer.Tiles)
                {
                    int tilesetIdx = 0;
                    foreach (var tileset in _map.Tilesets)
                    {
                        if (tileset.FirstGid <= tile.Gid && tile.Gid < tileset.FirstGid + tileset.TileCount)
                        {
                            Texture2D texture = _tilesetTextures[tilesetIdx];
                            //tileWidth and tileHeight are in pixels
                            int tileWidth = tileset.TileWidth, tileHeight = tileset.TileHeight;
                            //tilesetWidth measure number of tiles across
                            int tilesetWidth = texture.Width / (tileWidth + tileset.Spacing);

                            int textureID = tile.Gid - tileset.FirstGid;
                            int row = (textureID) / tilesetWidth;
                            int column = (textureID) % tilesetWidth;
                            Rectangle tilesetRec = new Rectangle(tileWidth * column + column * tileset.Spacing, tileHeight * row + row * tileset.Spacing, tileWidth, tileHeight);

                            int x = (tileIdx % _map.Width) * _map.TileWidth;
                            int y = (tileIdx / _map.Width) * _map.TileHeight + _map.TileHeight;
                            //At this point in time, (x, y) represents bottom left corner of map tile
                            y -= tileHeight;
                            //Now (x,y) represents top left corner of image tile
                            //Note that image tile represents the image being drawn,
                            //which can be a different size than the map tiles.
                            _batch.Draw(texture, new Rectangle(x, y, tileWidth, tileHeight), tilesetRec, Color.White);

                            break;
                        }
                        ++tilesetIdx;
                    }
                    ++tileIdx;
                }
            }

            _batch.End();

            _debugView.RenderDebugData(ref _projection, ref _view);

            base.Draw(gameTime);
        }
    }
}