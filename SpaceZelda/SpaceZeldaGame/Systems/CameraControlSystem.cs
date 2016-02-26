using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpaceZelda.Components;

namespace SpaceZelda.Systems
{
    // Handles camera movement by modifying TransformComponent of each entity
    // Takes input from keyboard
    // Applies to entities with the TransformComponent

    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous,
                         GameLoopType = GameLoopType.Update,
                         Layer = 0)]
    public class CameraControlSystem : EntityComponentProcessingSystem<TransformComponent>
    {
        public override void Process(Entity entity, TransformComponent transformComponent)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                transformComponent.Position += new Vector2(-1, 0);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                transformComponent.Position += new Vector2(1, 0);
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                transformComponent.Position += new Vector2(0, -1);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                transformComponent.Position += new Vector2(0, 1);
            }
        }
    }
}
