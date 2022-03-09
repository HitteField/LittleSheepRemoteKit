using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        public DuringFileTransfer fileTransferWindow = null;
        MediaPlayer mediaPlayer = null;
        public FunctionWindow()
        {
            InitializeComponent();

            LANConnector.Instance.msgHandler.AddMsgListener("ChattingMsg", OnRecvChattingMsg);
            LANConnector.Instance.msgHandler.AddMsgListener("FileTransferStopMsg", OnRecvFileTransferStopMsg);
            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.LANRemoteUserLostConnection, OnLostConnection);
            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.FileSendFinish, OnFileSendFinish);
            ConnectionManager.Instance.msgHandler.AddEventListener(NetEvent.FileRecvFinish, OnFileRecvFinish);

            FilePageTips.Text = $"当前连接的设备为{ConnectionManager.Instance.remoteUser}";
            ChattingDisplayer.Document.Blocks.Clear();
        }

        //-------------------------------聊天内容------------------------------------------

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(TabItem1.IsSelected)     //视频页被选择了
            {

            }
            else if(TabItem2.IsSelected)    //文件传输页被选择了
            {
                
            }
        }

        /// <summary>
        /// 在输入框按下了回车键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChattingSender_KeyDown(object sender, KeyEventArgs e)
        {
            //小键盘的enter是enter，大键盘的是return.........
            //无语
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Enter) || e.KeyStates == Keyboard.GetKeyStates(Key.Return))
            {
                //Ctrl + Enter 输入一个回车
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    //DebugKit.Log("输入了Ctrl+回车");
                    ChattingSender.Dispatcher.Invoke(new Action(delegate
                    {
                        int tempSelectionStart = ChattingSender.SelectionStart;
                        ChattingSender.Text += Environment.NewLine;
                        ChattingSender.SelectionStart = tempSelectionStart + Environment.NewLine.Length;
                    }));
                }
                else
                {
                    if (ChattingSender.Text.Length == 0) return;

                    string curTime = DateTime.Now.ToString("HH:mm:ss");
                    ConnectionManager.Instance.SendChattingMsg(ChattingSender.Text, curTime);
                    string str = $"本机 {curTime}:\r\n{ChattingSender.Text}";

                    ChattingDisplayerNewMsg(str, Colors.Green);

                    ChattingSender.Clear();
                }
            }
        }

        /// <summary>
        /// 按下了发送按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ChattingSender.Text.Length == 0) return;

            string curTime = DateTime.Now.ToString("HH:mm:ss");
            ConnectionManager.Instance.SendChattingMsg(ChattingSender.Text, curTime);
            string str = $"本机 {curTime}:\r\n{ChattingSender.Text}";

            ChattingDisplayerNewMsg(str, Colors.Green);

            ChattingSender.Clear();
        }

        /// <summary>
        /// 收到了聊天消息报文时
        /// </summary>
        /// <param name="msgBase"></param>
        /// <param name="args"></param>
        void OnRecvChattingMsg(MsgBase msgBase,Object[] args)
        {
            ChattingMsg chattingMsg = (ChattingMsg)msgBase;
            string str = $"对方 {chattingMsg.sendTime}:\r\n{chattingMsg.content}";
            ChattingDisplayerNewMsg(str, Colors.Red);
        }

        private void ChattingDisplayerNewMsg(string content,Color color)
        {
            int index = 0;
            do
            {
                index = content.IndexOf('\n', index);
                if (index == -1) break;
                content = content.Insert(++index, "   ");
                index += 2;
                if (index >= content.Length) break;
            }
            while (index != -1);

            ChattingDisplayer.Dispatcher.Invoke(new Action(delegate
            {
                Run run = new Run() { Text = content, Foreground = new SolidColorBrush(color) };
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(run);
                ChattingDisplayer.Document.Blocks.Add(paragraph);
                ChattingDisplayer.UpdateLayout();
                ChattingDisplayer.ScrollToEnd();
            }));
        }

        //--------------------------------------------------------------------------------

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
            /*System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "请选择要发送的文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ChooseFilePathString.Text = dialog.FileName;
                UserInformation.Instance.sendFilePath = dialog.FileName;
            }*/
            var dialog = new OpenFileDialog();
            dialog.Title = "请选择要发送的文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if(dialog.ShowDialog() == true)
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
            UserInformation.Instance.sendFilePath = ChooseFilePathString.Text;

            if(!File.Exists(ChooseFilePathString.Text))
            {
                DebugKit.MessageBoxShow("文件不存在！", "提示");
                return;
            }

            string[] parts = ChooseFilePathString.Text.Split('\\');
            string simpleFileName = parts.Last();
            ConnectionManager.Instance.tempFileName = simpleFileName;
            long fileTrueLength = FileOperationKit.GetFileLength(ChooseFilePathString.Text);
            string fileLength = FileOperationKit.ConvertLength(fileTrueLength);
            ConnectionManager.Instance.FileSendRequest(simpleFileName, fileLength, fileTrueLength);
        }

        /// <summary>
        /// 发送文件完成
        /// </summary>
        /// <param name="err"></param>
        private void OnFileSendFinish(string err)
        {
            ChattingDisplayerNewMsg(DateTime.Now.ToString("HH:mm:ss") + " 发送文件成功", Colors.Blue);
        }

        /// <summary>
        /// 接收文件完成
        /// </summary>
        /// <param name="err"></param>
        private void OnFileRecvFinish(string err)
        {
            ChattingDisplayerNewMsg(DateTime.Now.ToString("HH:mm:ss") + $" 接收文件成功", Colors.Blue);
        }

        /// <summary>
        /// 被强制关闭传输文件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void OnRecvFileTransferStopMsg(MsgBase msg,object[] args)
        {
            ChattingDisplayerNewMsg(DateTime.Now.ToString("HH:mm:ss") + $" 文件传输已被取消", Colors.Blue);
        }

        //----------------------------------------------------------------------------------

        /// <summary>
        /// 关闭窗口时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (fileTransferWindow != null) fileTransferWindow.Close();
            ConnectionManager.Instance.ShutConnection();

            RemoteListener();
        }

        /// <summary>
        /// 断开连接时
        /// </summary>
        /// <param name="err"></param>
        private void OnLostConnection(string err)
        {
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                Close();
            }));

            RemoteListener();
        }

        private void RemoteListener()
        {
            LANConnector.Instance.msgHandler.RemoveMsgListener("ChattingMsg", OnRecvChattingMsg);
            ConnectionManager.Instance.msgHandler.RemoveEventListener(NetEvent.LANRemoteUserLostConnection, OnLostConnection);
            ConnectionManager.Instance.msgHandler.RemoveEventListener(NetEvent.FileSendFinish, OnFileSendFinish);
            ConnectionManager.Instance.msgHandler.RemoveEventListener(NetEvent.FileRecvFinish, OnFileRecvFinish);

        }
    }
}
