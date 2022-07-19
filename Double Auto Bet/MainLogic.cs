using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
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
        public static string user = "silviac@if.sc.usp.br";
        public static string pass = "Pq5Ftq9I";
        static async Task Main(string[] _)
        {
            await CommandHandler.initializeCommandsHandler();
            await SignalHandler.startTelegramHandler();
            BlazeHandler.start();
        }
    }
}
