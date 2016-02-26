using Artemis.Interface;

namespace SpaceZelda.Components
{
    // Fixtures in Farseer can be though of as entities
    // They contain position as well as shape
    // Can be fixed or dynamic
    // Have other properties such as friction, elasticity, etc.
    // Has optional onCollision method which adds _____collisionComponents to be processed
    //     * optional because not all objects have collisions that trigger other events

    class FarseerFixtureComponent : IComponent
    {
        //TODO
    }
}
