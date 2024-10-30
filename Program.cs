using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.ShowMenu();
        game.Start();
    }
}
