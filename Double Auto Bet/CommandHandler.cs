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

                    default: Console.WriteLine("( " + command + " ) is not a valid command"); break;
                }
            }
        }
    }
}
