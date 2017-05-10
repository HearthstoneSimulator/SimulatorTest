using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicConsoleSimulator
{
    class Program
    {
        public static void Log(string s)
        {
            Console.WriteLine(s);
        }
        static void Main(string[] args)
        {
            int gamesToSimulate = 0;
            int player0wins = 0;
            int player1wins = 0;
            int draws =0;
            string deck0 = "random";
            string deck1 = "random";
            string AI0 = "face";
            string AI1 = "face";
            Log("Simulator started in Console Mode");
            if (args.Length == 0)
            {
                Log("No starting parametres detected - please imput your preferences manually.");
                Log("Imput number of games you want to simulate.");
                gamesToSimulate = 10000;// Convert.ToInt32(Console.ReadLine());
                //deck0 = "deck1.txt";
                deck0 = "basic_mage.txt";
                deck1 = "basic_mage.txt";
            }
            else
            {
                if (args.Length != 5)
                {
                    Log("Wrong number of arguments.");
                }
                else
                {
                    //load number of games
                    gamesToSimulate = Convert.ToInt32(args[0]);
                    //
                }
                //Format of arguments is following:
                //1 - number of games you want to play
                //2 - name of file containing decklist - one card name per line - deck for you  
                //3 - name of file containing decklist - one card name per line - deck for opponent
                //4 - name of AI for your deck
                //5 - name of AI for your opponents deck

                gamesToSimulate = Convert.ToInt32(Console.ReadLine());
            }
            System.Diagnostics.Stopwatch stopky = new System.Diagnostics.Stopwatch();
            GameIntestines.GameEngine oneGame = new GameIntestines.GameEngine();
            oneGame.WriteDebugTexts = false;
            oneGame.deck0name = deck0;
            oneGame.deck1name = deck1;
            oneGame.prepareGame();
            Log("Performance measurement started.");
            stopky.Start();
            for (int i = 0; i < gamesToSimulate; i++)
            {
                oneGame.setStuff();
                oneGame.simulateOneGame();
                //oneGame.InitialiseGame();
                int winner = oneGame.getWinner();
                if (winner == 0)
                {
                    //Console.WriteLine("Game n.{0} result: player {1} wins.",i,winner);
                    player0wins++;
                }
                if (winner == 1)
                {
                    //Console.WriteLine("Game n.{0} result: player {1} wins.", i, winner);
                    player1wins++;
                }
                if (winner == 3)
                {
                    //Console.WriteLine("Game n.{0} result: draw.", i);
                    draws++;
                }
            }
            stopky.Stop();
            Console.WriteLine("Total Time Elapsed: {0}", stopky.Elapsed);
            Console.WriteLine("Final score after {0} simulations: ",gamesToSimulate);
            Console.WriteLine("{0}/{1}/{2}", player0wins, player1wins, draws);
            Console.WriteLine("{0} + {1} vs {2} + {3}", deck0,AI0, deck1, AI1);
            //Log("Press any key to continue");
            Console.ReadKey();
        }
    }
}
