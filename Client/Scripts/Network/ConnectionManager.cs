using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace LittleSheep
{
    /// <summary>
    /// 总网络连接管理器
    /// </summary>
    class ConnectionManager
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly ConnectionManager instance = new ConnectionManager();
        }
        private ConnectionManager() {}
        public static ConnectionManager Instance { get { return Nested.instance; } }
        #endregion

        /// <summary>
        /// 局域网连接管理器
        /// </summary>
        readonly LANConnector lanConnectorInstance = LANConnector.Instance;

        /// <summary>
        /// 网络消息事件管理器
        /// </summary>
        public NetMsgHandler msgHandler = new NetMsgHandler();

        /// <summary>
        /// 与进行通信的远程用户
        /// </summary>
        public RemoteUser remoteUser { get; set; }

        private bool hasConnected = false;
        public bool HasConnected => hasConnected;

        /// <summary>
        /// 打开的功能页窗口
        /// </summary>
        XamlWindows.FunctionWindow window;

        /// <summary>
        /// 初始化客户端连接器
        /// </summary>
        public void Init()
        {
            lanConnectorInstance.msgHandler.AddMsgListener("ShutConnectionMsg", OnRecvShutConnectionMsg);

            InitFileModule();
        }

        public void Reset()
        {
            hasConnected = false;
            hasSendRequest = false;
        }

        //--------------------------------局域网报文---------------------------------

        //主动关闭连接
        public void ShutConnection()
        {
            if (!hasConnected) return;
            ShutConnectionMsg msg = new ShutConnectionMsg();
            lanConnectorInstance.UnicastMsg(msg, remoteUser);
            Reset();
        }

        public void OnRecvShutConnectionMsg(MsgBase msg, object[] obj)
        {
            this.Reset();
            DebugKit.MessageBoxShow("对方已主动断开连接", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            hasConnected = false;

            msgHandler.FireEvent(NetEvent.LANRemoteUserLostConnection);
        }

        //--------------------------------------------------------------------------

        #region 聊天模块

        /// <summary>
        /// 发送聊天内容
        /// </summary>
        /// <param name="content">内容</param>
        public void SendChattingMsg(string content)
        {
            ChattingMsg chattingMsg = new ChattingMsg();
            chattingMsg.sendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            chattingMsg.content = content;
            lanConnectorInstance.UnicastMsg(chattingMsg, remoteUser);
        }

        /// <summary>
        /// 发送聊天内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="sendTime">发送时间</param>
        public void SendChattingMsg(string content, string sendTime)
        {
            ChattingMsg chattingMsg = new ChattingMsg();
            chattingMsg.sendTime = sendTime;
            chattingMsg.content = content;
            lanConnectorInstance.UnicastMsg(chattingMsg, remoteUser);
        }

        #endregion

        #region 文件传输模块

        /// <summary>
        /// 监听TCP连接
        /// </summary>
        TcpListener fileListener = null;
        /// <summary>
        /// 进行TCP交流
        /// </summary>
        TcpClient fileClient = null;
        /// <summary>
        /// 文件传输模块是否已经被初始化过了
        /// </summary>
        bool fileModuleHasInit = false;
        /// <summary>
        /// 上一次收到的文件发送请求里表述的文件名和文件大小
        /// </summary>
        string tempFileName = "empty.txt", tempFileLength = "0B";
        /// <summary>
        /// 文件的保存位置
        /// </summary>
        string tempFileSavePath = "";
        /// <summary>
        /// 上一次收到的文件发送请求里表述的文件真实大小
        /// </summary>
        long tempFileTrueLength = 0;

        /// <summary>
        /// 初始化文件模块
        /// </summary>
        private void InitFileModule()
        {
            if (fileModuleHasInit) return;

            msgHandler.AddEventListener(NetEvent.LANAcceptConnectRequest, OnAcceptConnectRequest);
            msgHandler.AddEventListener(NetEvent.LANRecvAcceptConnectRequest, OnRecvAcceptConnectRequest);
            msgHandler.AddEventListener(NetEvent.FileSendStart, StartSendFile);
            msgHandler.AddEventListener(NetEvent.FileRecvStart, StartRecvFile);

            lanConnectorInstance.msgHandler.AddMsgListener("FileSendRequestMsg", OnRecvFileSendRequestMsg);
            lanConnectorInstance.msgHandler.AddMsgListener("FileSendReplyMsg", OnRecvFileSendReplyMsg);

            fileModuleHasInit = true;
        }

        /// <summary>
        /// 本机同意对方的连接请求，此时自己应该启动listener等待对方接下来的TCP连接
        /// </summary>
        /// <param name="err"></param>
        private void OnAcceptConnectRequest(string err)
        {
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                window = new XamlWindows.FunctionWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Show();
            }));

            hasConnected = true;
            Thread listenerThread = new Thread(FileListenerAcceptance);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        /// <summary>
        /// 本机收到了对方的同意连接情趣，此时自己应该使用Client向对方发起TCP连接请求
        /// </summary>
        /// <param name="err"></param>
        private void OnRecvAcceptConnectRequest(string err)
        {
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                window = new XamlWindows.FunctionWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Show();
            }));
            hasConnected = true;
            try
            {
                fileClient = new TcpClient(remoteUser.endPoint.Address.ToString(), 20718);
            }
            catch(Exception ex)
            {
                DebugKit.Error("[OnRecvAcceptConnectRequest]" + ex.Message);
                hasConnected = false;
            }
            
        }

        /// <summary>
        /// Listener监听Client的连接请求
        /// </summary>
        private void FileListenerAcceptance()
        {
            try
            {
                do
                {
                    fileListener = new TcpListener(IPAddress.Any, 20718);
                    fileListener.Start();

                    fileClient = fileListener.AcceptTcpClient();
                }
                while (fileClient.Client.RemoteEndPoint.ToString().Split(':')[0] != remoteUser.endPoint.Address.ToString());

                //DebugKit.Log("监听完毕");

                fileListener.Stop();
                fileListener = null;
            }
            catch(Exception ex)
            {
                DebugKit.Error("[FileListenerAcceptance]" + ex.Message);
                hasConnected = false;
            }
        }

        /// <summary>
        /// 开始接收文件
        /// </summary>
        /// <param name="err"></param>
        private void StartRecvFile(string err)
        {
            if(fileClient == null)
            {
                DebugKit.Warning("没有连结到TCP的远程设备，接收取消");
                return;
            }

            Thread thread = new Thread(RecvFileThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        /// 开始传输文件
        /// </summary>
        /// <param name="err"></param>
        private void StartSendFile(string err)
        {
            if(fileClient == null)
            {
                DebugKit.Warning("没有连接到TCP的远程设备,发送取消");
                return;
            }

            Thread thread = new Thread(SendFileThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void RecvFileThread()
        {
            string savePath = tempFileSavePath + "\\" + tempFileName;

            //如果此文件已存在
            if(File.Exists(savePath))
            {
                string[] parts = tempFileName.Split('.');
                parts[parts.Length - 2] += DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                savePath = tempFileSavePath + "\\" + string.Join(".", parts);
            }

            XamlWindows.DuringFileTransfer fileTransferWindow = null;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                fileTransferWindow = new XamlWindows.DuringFileTransfer();
                fileTransferWindow.Tips.Text = "正在接收文件" + UserInformation.Instance.sendFilePath;
                fileTransferWindow.Owner = window;
                window.fileTransferWindow = fileTransferWindow;
                fileTransferWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                fileTransferWindow.Show();
            }));

            try
            {
                NetworkStream networkStream = fileClient.GetStream();
                if(!Directory.Exists(tempFileSavePath))
                {
                    Directory.CreateDirectory(tempFileSavePath);
                }
                FileStream fileStream = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write);
                int fileReadSize = 0;
                long fileLength = 0;
                byte[] buffer = new byte[8192];
                while (fileLength < tempFileTrueLength) 
                {
                    fileReadSize = networkStream.Read(buffer, 0, buffer.Length);
                    fileLength += fileReadSize;
                    fileStream.Write(buffer, 0, fileReadSize);

                    App.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        fileTransferWindow.ProgressBar.Value = (double)fileLength / tempFileTrueLength * 100;
                        fileTransferWindow.FileTransferRatio.Text = $"{fileLength}B/{tempFileTrueLength}B 已接收";
                    }));
                }
                fileStream.Flush();
                fileStream.Close();
                networkStream.Flush();

                App.Current.Dispatcher.Invoke(new Action(delegate
                {
                    fileTransferWindow.Close();
                }));

                DebugKit.MessageBoxShow($"接收文件{tempFileName}完成", "提示");

                msgHandler.FireEvent(NetEvent.FileRecvFinish);

            }
            catch(Exception ex)
            {
                DebugKit.Error("[RecvFileThread]" + ex.Message);
                DebugKit.MessageBoxShow("接收文件出现异常，已停止接收文件。\n错误信息：" + ex.Message, "错误");
            }
      
        }

        private void SendFileThread()
        {
            XamlWindows.DuringFileTransfer fileTransferWindow = null;
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                fileTransferWindow = new XamlWindows.DuringFileTransfer();
                fileTransferWindow.Tips.Text = "正在发送文件" + UserInformation.Instance.sendFilePath;
                fileTransferWindow.Owner = window;
                window.fileTransferWindow = fileTransferWindow;
                fileTransferWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                fileTransferWindow.Show();
            }));

            try
            {
                NetworkStream networkStream = fileClient.GetStream();
                FileStream fileStream = new FileStream(UserInformation.Instance.sendFilePath, FileMode.Open);
                int fileReadSize = 0;
                long fileLength = 0;
                while (fileLength < fileStream.Length)
                {
                    byte[] buffer = new byte[8192];
                    fileReadSize = fileStream.Read(buffer, 0, buffer.Length);
                    networkStream.Write(buffer, 0, fileReadSize);
                    fileLength += fileReadSize;

                    App.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        fileTransferWindow.ProgressBar.Value = (double)fileLength / fileStream.Length * 100;
                        fileTransferWindow.FileTransferRatio.Text = $"{fileLength}B/{fileStream.Length}B 已发送";
                    }));
                }
                fileStream.Flush();
                networkStream.Flush();
                fileStream.Close();

                App.Current.Dispatcher.Invoke(new Action(delegate
                {
                    fileTransferWindow.Close();
                }));

                DebugKit.MessageBoxShow($"发送文件完成", "提示");

                msgHandler.FireEvent(NetEvent.FileSendFinish);
            }
            catch (Exception ex)
            {
                DebugKit.Error("[SendFileThread]" + ex.Message);
                DebugKit.MessageBoxShow("发送文件出现异常，已停止发送文件。\n错误信息：" + ex.Message, "错误");
            }
            
            //在接收完成后需要设置标识符
            hasSendRequest = false;
        }

        // --------------------------------文件传输局域网报文-----------------------------------

        bool hasSendRequest = false;
        public bool HasSendRequest => hasSendRequest;

        /// <summary>
        /// 发送文件传输请求报文
        /// </summary>
        public void FileSendRequest(string fileName, string fileLength,long fileTrueLength)
        {
            if (hasSendRequest)
            {
                DebugKit.Log("当前已有正在传输的文件");
                DebugKit.MessageBoxShow("当前已有正在传输的文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            FileSendRequestMsg fileSendRequestMsg = new FileSendRequestMsg();
            fileSendRequestMsg.fileName = fileName;
            fileSendRequestMsg.fileLength = fileLength;
            fileSendRequestMsg.fileTrueLength = fileTrueLength;
            try
            {
                lanConnectorInstance.UnicastMsg(fileSendRequestMsg, remoteUser);
                hasSendRequest = true;
                DebugKit.MessageBoxShow("已发送传输请求，请等待对方回复", "提示");
            }
            catch (Exception ex)
            {
                DebugKit.Warning("[FileSendRequest]" + ex.Message);
            }
        }

        /// <summary>
        /// 当接收到文件传输请求报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void OnRecvFileSendRequestMsg(MsgBase msg, object[] args)
        {
            FileSendRequestMsg fileSendRequestMsg = (FileSendRequestMsg)msg;
            //需要给对方回复是否同意文件传输
            FileSendReplyMsg fileSendReplyMsg = new FileSendReplyMsg();
            var result = DebugKit.MessageBoxShow($"收到了来自对方的文件传输请求:\n\n文件名:{fileSendRequestMsg.fileName}\n文件大小:{fileSendRequestMsg.fileLength}\n\n是否同意此传输请求?", "收到了文件请求", MessageBoxButton.YesNo);
            fileSendReplyMsg.permission = result == MessageBoxResult.Yes;

            if(fileSendReplyMsg.permission)
            {
                tempFileName = fileSendRequestMsg.fileName;
                tempFileLength = fileSendRequestMsg.fileLength;
                tempFileTrueLength = fileSendRequestMsg.fileTrueLength;

                //弹出选择文件夹窗口
                try
                {
                    App.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                        dialog.IsFolderPicker = true;
                        dialog.Title = "请选择文件保存位置";
                        dialog.Multiselect = false;
                        dialog.InitialDirectory = UserInformationCache.Default.LastFileSavePath;
                        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            if (string.IsNullOrEmpty(dialog.FileName))
                            {
                                DebugKit.MessageBoxShow("文件夹的路径不能为空\n已取消接收文件", "提示");
                                fileSendReplyMsg.permission = false;
                            }
                            tempFileSavePath = dialog.FileName;
                            UserInformationCache.Default.LastFileSavePath = dialog.FileName;

                            //启动接收文件
                            msgHandler.FireEvent(NetEvent.FileRecvStart);
                        }
                        else
                        {
                            DebugKit.MessageBoxShow("已取消接收文件", "提示");
                            fileSendReplyMsg.permission = false;
                        }
                    }));
                }
                catch(Exception ex)
                {
                    DebugKit.MessageBoxShow("弹出文件夹选择框失败:" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                /*System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "请选择文件保存位置";
                dialog.SelectedPath = UserInformationCache.Default.LastFileSavePath;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(dialog.SelectedPath))
                    {
                        DebugKit.MessageBoxShow("文件夹的路径不能为空\n已取消接收文件", "提示");
                        fileSendReplyMsg.permission = false;
                    }
                    tempFileSavePath = dialog.SelectedPath;
                    UserInformationCache.Default.LastFileSavePath = dialog.SelectedPath;

                    //启动接收文件
                    msgHandler.FireEvent(NetEvent.FileRecvStart);
                }
                else
                {
                    DebugKit.MessageBoxShow("已取消接收文件", "提示");
                    fileSendReplyMsg.permission = false;
                }*/
            }

            //把报文发出去
            try
            {
                lanConnectorInstance.UnicastMsg(fileSendReplyMsg, remoteUser);
            }
            catch (Exception ex)
            {
                DebugKit.Warning("[FileSendRequest]" + ex.Message);
            }
        }

        /// <summary>
        /// 当接收到文件传输请求回复报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void OnRecvFileSendReplyMsg(MsgBase msg, object[] args)
        {
            //这没发过请求咋还能收到回复捏
            if (!hasSendRequest) return;

            FileSendReplyMsg fileSendReplyMsg = (FileSendReplyMsg)msg;
            if(fileSendReplyMsg.permission)
            {
                //需要开始传输
                msgHandler.FireEvent(NetEvent.FileSendStart);
            }
            else
            {
                DebugKit.MessageBoxShow("对方拒绝了文件传输请求","太逊了");
                hasSendRequest = false;
            }
        }

        // -----------------------------------------------------------------------------------

        #endregion
    }
}
