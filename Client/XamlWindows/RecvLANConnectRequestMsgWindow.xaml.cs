using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LittleSheep.XamlWindows
{
    /// <summary>
    /// RecvLANConnectRequestMsgWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecvLANConnectRequestMsgWindow : Window
    {
        public RemoteUser remoteUser = new RemoteUser();

        public RecvLANConnectRequestMsgWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
        }

        private void AgreeBtn_Click(object sender, RoutedEventArgs e)
        {
            LANConnector.Instance.LANConnectReply(remoteUser, true);
            this.Close();
        }

        private void DeclineBtn_Click(object sender, RoutedEventArgs e)
        {
            LANConnector.Instance.LANConnectReply(remoteUser, false);
            this.Close();
        }
    }
}
