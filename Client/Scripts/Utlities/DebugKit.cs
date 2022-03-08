using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Net;
using Client;
using System.ComponentModel;
using System.Windows.Controls;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace LittleSheep
{
    public static class DebugKit
    {
        private static TextBox textBox;

        public static void SetTextBox(TextBox _textBox)
        {
            textBox = _textBox;
        }
        private static void Debug(string debugInformation)
        {
            string msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + debugInformation + "\n";

            textBox.AppendText(msg);
            textBox.ScrollToEnd();
        }

        public static void Log(string debugInformation)
        {
            textBox.Dispatcher.Invoke(new Action(delegate
            {
                string msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + "[Log]" + debugInformation + "\n";

                textBox.AppendText(msg);
                textBox.ScrollToEnd();
            }));
            //printDebug.BeginInvoke("[Log]" + debugInformation, null, null);
        }

        public static void Warning(string debugInformation)
        {
            textBox.Dispatcher.Invoke(new Action(delegate
            {
                string msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + "[Warning]" + debugInformation + "\n";

                textBox.AppendText(msg);
                textBox.ScrollToEnd();
            }));
            //printDebug.BeginInvoke("[Warning]" + debugInformation, null, null);
        }

        public static void Error(string debugInformation)
        {
            textBox.Dispatcher.Invoke(new Action(delegate
            {
                string msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + "[Error]" + debugInformation + "\n";

                textBox.AppendText(msg);
                textBox.ScrollToEnd();
            }));
            //printDebug.BeginInvoke("[Error]" + debugInformation, null, null);
        }

        public static MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                result = MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
            }));

            return result;
        }

        public static MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                result = MessageBox.Show(messageBoxText, caption, button, icon);
            }));

            return result;
        }

        public static MessageBoxResult MessageBoxShow(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                result = MessageBox.Show(messageBoxText, caption, button);
            }));

            return result;
        }

        public static MessageBoxResult MessageBoxShow(string messageBoxText, string caption)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                result = MessageBox.Show(messageBoxText, caption);
            }));

            return result;
        }
        public static MessageBoxResult MessageBoxShow(string messageBoxText)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                result = MessageBox.Show(messageBoxText);
            }));

            return result;
        }
    }
}