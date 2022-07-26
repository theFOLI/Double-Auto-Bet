﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace Double_Auto_Bet
{
    static class SignalHandler
    {
        public static User my;
        public static Thread teelegramConnectionKeeper;

        public static TimeSpan startBet;
        public static TimeSpan endBet;

        static Client client;
        static readonly Dictionary<long, User> Users = new Dictionary<long, User>();
        static readonly Dictionary<long, ChatBase> Chats = new Dictionary<long, ChatBase>();
        public static async Task startTelegramHandler()
        {
            await startTelegramClient();
        }
        public static async Task startTelegramClient()
        {
            client = new WTelegram.Client(Config);
            my = await client.LoginUserIfNeeded();
            Console.WriteLine($"We are logged-in as {my.username ?? my.first_name + " " + my.last_name} (id {my.id})");
            var chats = await client.Messages_GetAllChats();

            InputPeer peer = chats.chats[1511284561]; //million 2.0
            WTelegram.Helpers.Log = (lvl, str) => { };
            Console.WriteLine("we are now monitoring " + chats.chats[1511284561].Title);

            teelegramConnectionKeeper = new Thread(startListening);

            teelegramConnectionKeeper.Start();
        }

        public static async void startListening()
        {
            //get state
            client.Update += Client_Update;
            client.PingInterval = 10;

            while (true)
            {
                if (client.Disconnected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nServer connection lost");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nReconnecting...");
                    Console.ResetColor();
                    await client.ConnectAsync();
                    Thread.Sleep(200);
                    if (!client.Disconnected)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nServer connection reestablished");
                        Console.ResetColor();
                    }
                }

                Thread.Sleep(400);
            }
        }


        private static async Task readMessage(MessageBase messageBase, bool edit = false)
        {
            if (edit) Console.Write("(Edit): ");
            switch (messageBase)
            {
                case Message m:

                    if (m.Peer.ID.Equals(1399031811) || m.Peer.ID.Equals(5536625825))
                    {
                        if (m.message.Contains("ATENÇÃO LISTA BRANCO"))
                        {
                            string[] lines = m.message.Split('\n');

                            string firstWhite = Regex.Replace(lines[3], ".. ", "");
                            string secondWhite = Regex.Replace(lines[4], ".. ", "");
                            string thirdWhite = Regex.Replace(lines[5], ".. ", "");

                            startBet = TimeSpan.Parse(firstWhite);
                            endBet = TimeSpan.Parse(secondWhite);

                            Console.WriteLine($"\nNext bet avoid times are from {firstWhite} to {thirdWhite}");

                            Console.WriteLine($"\n{startBet} | {endBet}");
                        }
                    }

                    if (m.Peer.ID.Equals(1511284561))//1511284561
                    {
                        if (m.message.Contains("Sinal confirmado") && !IsBetween(System.DateTime.Now, startBet, endBet))
                        {
                            Color betColor = Color.Black;
                            Color startColor = Color.Black;

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nSinal confirmado\n");
                            

                            string[] lines = m.message.Split('\n');

                            string startNumber = Regex.Matches(lines[2], "\\d").Cast<Match>().Aggregate(string.Empty, (a, m) => a += m.Value);

                            if (lines[1].Contains("Vermelho")) betColor = Color.Red;
                            if (lines[1].Contains("Preto")) betColor = Color.Black;

                            if (lines[2].Contains("Vermelho"))
                            {
                                startColor = Color.Red;
                                Console.WriteLine("Were betting on " + betColor + " after Red (" + startNumber + ")");
                            }
                            if (lines[2].Contains("Preto"))
                            {
                                startColor = Color.Black;
                                Console.WriteLine("Were betting on " + betColor + " after Black (" + startNumber + ")");
                            }

                            Console.ResetColor();

                            await BlazeHandler.startBettingRoutine(startNumber, startColor, betColor);
                        }
                    }
                    else if (m.Peer.ID.Equals(5536625825)) Console.WriteLine("\n" + m.message + " SOU O MARCO" + "\n");
                    break;
            }
        }
        public static bool IsBetween(this DateTime now, TimeSpan start, TimeSpan end)
        {
            var time = now.TimeOfDay;
            // Scenario 1: If the start time and the end time are in the same day.
            if (start <= end)
                return time >= start && time <= end;
            // Scenario 2: The start time and end time is on different days.
            return time >= start || time <= end;
        }

        private static async void Client_Update(IObject arg)
        {
            if (!(arg is UpdatesBase updates)) return;
            updates.CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm: await readMessage(unm.message); break;
                }
            }
        }

        static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return "16142664";
                case "api_hash": return "c13b3322a68ecb88fe680987cbc535ac";
                case "phone_number": return "+5517981748182";
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                case "first_name": return "the";      // if sign-up is required
                case "last_name": return "FOLI";        // if sign-up is required
                case "password": return "secret!";     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }


    }
}
