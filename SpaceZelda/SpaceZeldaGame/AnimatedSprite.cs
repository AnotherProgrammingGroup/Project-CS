using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceZeldaGame
{
    public class AnimatedSprite
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int MillisecondsPerFrame { get; set; }

        private int currentFrame;
        private int totalFrames;
        private int timeSinceLastFrame = 0;


        public AnimatedSprite(Texture2D texture, int rows, int columns, int milliseconds)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            Height = texture.Height / rows;
            Width = texture.Width / columns;
            MillisecondsPerFrame = milliseconds;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > MillisecondsPerFrame)
            {
                ++currentFrame;
                if (currentFrame == totalFrames)
                {
                    currentFrame = 0;
                }
                timeSinceLastFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int row = currentFrame / Columns;
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(Width * column + 1, Height * row + 1, Width - 1, Height - 1);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, Width, Height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}