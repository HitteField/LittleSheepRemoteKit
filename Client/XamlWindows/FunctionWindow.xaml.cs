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
    /// FunctionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionWindow : Window
    {
        public FunctionWindow()
        {
            InitializeComponent();

            FilePageTips.Text = $"当前连接的设备为{ConnectionManager.Instance.remoteUser}";
        }

        /// <summary>
        /// 按下选择文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if(ConnectionManager.Instance.HasSendRequest)
            {
                MessageBox.Show("已有一个正在处理的文件传输请求，禁止修改目录！\n（虽然修改了也没啥问题）", "警告");
                return;
            }
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "请选择要发送的文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ChooseFilePathString.Text = dialog.FileName;
                UserInformation.Instance.sendFilePath = dialog.FileName;
            }
        }

        /// <summary>
        /// 按下发送文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] parts = ChooseFilePathString.Text.Split('\\');
            string simpleFileName = parts.Last();
            long fileTrueLength = FileOperationKit.GetFileLength(ChooseFilePathString.Text);
            string fileLength = FileOperationKit.ConvertLength(fileTrueLength);
            ConnectionManager.Instance.FileSendRequest(simpleFileName, fileLength,fileTrueLength);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ConnectionManager.Instance.ShutConnection();
        }
    }
}
