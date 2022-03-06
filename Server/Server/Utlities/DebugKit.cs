using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Net;

namespace LittleSheep
{
    public static class DebugKit
    {
        public static void Debug(string debugInformation, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            Log(debugInformation);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Log(string debugInformation)
        {
            string msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm-ss]") + debugInformation;
            Console.WriteLine(msg);
        }

        public static void Warning(string debugInformation)
        {
            Debug(debugInformation, ConsoleColor.Yellow);
        }

        public static void Error(string debugInformation)
        {
            Debug(debugInformation, ConsoleColor.Red);
        }
    }
}