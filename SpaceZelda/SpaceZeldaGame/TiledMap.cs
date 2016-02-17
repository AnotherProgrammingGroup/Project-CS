using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TiledSharp;

namespace SpaceZeldaGame
{
    public class TiledMap
    {
        TmxMap map;
        List<Texture2D> mapTextures;

        public TiledMap(ContentManager Content, string mapLocation)
        {
            map = new TmxMap(mapLocation);
            mapTextures = new List<Texture2D>();
            foreach (var tileset in map.Tilesets)
            {
                mapTextures.Add(Content.Load<Texture2D>(tileset.Name.ToString()));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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
                            Texture2D texture = mapTextures[tilesetIdx];
                            //tileWidth and tileHeight are in pixels
                            int tileWidth = tileset.TileWidth, tileHeight = tileset.TileHeight;
                            //tilesetWidth measure number of tiles across
                            int tilesetWidth = texture.Width / (tileWidth + tileset.Spacing);

                            int textureID = tile.Gid - tileset.FirstGid;
                            int row = (textureID) / tilesetWidth;
                            int column = (textureID) % tilesetWidth;
                            Rectangle sourceRectangle = 
                                new Rectangle(tileWidth * column + column * tileset.Spacing, 
                                              tileHeight * row + row * tileset.Spacing, 
                                              tileWidth, tileHeight);

                            int x = (tileIdx % map.Width) * map.TileWidth;
                            int y = (tileIdx / map.Width) * map.TileHeight + map.TileHeight - 1;
                            //At this point in time, (x, y) represents bottom left corner of map tile
                            y -= tileHeight - 1;
                            Rectangle destinationRectangle = 
                                new Rectangle(x, y, tileWidth, tileHeight);

                            //Now (x,y) represents top left corner of image tile
                            //Note that image tile represents the image being drawn,
                            //which can be a different size than the map tiles.
                            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);

                            break;
                        }
                        ++tilesetIdx;
                    }
                    ++tileIdx;
                }
            }
        }
    }
}