using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceZeldaGame
{
    abstract class AnimatedSprite
    {
        public enum myDirection { none, left, right, up, down };
        protected myDirection currentDir = myDirection.none;
        protected Texture2D sTexture;
        public Vector2 sPostion;
        private int frameIndex;
        private double timeElapsed;
        private double timeToUpdate;
        protected string currentAnimation;
        protected Vector2 sDirection = Vector2.Zero;

        public int FramesPerSecond
        {
            set { timeToUpdate = (1f / value); }
        }

        private Dictionary<string, Rectangle[]> sAnimations = new Dictionary<string, Rectangle[]>();
        private Dictionary<string, Vector2> sOffsets = new Dictionary<string, Vector2>();

        public AnimatedSprite(Vector2 position)
        {
            sPostion = position;
        }

        public void AddAnimation(int frames, int yPos, int xStartFrame, string name, int width, int height, Vector2 offset)
        {
            Rectangle[] Rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
            {
                Rectangles[i] = new Rectangle((i + xStartFrame) * width, yPos, width, height);
            }
            sAnimations.Add(name, Rectangles);
            sOffsets.Add(name, offset);
        }

        public virtual void Update(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;

                if (frameIndex < sAnimations[currentAnimation].Length - 1)
                {
                    frameIndex++;
                }
                else
                {
                    AnimationDone(currentAnimation);
                    frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sTexture, sPostion + sOffsets[currentAnimation], sAnimations[currentAnimation][frameIndex], Color.White);
        }

        public void PlayAnimation(string name)
        {
            if (currentAnimation != name && currentDir == myDirection.none)
            {
                currentAnimation = name;
                frameIndex = 0;
            }
        }

        public abstract void AnimationDone(string animation);
    }
}
