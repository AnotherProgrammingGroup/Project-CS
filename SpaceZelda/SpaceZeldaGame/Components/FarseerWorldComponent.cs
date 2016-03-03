using Artemis.Interface;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SpaceZelda.Components
{
    // Contains the Farseer physics World class
    // Really just a wrapper for above mentioned class

    class FarseerWorldComponent : IComponent
    {
        public World world { get; }

        public FarseerWorldComponent(float gravity)
        {
            world = new World(new Vector2(0, gravity));
        }
    }
}
