using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using System.Collections.Generic;
using TiledSharp;
using TiledSharpUtil;
using System;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Collision.Shapes;

// TODO: Add zoom feature (_view * _scale)

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private SpriteFont _font;

        private World _world;

        private Body _circleBody;

        // Simple camera controls
        private Matrix _projection;
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        private DebugViewXNA _debugView;

        private BasicEffect _basicEffect;

        private TmxMap _map;
        private List<Texture2D> _tilesetTextures;

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
            _projection = Matrix.CreateOrthographicOffCenter(0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 0, 0.1f, 1000f);
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            _debugView.LoadContent(_graphics.GraphicsDevice, Content);

            _basicEffect = new BasicEffect(_graphics.GraphicsDevice);
            _basicEffect.Projection = _projection;
            _basicEffect.View = _view;
            _basicEffect.World = Matrix.Identity;
            _basicEffect.TextureEnabled = true;

            _font = Content.Load<SpriteFont>("Font");

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, -1.5f);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(48 / 2f), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            _tilesetTextures = new List<Texture2D>();
            foreach (var tileset in _map.Tilesets)
            {
                _tilesetTextures.Add(Content.Load<Texture2D>(tileset.Name.ToString()));
            }

            TmxMapUtil.InsertObjects(_map, _world);
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            // Move camera
            float cameraSpeed = 3.5f;
            if (state.IsKeyDown(Keys.Left))
                _cameraPosition.X += cameraSpeed;

            if (state.IsKeyDown(Keys.Right))
                _cameraPosition.X -= cameraSpeed;

            if (state.IsKeyDown(Keys.Up))
                _cameraPosition.Y += cameraSpeed;

            if (state.IsKeyDown(Keys.Down))
                _cameraPosition.Y -= cameraSpeed;

            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition, -1f));

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _basicEffect.View = _view;

            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, _basicEffect, null);

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

            Matrix debugProjection = Matrix.CreateOrthographicOffCenter(0, ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth), ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferHeight), 0, ConvertUnits.ToSimUnits(0.1f), ConvertUnits.ToSimUnits(1000f));
            Matrix debugView = Matrix.CreateTranslation(ConvertUnits.ToSimUnits(new Vector3(_cameraPosition, -1f)));
            _debugView.RenderDebugData(debugProjection, debugView);

            base.Draw(gameTime);
        }
    }
}