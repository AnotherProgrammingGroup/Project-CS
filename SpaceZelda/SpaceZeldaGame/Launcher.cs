using System;
using Microsoft.Xna.Framework;

namespace SpaceZelda
{
    public static class Launcher
    {
        [STAThread]
        static void Main()
        {
            Game game;
            game = new SpaceZeldaGame();
            //game = new HacksoiGame();
            game.Run();
        }
    }
}
