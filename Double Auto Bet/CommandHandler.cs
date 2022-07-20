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

        public static async Task initializeCommandsHandler()
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

                        break;

                    default: Console.WriteLine("( " + command + " ) is not a valid command"); break;
                }
            }
        }
    }
}
