using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SpaceZelda.Components;

namespace SpaceZelda.Systems
{
    // Increments Farseer game time
    // In other words, simulates physics
    // Applies to entities with the FarseerWorldComponent

    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous,
                         GameLoopType = GameLoopType.Update,
                         Layer = 1)]
    class FarseerPhysicsSystem : EntityComponentProcessingSystem<FarseerWorldComponent>
    {
        public override void Process(Entity entity, FarseerWorldComponent farseerWorldComponent)
        {
            //TODO
        }
    }
}
