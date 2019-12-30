using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Train.Utilities
{
    class Client
    {
        string localIP, remoteIP;
        int localPort, remotePort;
        IPEndPoint localEP, remoteEP;
        TcpClient tcpClient;
        NetworkStream networkStream;
        const int BUFFER_SIZE = 1024;
        byte[] buffer = new byte[BUFFER_SIZE];
        int lastReadLeftBytes;      //上次读取时多余的字节的数量（即发生了粘包）

        public Client(string localIP,int localPort, string remoteIP,int remotePort)
        {
            this.localIP = localIP;
            this.localPort = localPort;
            this.remoteIP = remoteIP;
            this.remotePort = remotePort;
            localEP = new IPEndPoint(IPAddress.Parse(localIP), localPort);
            remoteEP = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
        }

        public void SendMsg(byte[] byteToSend)
        {
            if (byteToSend == null) return;
            if (networkStream != null)
                networkStream.Write(byteToSend, 0, byteToSend.Length);
        }

        public byte[] RecvMsg()
        {
            byte[] recvBytes = null;
            if (networkStream == null) return recvBytes;
            int len = networkStream.Read(buffer, 0, buffer.Length);
            if (len == 0)   //可能是通信已经断开
            {
                Connect();  //不一定连得上
            }
            if (len > 0)
            {
                recvBytes = new byte[len];
                Array.Copy(buffer, recvBytes, len);
            }
            return recvBytes;
        }
        /// <summary>
        /// 接收到n个字节后返回
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public byte[] RecvMsg(int n)
        {
            if (networkStream == null) return null;
            byte[] recvBytes = new byte[n];
            int offset = lastReadLeftBytes;
            while (offset < n)
            {
                int len = networkStream.Read(buffer, offset, n - offset);
                offset += len;
            }
            Array.Copy(buffer, recvBytes, n);
            if (offset > n)  //接收到多余的字节
            {
                lastReadLeftBytes = offset - n;
                Array.Copy(buffer, n, buffer, 0, lastReadLeftBytes); //将多余的字节保存在buffer中
            }
            return recvBytes;
        }

        public void Connect()
        {
            if (tcpClient != null) Disconnect();
            try
            {
                //如果EP已经被使用，则会报错：AddressAlreadyInUse，ErrorCode：10048
                tcpClient = new TcpClient(localEP);
                tcpClient.NoDelay = true;
                //tcpClient.ExclusiveAddressUse = false;// permit reuse port
                //如果Server端未开启，则会报错：ConnectionRefused，ErrorCode：10061
                tcpClient.Connect(remoteEP);
                networkStream = tcpClient.GetStream();
            }
            catch (SocketException se)
            {
                //如果距离上次断开通信还未经过2MSL时间，则会报错：
                //通常每个套接字地址只能使用一次，ErrorCode：10048
                Thread.Sleep(1000); //2分钟
                MessageBox.Show(se.Message+"\r\nErrorCode"+se.ErrorCode,
                    "错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        public void Disconnect()
        {
            if (networkStream != null)
            {
                networkStream.Close();networkStream = null;
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
        }
        public bool Connected()
        {
            if (tcpClient != null) return tcpClient.Connected;
            return false;
        }
    }
}
