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
        bool hasFinished = false;
        public DuringFileTransfer()
        {
            InitializeComponent();

            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.LANRemoteUserLostConnection, CloseThisWindow);
            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.FileSendFinish, CloseThisWindow);
            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.FileRecvFinish, CloseThisWindow);
        }

        private void CloseThisWindow(string err)
        {
            hasFinished = true;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                Close();
            }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            ConnectionManager.Instance.msgHandler.RemoveEventListener(NetEvent.LANRemoteUserLostConnection, CloseThisWindow);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (hasFinished) return;
            DebugKit.MessageBoxShow("请不要在传输过程中关闭窗口", "一个不想解决Bug的临时解决办法", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Cancel = true;
        }
    }
}
