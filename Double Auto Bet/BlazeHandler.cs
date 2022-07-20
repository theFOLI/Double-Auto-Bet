﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Double_Auto_Bet
{
    public enum Color
    {
        Red = 0,
        Black = 1,
        White = 2
    }


    class BlazeHandler
    {

        public static ChromeDriver driver;
        public static Thread monitorThread;
        public static Thread balanceMonitorThread;

        public static (string currentColor, string currentNumber) currentBlock;
        static Color awaitForColor;
        static string awaitForNumber;

        public static string balance;

        public static int betStartingValue;
        public static int galeCount;

        public static bool isDebugging = true;
        public static bool isVerbose = true;
        public static bool isShowingRolls = true;


        static int currentGale = 0;


        public static void start()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArguments(new List<string>()
            {
              "--disable-gpu",
              "--headless"
            });

            driver = new ChromeDriver(options);
            startBlaze();
        }

        static void startBlaze()
        {
            driver.Navigate().GoToUrl("https://blaze.com/games/double?modal=auth&tab=login");
            Thread.Sleep(4000);

            Console.WriteLine("logging in as " + MainLogic.user);

            driver.FindElement(By.XPath("/html/body/div[1]/main/div[3]/div/div[2]/div[2]/form/div[1]/div/input")).SendKeys(MainLogic.user);
            Thread.Sleep(200);
            driver.FindElement(By.XPath("/html/body/div[1]/main/div[3]/div/div[2]/div[2]/form/div[2]/div/input")).SendKeys(MainLogic.pass);
            Thread.Sleep(200);

            driver.FindElement(By.XPath("/html/body/div[1]/main/div[3]/div/div[2]/div[2]/form/div[4]/button")).Click();

            Thread.Sleep(3000);

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("We are monitoring telegram signals as ( " + SignalHandler.my.first_name + " " + SignalHandler.my.last_name + " )");

            Console.WriteLine("\nWe should be logged in to blaze.com as ( " + MainLogic.user + " )");

            //Console.WriteLine("\nPlease ensure you are properly logged in to blaze.com and press any key");

            //Console.ReadKey();

            Console.WriteLine("\nPlease enter the base bet value\n");

            while (!int.TryParse(Console.ReadLine(), out betStartingValue))
            {
                Console.WriteLine("\nPlease enter a valid value\n");
            }

            Console.WriteLine("\nPlease enter the number of gales between signals (we recomend doing 1 at max)\n");

            while (!int.TryParse(Console.ReadLine(), out galeCount))
            {
                Console.WriteLine("\nPlease enter a valid value\n");
            }

            if (isDebugging) Console.WriteLine("\nThis is a test version and is running on debug mode\n\nno actual betting is done on debug mode\n\nto disable debug mode type (debug 0)");

            if (isVerbose) Console.WriteLine("\nVerbose mode is on\n\nto disable verbose type (verbose 0)");

            if (isShowingRolls) Console.WriteLine("\nShowing recent rolls\n\nto disable this type (rolls 0)");

            Console.WriteLine("\nThe magic will now commence");

            CommandHandler.commandsThread.Start();

            Console.ResetColor();

            Thread.Sleep(4000);            

            balanceMonitorThread = new Thread(balanceMonitor);
            monitorThread = new Thread(blazeMonitor);

            balanceMonitorThread.Start();
            SignalHandler.startListening();
            monitorThread.Start();
        }

        public static async Task startBettingRoutine(string startNum, Color startColor, Color betColor)
        {
            int currentBet = betStartingValue;
            bool isGreen = false;
            if (currentGale > 0)
            {
                if (currentGale >= galeCount) currentGale = 0;
                currentBet *= 8;
            }
            if (await WaitForBet(startNum, startColor))
            {
                if (isVerbose)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nBet starting point found! \n\nBetting on " + betColor);
                    Console.ResetColor();
                }

                for (int i = 0; i < 3; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nbetting R$" + currentBet + " on " + betColor);
                    Console.ResetColor();

                    bool hit = await bet(betColor, currentBet);

                    if (hit)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nGREEN");
                        Console.ResetColor();
                        isGreen = true;
                        break;
                    }
                    else if (!hit)
                    {
                        currentBet *= 2;
                        if (isVerbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            if (i != 2) Console.WriteLine("\nIncreasing bet to R$" + currentBet);
                            Console.ResetColor();
                        }
                    }
                }

                if (!isGreen)
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nRED");
                    Console.ResetColor();

                    if (currentGale < galeCount)
                    {
                        currentGale += 1;
                    }
                }
            }
        }

        public static async Task<bool> bet(Color color, int value)
        {
            driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[1]/div[1]/div[2]/div[1]/div/div[1]/input")).SendKeys(value.ToString());

            Thread.Sleep(500);

            switch (color)
            {
                case Color.Red: driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[1]/div[1]/div[2]/div[2]/div/div[1]/div")).Click(); break;
                case Color.Black: driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[1]/div[1]/div[2]/div[2]/div/div[3]/div")).Click(); break;
            }

            await WaitForButton();

            if (!isDebugging) driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[1]/div[1]/div[3]/button")).Click();
            else if (isVerbose)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nclicked");
                Console.ResetColor();
            }

            await WaitForChange();

            if (currentBlock.currentColor.Equals("RED") && color.Equals(Color.Red)) return true;
            else if (currentBlock.currentColor.Equals("BLACK") && color.Equals(Color.Black)) return true;
            else return false;

        }

        public static async Task WaitForButton()
        {
            while (true)
            {
                if (driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[1]/div[1]/div[3]/button")).Text.Equals("Enter Round") 
                    || driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[1]/div[1]/div[3]/button")).Text.Equals("Começar o jogo")
                    ) break;

                Thread.Sleep(50);
            }
            Thread.Sleep(100);
        }

        public static async Task<bool> WaitForBet(string startNumber, Color startColor)
        {
            int tries = 0;


            (string lastColor, string lastNumber) lastBlock = currentBlock;

            (string lastColor, string lastNumber) targetBlock = (null, null);

            if (startColor.Equals(Color.Black)) targetBlock = ("BLACK", startNumber);
            if (startColor.Equals(Color.Red)) targetBlock = ("RED", startNumber);

            while (tries < 1)
            {
                if (currentBlock != lastBlock && currentBlock != targetBlock)
                {
                    lastBlock = currentBlock;
                    tries++;
                }
                else if (currentBlock == targetBlock) return true;

                Thread.Sleep(100);
            }
            return false;
        }

        public static async Task WaitForChange()
        {
            (string lastColor, string lastNum) lastBlock = currentBlock;

            while (true)
            {                
                if (lastBlock != currentBlock) break;                

                Thread.Sleep(100);
            }            
            Thread.Sleep(3000);
        }

        public static void blazeMonitor()
        {
            Color currentColor = Color.Black;
            IWebElement e = driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[2]/div[2]/div/div[1]/div[1]/div/div"));
            int lastHashCode = e.GetHashCode();
            string currentNumber;

            while (true)
            {
                IWebElement element = driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[4]/div/div[1]/div/div/div[1]/div[2]/div[2]/div/div[1]/div[1]/div/div"));

                if (element.GetHashCode() != lastHashCode)
                {
                    Thread.Sleep(2000);
                    if (element.GetAttribute("class").Equals("sm-box black")) currentColor = Color.Black;
                    if (element.GetAttribute("class").Equals("sm-box red")) currentColor = Color.Red;
                    if (element.GetAttribute("class").Equals("sm-box white")) currentColor = Color.White;


                    if (currentColor != Color.White) currentNumber = element.FindElement(By.XPath(".//*")).Text;
                    else currentNumber = "White";

                    if (currentColor.Equals(Color.Black)) currentBlock = ("BLACK", currentNumber);
                    if (currentColor.Equals(Color.White)) currentBlock = ("WHITE", currentNumber);
                    if (currentColor.Equals(Color.Red)) currentBlock = ("RED", currentNumber);


                    if (isShowingRolls)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\nBlaze rolled: " + currentBlock);
                        Console.ResetColor();
                    }

                    lastHashCode = element.GetHashCode();
                    Thread.Sleep(12500);
                }
                Thread.Sleep(2000);
            }
        }

        public static void balanceMonitor()
        {
            string currentBalance = driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[2]/div/div/div[2]/div/div[1]/div/a/div/div/div[1]")).Text;

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nStarting balance at: " + currentBalance);
            Console.ResetColor();

            while (true)
            {
                if (currentBalance != driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div[2]/div/div/div[2]/div/div[1]/div/a/div/div/div[1]")).Text)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nBalance currently at: " + currentBalance);
                    Console.ResetColor();
                }
                Thread.Sleep(1000);
            }
        }
    }
}
