using Artemis.Interface;
using Artemis.System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TiledSharp;

namespace SpaceZelda.Components
{
    // Component which stores a TmxMap along with associated textures

    public class TiledMapComponent : IComponent
    {
        public TmxMap Map { get; set; }

        public List<Texture2D> Textures { get; }

        public TiledMapComponent(string mapFile, string tilesetLocation)
        {
            Map = new TmxMap(mapFile);
            Textures = new List<Texture2D>();

            ContentManager Content = EntitySystem.BlackBoard.GetEntry<ContentManager>("ContentManager");
            foreach (var tileset in Map.Tilesets)
            {
                Textures.Add(Content.Load<Texture2D>(tilesetLocation + tileset.Name));
            }
        }
    }
}
