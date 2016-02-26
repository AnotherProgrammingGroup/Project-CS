using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace SpaceZelda.Components
{
    // Determines how the in game physics world is displayed on the screen
    // e.g. Translation means viewing only part of physics world
    //      Scaling (not added) means zooming in.

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
