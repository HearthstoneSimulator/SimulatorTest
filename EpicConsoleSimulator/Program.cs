﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            int draws = 0;
            int turnsTotal = 0;
            string deck0 = "random";
            string deck1 = "random";
            string AI0 = "face";
            string AI1 = "face";
            Log("Simulator started in Console Mode");
            //We check for number of arguments to start the simulator properly
            //No arguments lead to manual imput of required data
            try
            {


                if (args.Length == 0)
                {
                    Log("No starting parametres detected - please imput your preferences manually.");
                    Log("Input number of games you want to simulate.");
                    gamesToSimulate = Convert.ToInt32(Console.ReadLine());//10000;
                    Log("Input name of the first deck.");
                    deck0 = Console.ReadLine();
                    Log("Input name of the second deck");
                    deck1 = Console.ReadLine();
                    Log("Input name of the AI that shall play with the first deck.");
                    AI0 = Console.ReadLine();
                    Log("Input name of the AI that shall play with the second deck.");
                    AI1 = Console.ReadLine();

                    //deck0 = "basic_mage.txt";
                    //deck1 = "basic_mage.txt";
                }
                else
                {
                    //Incorrect ammount of arguments leads to manual imput of required data
                    if (args.Length != 5)
                    {
                        Log("Wrong number of arguments.");
                        Log("Please enter the arguments manually.");
                        Log("Input number of games you want to simulate.");
                        gamesToSimulate = Convert.ToInt32(Console.ReadLine());
                        Log("Input name of the first deck.");
                        deck0 = Console.ReadLine();
                        Log("Input name of the second deck");
                        deck1 = Console.ReadLine();
                        Log("Input name of the AI that shall play with the first deck.");
                        AI0 = Console.ReadLine();
                        Log("Input name of the AI that shall play with the second deck.");
                        AI1 = Console.ReadLine();
                    }
                    //We can load from arguments properly
                    else
                    {
                        //load number of games
                        gamesToSimulate = Convert.ToInt32(args[0]);
                        deck0 = args[1];
                        deck1 = args[2];
                        AI0 = args[3];
                        AI1 = args[4];

                    }
                    //Format of arguments is following:
                    //1 - number of games you want to play
                    //2 - name of file containing decklist - one card name per line - deck for you  
                    //3 - name of file containing decklist - one card name per line - deck for opponent
                    //4 - name of AI for your deck
                    //5 - name of AI for your opponents deck

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Wrong format of provided data. Follow instructions for valid input.");
                Environment.Exit(1);
                //throw;
            }
            //Now simulation parametres have been properly loaded
            //We set up game with simulation data provided

            #region Alternative simulation management for multithreaded simulation - not tested for benchmark because other simulators worked in single thread only
            /////////////////////////////////////////////////////////////
            /*
            System.Diagnostics.Stopwatch stopky = new System.Diagnostics.Stopwatch();
            var sync = new object();
            long totalGameTime = 0;
            long totalPrepTime = 0;
            int totalRuns = 100000;
            var tasks = new Task[Environment.ProcessorCount];
            int[] wins = new int[tasks.Length];
            int[] losses = new int[tasks.Length];
            int[] drawss = new int[tasks.Length];

            int runsPerTask = totalRuns / (Environment.ProcessorCount);

            for (int iii = 0; iii < tasks.Length; iii++)
            {
                tasks[iii] = Task.Factory.StartNew(() =>
                {
                    //var sw = Stopwatch.StartNew();
                    
                    GameIntestines.GameEngine NoneGame = new GameIntestines.GameEngine();
                    NoneGame.WriteDebugTexts = false;
                    NoneGame.deck0name = deck0;
                    NoneGame.deck1name = deck1;
                    //Preparing game loads card database and reduces time of each simulation
                    NoneGame.prepareGame();
                    
                    //InitializeGame();
                    //sw.Stop();

                    //lock (sync)
                    //{
                    //    totalPrepTime += sw.ElapsedMilliseconds;
                    //}

                    //sw.Reset();
                    //sw.Start();
                    for (int kkk = 0; kkk < runsPerTask; kkk++)
                    {
                        NoneGame.setStuff();
                        NoneGame.simulateOneGame();
                        //GameLogicStuff();
                    }


                    int winner = NoneGame.getWinner();
                    
                    if (winner == 0)
                    {
                        //Console.WriteLine("Game n.{0} result: player {1} wins.",i,winner);
                       // wins[iii]++;
                    }
                    if (winner == 1)
                    {
                        //Console.WriteLine("Game n.{0} result: player {1} wins.", i, winner);
                        //losses[iii]++;
                    }
                    if (winner == 3)
                    {
                        //Console.WriteLine("Game n.{0} result: draw.", i);
                        //drawss[iii]++;
                    }

                    //sw.Stop();

                    //lock (sync)
                    //{
                    //    totalGameTime += sw.ElapsedMilliseconds;
                    //}



                });
            }

            Task.WaitAll(tasks);
            // Do what you want
            for (int i = 0; i < tasks.Length; i++)
            {
                player0wins += wins[i];
                player1wins += losses[i];
                draws += drawss[i];
            }

            */
            /////////////////////////////////////////////////////////////
            ///////////////////**************
            #endregion

            try
            {
                var watch = Stopwatch.StartNew();
                var preptime = new Stopwatch();
                var gametime = new Stopwatch();
                gametime.Reset();
                preptime.Reset();

                System.Diagnostics.Stopwatch stopky = new System.Diagnostics.Stopwatch();
                GameIntestines.GameEngine oneGame = new GameIntestines.GameEngine();
                oneGame.WriteDebugTexts = false;
                oneGame.deck0name = deck0;
                oneGame.deck1name = deck1;
                oneGame.AI0name = AI0;
                oneGame.AI1name = AI1;
                //Preparing game loads card database and reduces time of each simulation
                oneGame.prepareGame();
                //We can now start measuring performance of simulated games
                Log("Performance measurement started.");
                stopky.Start();



                for (int i = 0; i < gamesToSimulate; i++)
                {

                    //Refreshing game data for next game
                    preptime.Start();
                    oneGame.setStuff();
                    preptime.Stop();

                    //start logging pure game time
                    gametime.Start();
                    try
                    {
                        oneGame.simulateOneGame();

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("There was an error during simulation of game number " + i);
                        continue;
                        //throw;
                    }
                    gametime.Stop();


                    //Logging winners
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

                    //loggingTurns
                    turnsTotal += oneGame.getTurns();
                }

                ///////////////////////////////////**************************

                //Simulation sucessfully simulated required ammount of games
                stopky.Stop();
                //We write simulation statistics on standard output
                Console.WriteLine("Total Time Elapsed: {0}", stopky.Elapsed);
                Console.WriteLine("Final score after {0} simulations: ", gamesToSimulate);
                Console.WriteLine("{0}/{1}/{2}", player0wins, player1wins, draws);
                Console.WriteLine("{0} + {1} vs {2} + {3}", deck0, AI0, deck1, AI1);

                //Console.WriteLine($"\n{watch.ElapsedMilliseconds}total \n{preptime.ElapsedMilliseconds}preparation \n{gametime.ElapsedMilliseconds}gametime \n{watch.ElapsedMilliseconds - preptime.ElapsedMilliseconds - gametime.ElapsedMilliseconds}difference");
                Console.WriteLine($"\n{watch.Elapsed} total \n{preptime.Elapsed} preparation \n{gametime.Elapsed} gametime \n{watch.Elapsed - preptime.Elapsed - gametime.Elapsed} difference");
                Console.WriteLine($"\n{turnsTotal} total turns elapsed");
                Console.ReadKey();
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error during the simulation. Simulation could not finish properly.");
                
                Environment.Exit(1);
               
            }
        }
    }
}
