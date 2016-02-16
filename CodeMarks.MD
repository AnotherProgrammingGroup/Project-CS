### Imagine this is like a big bookmark file, for code we're not ready to use yet, but would like to save publicly.
```c#
SpriteEffects s = SpriteEffects.FlipHorizontally;
int smX = 200;

    public void runChar()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            smX -= 2;
            s = SpriteEffects.FlipHorizontally;
        }
        else if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            smX += 2;
            s = SpriteEffects.None;
        }
    }

spriteBatch.Draw(sM, new Rectangle(smX, 200, 100, 100), null, Color.White, rotation, new Vector2(50, 50), s, 0f);
```
