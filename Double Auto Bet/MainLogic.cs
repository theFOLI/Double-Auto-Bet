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
        public static string user = "assisreginaldo1966@outlook.com";
        public static string pass = "theFOLI3654";
        static async Task Main(string[] _)
        {
            CommandHandler.initializeCommandsHandler();
            await SignalHandler.startTelegramHandler();
            BlazeHandler.start();
        }
    }
}
