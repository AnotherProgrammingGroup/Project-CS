using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SpaceZelda.Components;

namespace SpaceZelda.Systems
{
    // Processes PlayerCollisionComponent and acts accordingly
    // e.g. Colliding with item will add it to inventory
    // Applies to entities with the PlayerCollisionComponent

    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous,
                         GameLoopType = GameLoopType.Update,
                         Layer = 0)]
    class PlayerCollisionSystem : EntityComponentProcessingSystem<PlayerCollisionComponent>
    {
        public override void Process(Entity entity, PlayerCollisionComponent playerCollisionComponent)
        {
            //TODO
        }
    }
}
