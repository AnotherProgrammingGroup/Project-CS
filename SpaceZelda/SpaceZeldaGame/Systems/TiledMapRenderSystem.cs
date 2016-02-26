using Artemis.Attributes;
using Artemis.Blackboard;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceZelda.Components;
using System.Collections.Generic;
using Artemis;
using TiledSharp;
using Microsoft.Xna.Framework;

namespace SpaceZelda.Systems
{
    // Renders the Tiled map
    // Applies to entities with the following components:
    //      * TiledMapComponent
    //      * TransformComponent

    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous,
                         GameLoopType = GameLoopType.Draw,
                         Layer = 0)]
    public class TiledMapRenderSystem : EntityComponentProcessingSystem<TiledMapComponent, TransformComponent>
    {
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;

        public override void LoadContent()
        {
            this.contentManager = BlackBoard.GetEntry<ContentManager>("ContentManager");
            this.spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
        }

        public override void Process(Entity entity, 
                                     TiledMapComponent tiledMapComponent,
                                     TransformComponent transformComponent)
        {
            TmxMap map = tiledMapComponent.Map;
            List<Texture2D> textures = tiledMapComponent.Textures;
            Vector2 transform = transformComponent.Position;
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
                            Texture2D texture = textures[tilesetIdx];
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
                            //At this point in time, (x, y) represents bottom left corner of Map tile
                            y -= tileHeight - 1;
                            Rectangle destinationRectangle =
                                new Rectangle((int) (x - transform.X), (int) (y - transform.Y), tileWidth, tileHeight);

                            //Now (x,y) represents top left corner of image tile
                            //Note that image tile represents the image being drawn,
                            //which can be a different size than the Map tiles.
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
