using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowForm checkersGame = new GameWindowForm();
            checkersGame.RunGame();
        }
    }
}
