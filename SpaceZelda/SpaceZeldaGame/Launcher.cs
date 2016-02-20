using System;
using Hacksoi;
using Microsoft.Xna.Framework;

namespace SpaceZeldaGame
{
    public static class Launcher
    {
        [STAThread]
        static void Main()
        {
            Game game;
            //game = new SpaceZeldaGame();
            game = new HacksoiGame();
            game.Run();
        }
    }
}
