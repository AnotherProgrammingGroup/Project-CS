using Artemis;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpaceZelda.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceZeldaGame.Systems
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous, GameLoopType = GameLoopType.Update, Layer = 1)]
    public class CameraMovementSystem : EntityComponentProcessingSystem<TransformComponent>
    {
        public override void Process(Entity e, TransformComponent transformComponent)
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
