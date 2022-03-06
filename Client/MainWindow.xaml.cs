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
            RootManager.Instance.GlobalInit();

            DebugKit.SetTextBox(debugString);
            userNameString.Text = UserInformationCache.Default.UserName;
            
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
            UserInformationCache.Default.UserName = userNameString.Text;
            UserInformationCache.Default.Save();
        }

        /// <summary>
        /// 关闭主窗口时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            RootManager.Instance.GlobalDestruct();
        }
    }
}
