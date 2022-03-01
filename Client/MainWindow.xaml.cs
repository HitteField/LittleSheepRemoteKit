using System;
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

            Thread netConnection = new Thread(NetConnection);
            netConnection.Start();
        }

        public void NetConnection()
        {
            NetManager.Connect(NetManager.ConnectMethod.IPaddress, "127.0.0.1", 8888);
            while(true)
            {
                Thread.Sleep(2);
                NetManager.Update();
            }
        }
    }
}
