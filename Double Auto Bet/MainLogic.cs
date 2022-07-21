using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace Double_Auto_Bet
{
    class MainLogic
    {
        public static string user = "ricardo.meglio@gmail.com";
        public static string pass = "Ri.2846999";
        static async Task Main(string[] _)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Please enter the email to log in to blaze");
            user = Console.ReadLine();
            Console.WriteLine("Please enter the password to log in to blaze");
            pass = Console.ReadLine();
            Console.ResetColor();


            CommandHandler.initializeCommandsHandler();
            await SignalHandler.startTelegramHandler();
            BlazeHandler.start();
        }
    }
}
