﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LittleSheep;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DebugKit.SetTextBox(debugString);
            
        }

        private void InitLan_Click(object sender, RoutedEventArgs e)
        {
            LANConnector.Instance.Initialization();
        }

        private void StartRecvBoardcast_Click(object sender, RoutedEventArgs e)
        {
            if(LANConnector.Instance.OpenToLan == false)
            {
                LANConnector.Instance.OpenToLan = true;
                startRecvBoardcastButton.Content = "关闭接收广播";
            }
            else
            {
                LANConnector.Instance.OpenToLan = false;
                startRecvBoardcastButton.Content = "开启接收广播";
            }
        }

        private void sendBoardcast_Click(object sender, RoutedEventArgs e)
        {
            LANConnector.Instance.LANProbeRequest();
        }

        private void userNameString_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserInformation.Instance.Username = userNameString.Text;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FirewallOperation.NetFwDelApps(20713, System.Net.Sockets.ProtocolType.Udp);
            FirewallOperation.NetFwDelApps(20714, System.Net.Sockets.ProtocolType.Udp);
        
        }
    }
}
