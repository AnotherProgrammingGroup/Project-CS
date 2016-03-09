using Artemis;
using Artemis.System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceZelda.Components;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace SpaceZelda.Utilities
{
    static class LoadTiledMapUtility
    {
        // BuildEntity(entity, entityWorld)
        public static void LoadTiledMap(EntityWorld entityWorld, string mapFile, string tilesetLocation)
        {
            // Actual map entity. Contains map data. Rendering system draws map from this entity
            Entity tiledMapEntity = entityWorld.CreateEntity();
            tiledMapEntity.Tag = "map";
            tiledMapEntity.AddComponent(new TransformComponent());
            tiledMapEntity.AddComponent(new TiledMapComponent(mapFile, tilesetLocation));
            TmxMap tmxMap = tiledMapEntity.GetComponent<TiledMapComponent>().Map;
            List<Texture2D> textures = tiledMapEntity.GetComponent<TiledMapComponent>().Textures;

            // Farseer world associated with map
            Entity farseerWorldEntity = entityWorld.CreateEntity();
            farseerWorldEntity.AddComponent(new FarseerWorldComponent(9.81f));
            World world = farseerWorldEntity.GetComponent<FarseerWorldComponent>().world;
            
            foreach (var objLayer in tmxMap.ObjectGroups)
            {
                // Create entities for static objects in map
                // TODO: If there are performance issues, can optimize all static objects into single entity
                if (objLayer.Name == "collision")
                {
                    foreach (var tmxObj in objLayer.Objects)
                    {
                        Entity fixtureEntity = entityWorld.CreateEntity();
                        fixtureEntity.AddComponent(new FarseerFixtureComponent(tmxObj, world));
                    }
                }

                // Create dynamic objects
                // In particular starting location of player is in here
                if (objLayer.Name == "dynamic")
                {
                    foreach (var tmxObj in objLayer.Objects)
                    {
                        tmxObj.Properties.Add("BodyType", "Dynamic");
                        Entity dynamicEntity = entityWorld.CreateEntity();

                        if (tmxObj.Tile != null)
                        {
                            dynamicEntity.AddComponent(new TransformComponent());

                            Texture2D texture = GetTexture(tmxMap, textures, tmxObj.Tile.Gid);
                            dynamicEntity.AddComponent(new TextureComponent(texture));
                        }

                        dynamicEntity.AddComponent(new FarseerFixtureComponent(tmxObj, world));

                        if (tmxObj.Name == "player")
                        {
                            dynamicEntity.Tag = "player";
                        }
                    }
                }
            }

        }

        public static Texture2D GetTexture(TmxMap map, List<Texture2D> textures, int gid)
        {
            int tilesetIdx = 0;
            foreach (var tileset in map.Tilesets)
            {
                if (tileset.FirstGid <= gid && gid < tileset.FirstGid + tileset.TileCount)
                {
                    Texture2D originalTexture = textures[tilesetIdx];
                    // tileWidth and tileHeight are in pixels
                    int tileWidth = tileset.TileWidth, tileHeight = tileset.TileHeight;
                    // tilesetWidth measure number of tiles across
                    int tilesetWidth = originalTexture.Width / (tileWidth + tileset.Spacing);

                    int textureID = gid - tileset.FirstGid;
                    int row = (textureID) / tilesetWidth;
                    int column = (textureID) % tilesetWidth;
                    Rectangle sourceRectangle =
                        new Rectangle(tileWidth * column + column * tileset.Spacing,
                                      tileHeight * row + row * tileset.Spacing,
                                      tileWidth, tileHeight);

                    GraphicsDevice graphicsDevice = EntitySystem.BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
                    Texture2D cropTexture = new Texture2D(graphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    originalTexture.GetData(0, sourceRectangle, data, 0, data.Length);
                    cropTexture.SetData(data);
                    return cropTexture;
                }
                ++tilesetIdx;
            }

            return null;
        }
    }
}
