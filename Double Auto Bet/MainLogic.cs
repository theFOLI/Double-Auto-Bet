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
            CommandHandler.initializeCommandsHandler();
            await SignalHandler.startTelegramHandler();
            BlazeHandler.start();
        }
    }
}
