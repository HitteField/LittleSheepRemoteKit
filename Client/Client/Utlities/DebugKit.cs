using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Net;
using Client;
using System.ComponentModel;
using System.Windows.Controls;
using System.Text;

namespace LittleSheep
{
    public static class DebugKit
    {
        private static TextBox textBox;

        public static void SetTextBox(TextBox _textBox)
        {
            textBox = _textBox;
        }
        public static void Debug(string debugInformation, ConsoleColor consoleColor = ConsoleColor.White)
        {
            //Console.ForegroundColor = consoleColor;
            Log(debugInformation);
            //Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Log(string debugInformation)
        {
            string msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + debugInformation + "\n";
            textBox.AppendText(msg);
            textBox.ScrollToEnd();
        }

        public static void Warning(string debugInformation)
        {
            Debug("[Warning]" + debugInformation, ConsoleColor.Yellow);
        }

        public static void Error(string debugInformation)
        {
            Debug("[Error]" + debugInformation, ConsoleColor.Red);
        }
    }
}