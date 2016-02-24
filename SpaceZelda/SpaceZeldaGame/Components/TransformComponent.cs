using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceZelda.Components
{
    public class TransformComponent : IComponent
    {
        public Vector2 Position { get; set; }

        public TransformComponent() : this(Vector2.Zero) { }

        public TransformComponent(Vector2 position)
        {
            this.Position = position;
        }
    }
}
