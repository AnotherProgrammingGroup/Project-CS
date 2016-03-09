using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;

namespace SpaceZelda.Systems
{
    // Handles player movement
    // Takes input from keyboard and adjusts Player properties in physics engine
    // Only applies to entity with tag "player"

    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous,
                         GameLoopType = GameLoopType.Update,
                         Layer = 0)]
    class PlayerControlSystem : TagSystem
    {
        // base(player) makes system only apply to entities with tag player
        public PlayerControlSystem() : base("player") { }

        public override void Process(Entity entity)
        {
        }
    }
}
