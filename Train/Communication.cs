using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Train.Utilities;
using Train.XmlResolve;

namespace Train
{
    public enum _CommType
    {
        RBC,
        NRBC,
    }
    public class Communication
    {
        #region 通信参数配置
        //IP
        private static string trainIP;
        private static string train_nrbcIP;
        private static string rbcIP;
        private static string nrbcIP;
        //Port
        private static string train_rbcLocalPort;                //本端列车对RBC端口号
        private static string train_nrbcLocalPort;            //本端列车对NRBC端口号
        private static string rbcRemotePort;                      //对端RBC端口号
        private static string nrbcRemotePort;                //对端NRBC端口号
        //
        static Client train_rbc;
        static Client train_nrbc;
        #endregion

        //读取通信配置文件
        public static void ReadIniFile(string fileName)
        {
            IniFile file = new IniFile(fileName);
            if (file.ExistINIFile())
            {
                //IP
                trainIP = file.IniReadValue("IP", "TrainIP");
                train_nrbcIP = file.IniReadValue("IP", "Train_NRBCIP");
                rbcIP = file.IniReadValue("IP", "RBCIP");
                nrbcIP = file.IniReadValue("IP", "NRBCIP");
                //Port
                train_rbcLocalPort = file.IniReadValue("Port", "Train_RBCPort");
                rbcRemotePort = file.IniReadValue("Port", "RBCPort");
                train_nrbcLocalPort = file.IniReadValue("Port", "Train_NRBCPort");
                nrbcRemotePort = file.IniReadValue("Port", "NRBCPort");
                // CTCS ID
                string trainId = file.IniReadValue("CTCSID", "TrainId");
                TrainInfo.NID_ENGINE = Convert.ToInt32(trainId);
            }
            else
            {
                MessageBox.Show("通信配置文件不存在，请确认配置文件路径是否正确");
            }
        }
        //初始化通信连接
        public static void Init(string fileName)
        {
            ReadIniFile(fileName);
            train_rbc = new Client(trainIP, int.Parse(train_rbcLocalPort), rbcIP, int.Parse(rbcRemotePort));
            train_nrbc = new Client(train_nrbcIP, int.Parse(train_nrbcLocalPort), nrbcIP, int.Parse(nrbcRemotePort));
        }

        public static void Close()
        {
            Disconnect(_CommType.NRBC);
            Disconnect(_CommType.RBC);
        }
        public static void Connect(_CommType commType)
        {
            int ctcsid = TrainInfo.NID_ENGINE;
            switch (commType)
            {
                case _CommType.NRBC:
                    if(train_nrbc!=null) train_nrbc.Connect();
                    break;
                case _CommType.RBC:
                    if (train_rbc != null)
                    {
                        // if(train_rbc.Connected()==false)//if three-way handshake not established
                            train_rbc.Connect(); // if already connected,then dispose this socket pair,and renew one
                        byte[] toSend = XmlParser.ConnReq(ctcsid);
                        SendMsg(toSend, commType);
                    }
                    break;
            }
        }
       
        public static bool IsConnected(_CommType commType)
        {
            Client client = GetClient(commType);
            if (client != null) return client.Connected();
            return false;
        }

        public static void Disconnect(_CommType commType)
        {
            switch (commType)
            {
                case _CommType.NRBC:
                    if (train_nrbc != null) train_nrbc.Disconnect();
                    break;
                case _CommType.RBC:
                    if (train_rbc != null)
                    {
                        byte[] toSend = XmlParser.Disconnect();
                        SendMsg(toSend, commType);
                        train_rbc.Disconnect();
                    }
                    break;
            }
        }


        public static void SendMsg(byte[] byteToSend,_CommType commType)
        {
            switch (commType)
            {
                case _CommType.NRBC:
                    if (train_nrbc != null) train_nrbc.SendMsg(byteToSend);
                    break;
                case _CommType.RBC:
                    if (train_rbc != null)
                    {
                        byte[] b = new byte[2];
                        int len = byteToSend.Length;
                        b[1] = (byte)len;
                        b[0] = (byte)(len >> 8);
                        train_rbc.SendMsg(b); //先发送数据的长度
                        train_rbc.SendMsg(byteToSend);  //再发送实际的数据
                    }
                    break;
            }
        }
       
        public static byte[] RecvMsg(_CommType commType)
        {
            switch (commType)
            {
                case _CommType.NRBC:
                    if (train_nrbc != null)
                    {
                        byte[] len = train_nrbc.RecvMsg(2);
                        if (len == null) return null;
                        return train_nrbc.RecvMsg((len[0] << 8) | len[1]);
                    }
                    break; 
                case _CommType.RBC:
                    if (train_rbc != null)
                    {
                        byte[] recv = train_rbc.RecvMsg(2); //先接收待接收数据的长度
                        if (recv == null) return null;
                        return train_rbc.RecvMsg((recv[0] << 8) | recv[1]); //再接收实际数据
                    }
                    break;
            }
            return null;
        }

        private static Client GetClient(_CommType commType)
        {
            switch (commType)
            {
                case _CommType.NRBC:
                    return train_nrbc;
                case _CommType.RBC:
                    return train_rbc;
            }
            return null;
        }

    }
}