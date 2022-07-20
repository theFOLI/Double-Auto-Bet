using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Double_Auto_Bet
{
    class CommandHandler
    {
        public static Thread commandsThread;

        public static void initializeCommandsHandler()
        {
            commandsThread = new Thread(commandsListener);
        }

        public static void commandsListener()
        {
            while (true)
            {
                string command = Console.ReadLine().ToLower();
                switch (command)
                {
                    case "startsimulation":

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nPlease enter a starting balance for your simulation\n");

                        while (!int.TryParse(Console.ReadLine(), out BlazeHandler.simulationBalance))
                        {
                            Console.WriteLine("\nPlease enter a valid value\n");
                        }

                        BlazeHandler.isSimulating = true;

                        Console.WriteLine("\nthe simulation has now started");

                        break;

                    case "stopsimulation":

                        BlazeHandler.isSimulating = false;

                        Console.WriteLine("\nthe simulation has finished with a balance of " + BlazeHandler.simulationBalance);

                        break;


                    case "clear":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This will clear the whole log\n" +
                            "Press Y to confirm\n" +
                            "Press any other key to cancel");


                        ConsoleKeyInfo response = Console.ReadKey();

                        if (response.Key == ConsoleKey.Y) Console.Clear();
                        else
                        {
                            Console.WriteLine("\b");
                            Console.WriteLine("Console.Clear() cancelled");
                        }
                        Console.ResetColor();

                        break;

                    case "changebet":
                    case "betvalue":

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDigite o novo valor\n");

                        while (!int.TryParse(Console.ReadLine(), out BlazeHandler.betStartingValue))
                        {
                            Console.WriteLine("\nPlease enter a valid value\n");
                        }

                        Console.WriteLine("\nBet value changed!");
                        Console.ResetColor();

                        break;

                    case "galecount":

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDigite o novo valor\n");

                        while (!int.TryParse(Console.ReadLine(), out BlazeHandler.galeCount))
                        {
                            Console.WriteLine("\nPlease enter a valid value\n");
                        }

                        Console.WriteLine("\nGale count changed!");
                        Console.ResetColor();

                        break;

                    case "debug 0":
                        BlazeHandler.isDebugging = false;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDebug mode off\n");
                        Console.ResetColor();                   
                        break;

                    case "debug 1":
                        BlazeHandler.isDebugging = true;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDebug mode on\n");
                        Console.ResetColor();
                        break;

                    case "verbose 0":
                        BlazeHandler.isVerbose = false;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nVerbose mode off\n");
                        Console.ResetColor();
                        break;

                    case "verbose 1":
                        BlazeHandler.isVerbose = true;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nVerbose mode on\n");
                        Console.ResetColor();
                        break;

                    case "rolls 0":
                        BlazeHandler.isShowingRolls = false;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nNo longer showing rolls\n");
                        Console.ResetColor();
                        break;

                    case "rolls 1":
                        BlazeHandler.isShowingRolls = true;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nShowing Rolls\n");
                        Console.ResetColor();
                        break;


                    default: Console.WriteLine("( " + command + " ) is not a valid command"); break;
                }
            }
        }
    }
}
