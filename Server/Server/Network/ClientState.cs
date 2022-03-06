using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace LittleSheep
{
    /// <summary>
    /// �ͻ�����Ϣ
    /// </summary>
    public class ClientState
    {
        /// <summary>
        /// �ͻ����׽���
        /// </summary>
        public Socket socket;
        /// <summary>
        /// �ͻ��˵�IP��ַ�Ͷ˿���Ϣ
        /// </summary>
        public string remoteName = "";
        /// <summary>
        /// �ÿͻ��˵Ļ�����
        /// </summary>
        public ByteArray readBuff = new ByteArray();
        /// <summary>
        /// �ÿͻ�����һ�η�����������ʱ��
        /// </summary>
        public long lastPingTime = 0;

        #region �û���Ϣ

        /// <summary>
        /// �û���
        /// </summary>
        public string clientName = "";

        /// <summary>
        /// ���û���sid
        /// </summary>
        public int clientShareId = 0;

        #endregion
    }
}
