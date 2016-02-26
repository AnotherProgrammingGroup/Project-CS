using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceZelda.Components;

namespace SpaceZelda.Systems
{
    // Renders FarseerFixtures
    // Applies to entities with following components: 
    //      * farseer fixture component
    //      * texture component
    //      * transform component

    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous,
                         GameLoopType = GameLoopType.Draw,
                         Layer = 1)]
    class FarseerFixtureRenderSystem
        : EntityComponentProcessingSystem<FarseerFixtureComponent, TextureComponent, TransformComponent>
    {
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;

        public override void LoadContent()
        {
            this.contentManager = BlackBoard.GetEntry<ContentManager>("ContentManager");
            this.spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
        }

        public override void Process(Entity entity,
                                     FarseerFixtureComponent farseerFixtureComponent,
                                     TextureComponent textureComponent,
                                     TransformComponent transformComponent)
        {
            //TODO
        }
    }
}
