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
    /// DuringFileTransfer.xaml 的交互逻辑
    /// </summary>
    public partial class DuringFileTransfer : Window
    {
        public DuringFileTransfer()
        {
            InitializeComponent();

            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.LANRemoteUserLostConnection, OnLostConnection);
        }

        private void OnLostConnection(string err)
        {
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                Close();
            }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
