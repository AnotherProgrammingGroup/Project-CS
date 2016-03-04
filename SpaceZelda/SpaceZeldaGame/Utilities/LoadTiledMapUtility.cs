using Artemis;
using FarseerPhysics.Dynamics;
using SpaceZelda.Components;
using TiledSharp;

namespace SpaceZelda.Utilities
{
    static class LoadTiledMapUtility
    {
        //BuildEntity(entity, entityWorld)
        public static void LoadTiledMap(EntityWorld entityWorld, string mapFile, string tilesetLocation)
        {
            Entity tiledMapEntity = entityWorld.CreateEntity();
            tiledMapEntity.Tag = "map";
            tiledMapEntity.AddComponent(new TransformComponent());
            tiledMapEntity.AddComponent(new TiledMapComponent(mapFile, tilesetLocation));

            Entity farseerWorldEntity = entityWorld.CreateEntity();
            farseerWorldEntity.AddComponent(new FarseerWorldComponent(9.81f));

            TmxMap tmxMap = tiledMapEntity.GetComponent<TiledMapComponent>().Map;
            World world = farseerWorldEntity.GetComponent<FarseerWorldComponent>().world;
            foreach (var objLayer in tmxMap.ObjectGroups)
            {
                if (objLayer.Name == "collision")
                {
                    foreach (var tmxObj in objLayer.Objects)
                    {
                        Entity fixtureEntity = entityWorld.CreateEntity();
                        fixtureEntity.AddComponent(new FarseerFixtureComponent(tmxObj, world));
                    }
                }
            }
        }
    }
}
