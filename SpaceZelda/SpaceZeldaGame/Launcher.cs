using System;

namespace SpaceZeldaGame
{
    public static class Launcher
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game())
                game.Run();
        }
    }
}
