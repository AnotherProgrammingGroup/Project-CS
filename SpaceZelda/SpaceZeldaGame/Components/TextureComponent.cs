using Artemis.Interface;
using Artemis.System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceZelda.Components
{
    // Stores texture of entity

    class TextureComponent : IComponent
    {
        public string textureFile { get; set; }

        public Texture2D texture { get; }

        public TextureComponent(string file)
        {
            textureFile = file;
            ContentManager Content = EntitySystem.BlackBoard.GetEntry<ContentManager>("ContentManager");
            texture = Content.Load<Texture2D>(file);
        }
    }
}
