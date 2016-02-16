using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;
using System;
using System.Collections.Generic;

namespace Physics_Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TmxMap map;
        List<Texture2D> tilesetTextures;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            map = new TmxMap("Content/test.tmx");
            graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tilesetTextures = new List<Texture2D>();
            foreach (var tileset in map.Tilesets)
            {
                tilesetTextures.Add(Content.Load<Texture2D>(tileset.Name.ToString()));
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            foreach (var layer in map.Layers)
            {
                int tileIdx = 0;
                foreach (var tile in layer.Tiles)
                {
                    int tilesetIdx = 0;
                    foreach (var tileset in map.Tilesets)
                    {
                        if (tileset.FirstGid <= tile.Gid && tile.Gid < tileset.FirstGid + tileset.TileCount)
                        {
                            Texture2D texture = tilesetTextures[tilesetIdx];
                            //tileWidth and tileHeight are in pixels
                            int tileWidth = tileset.TileWidth, tileHeight = tileset.TileHeight;
                            //tilesetWidth measure number of tiles across
                            int tilesetWidth = texture.Width / (tileWidth + tileset.Spacing);

                            int textureID = tile.Gid - tileset.FirstGid;
                            int row = (textureID) / tilesetWidth;
                            int column = (textureID) % tilesetWidth;
                            Rectangle tilesetRec = new Rectangle(tileWidth * column + column * tileset.Spacing, tileHeight * row + row * tileset.Spacing, tileWidth, tileHeight);

                            int x = (tileIdx % map.Width) * map.TileWidth;
                            int y = (tileIdx / map.Width) * map.TileHeight + map.TileHeight;
                            //At this point in time, (x, y) represents bottom left corner of map tile
                            y -= tileHeight;
                            //Now (x,y) represents top left corner of image tile
                            //Note that image tile represents the image being drawn,
                            //which can be a different size than the map tiles.
                            spriteBatch.Draw(texture, new Rectangle(x, y, tileWidth, tileHeight), tilesetRec, Color.White);

                            break;
                        }
                        ++tilesetIdx;
                    }
                    ++tileIdx;
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
