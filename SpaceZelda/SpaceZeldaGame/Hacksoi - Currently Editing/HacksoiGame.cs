using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using System.Collections.Generic;
using Util;
using SpaceZeldaGame;

// TODO: Add zoom feature (_view * _scale)

namespace Hacksoi
{
    public class HacksoiGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private SpriteFont _font;

        private Matrix _projection;
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        private World _world;

        private Body _playerBody;

        private DebugViewXNA _debugView;

        private BasicEffect _basicEffect;

        private TiledMap _map;

        public HacksoiGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _map = new TiledMap(Content, "Content/HacksoiContent/test.tmx", "HacksoiContent/");

            _graphics.PreferredBackBufferWidth = _map.Map.Width * _map.Map.TileWidth;
            _graphics.PreferredBackBufferHeight = _map.Map.Height * _map.Map.TileHeight;
            _graphics.ApplyChanges();
            
            _projection = Matrix.CreateOrthographicOffCenter(0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 0, 0.1f, 1000f);
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            _basicEffect = new BasicEffect(_graphics.GraphicsDevice);
            _basicEffect.Projection = _projection;
            _basicEffect.View = _view;
            _basicEffect.World = Matrix.Identity;
            _basicEffect.TextureEnabled = true;

            _font = Content.Load<SpriteFont>("Font");

            LoadWorld();
        }

        private void LoadWorld()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            _world = new World(new Vector2(0, 9.81f));
            _debugView = new DebugViewXNA(_world);
            _debugView.LoadContent(_graphics.GraphicsDevice, Content);

            List<MapWorldUtil.TmxObjectBody> objectBodies = MapWorldUtil.InsertObjects(_map.Map, _world);
            foreach (var objBod in objectBodies)
            {
                if(objBod.TmxObject.Name.Equals("Player"))
                {
                    _playerBody = objBod.Body;
                    break;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();
            
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            
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

            float torqueAmt = 2;
            if (state.IsKeyDown(Keys.A))
                _playerBody.ApplyTorque(-torqueAmt);

            if (state.IsKeyDown(Keys.D))
                _playerBody.ApplyTorque(torqueAmt);

            if (state.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                _playerBody.ApplyLinearImpulse(new Vector2(0, -2.5f));

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _basicEffect.View = _view;

            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, _basicEffect, null);
            _map.Draw(_batch);
            _batch.End();

            Matrix debugProjection = Matrix.CreateOrthographicOffCenter(0, ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth), 
                ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferHeight), 0, ConvertUnits.ToSimUnits(0.1f), ConvertUnits.ToSimUnits(1000f));
            Matrix debugView = Matrix.CreateTranslation(ConvertUnits.ToSimUnits(new Vector3(_cameraPosition, -1f)));
            _debugView.RenderDebugData(debugProjection, debugView);

            base.Draw(gameTime);
        }
    }
}