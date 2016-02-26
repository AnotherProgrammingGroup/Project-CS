using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceZelda
{
    class Player : AnimatedSprite
    {
        float mySpeed = 90;

        public Player(Vector2 position) : base(position)
        {
            FramesPerSecond = 10;
            AddAnimation(8, 100, 0, "Left", 50, 50, new Vector2(0, 0));
            AddAnimation(1, 100, 0, "IdleLeft", 50, 50, new Vector2(0, 0));
            AddAnimation(8, 100, 8, "Right", 50, 50, new Vector2(0, 0));
            AddAnimation(1, 100, 8, "IdleRight", 50, 50, new Vector2(0, 0));
            PlayAnimation("IdleLeft");
        }

        public void LoadContent(ContentManager content)
        {
            sTexture = content.Load<Texture2D>("Sprite1");
        }

        public override void Update(GameTime gameTime)
        {
            sDirection = Vector2.Zero;
            HandleInput(Keyboard.GetState());
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            sDirection *= mySpeed;
            sPostion += (sDirection * deltaTime);
            base.Update(gameTime);
        }

        private void HandleInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.A))
            {
                sDirection += new Vector2(-1, 0);
                PlayAnimation("Left");
                currentDir = myDirection.left;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                sDirection += new Vector2(1, 0);
                PlayAnimation("Right");
                currentDir = myDirection.right;
            }
            if (currentAnimation.Contains("Left"))
            {
                PlayAnimation("IdleLeft");
            }
            if (currentAnimation.Contains("Right"))
            {
                PlayAnimation("IdleRight");
            }
            currentDir = myDirection.none;

        }

        public override void AnimationDone(string animation)
        {
        }
    }
}
