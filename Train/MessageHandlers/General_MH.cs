using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Train.Messages;
using Train.Packets;
using Train.Utilities;

namespace Train.MessageHandlers
{
    /// <summary>
    /// 主要用来处理Message24及其中所带的信息包
    /// </summary>
    class General_MH: AbstractMessageHandler
    {
        //存放p72信息包中的纯文本信息，并在主界面上显示
        static CircularQueue<ShowPureText> pureText = new CircularQueue<ShowPureText>();
        public static CircularQueue<ShowPureText> PureText{get{return pureText;}}
        public General_MH(MessageHandler mh) : base(mh) { }

        public override bool Solve(AbstractRecvMessage arm)
        {
            int msgId = arm.GetMessageID();
            //通常消息24
            if (msgId == 24)
            {
                MH((Message024)arm);
                return true;
            }
            //RBC对列车数据的确认
            if (msgId == 8)
            {
                if (arm.M_ACK) //需要发送确认消息：M146
                {
                    SendAck(arm);
                }
                return true;
            }
            return false;
        }
        private void MH(Message024 m24)
        {
            if (m24.M_ACK) //需要发送确认消息：M146
            {
                SendAck(m24);
            }
            List<AbstractPacket> apList = m24.GetAlternativePacket();
            foreach(AbstractPacket ap in apList)
            {
                Type type = ap.GetType();
                if (type == typeof(Packet042)) PH((Packet042)ap);
                else if (type == typeof(Packet057)) PH((Packet057)ap);
                else if (type == typeof(Packet058)) PH((Packet058)ap);
                else if (type == typeof(Packet072)) PH((Packet072)ap);
                else if (type == typeof(Packet003)) PH((Packet003)ap);
                else if (type == typeof(Packet131)) PH((Packet131)ap);
                else if (type == typeof(Packet065)) PH((Packet065)ap);
                else if (type == typeof(Packet066)) PH((Packet066)ap);
                else
                {
                    DebugInfo.WriteToFile("Unhandled Packet in Message024:" + type.ToString(),"General_MH");
                }
            }
        }

        private void PH(Packet057 p57)
        {
            // if revd p57 from NRBC , then send M129 automatically
            if(mh.CommType == _CommType.NRBC)
            {
                Message129 m129 = new Message129();
                m129.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                SendMsg(m129);
            }
            mh.MhMa.p57 = p57;//交由MA_MH类处理
        }
        private void PH(Packet058 p58)
        {
            mh.MhLocReport.p58 = p58;//交由LocReport_MH类处理
        }

        // TSR
        public static void PH(Packet065 p65)
        {
            StaticSpeedLimits.TSR.Add(new StaticSpeedLimits.TSR(p65));
        }
        // TSR cancel
        public static void PH(Packet066 p66)
        {
            StaticSpeedLimits.TSR.Remove(p66.getNidTsr());
        }


        // RBC handover
        private void PH(Packet131 p131)
        {
            //开启一个线程处理
            Thread t = new Thread(new ParameterizedThreadStart(HandleHandover));
            t.IsBackground = true;
            t.Start(p131);
        }

        private void HandleHandover(object obj)
        {
            Packet131 p131 = (Packet131)obj;
            double dis = p131.GetDrbctr();
            TextInfo.Add("距离移交点" + dis + "米");
            JudgeDistance(dis);
            _CommType neighbor = mh.CommType == _CommType.RBC ? _CommType.NRBC : _CommType.RBC;
            TextInfo.Add("开始与" + neighbor + "建立通信");
            if (!Communication.IsConnected(neighbor))
            {
                Communication.Connect(neighbor);
                mainForm.StartRecvMsgModule(neighbor);
            }
            mainForm.SendMsg(new Message155(), neighbor);
        }

        private void PH(Packet072 p72)
        {
            if (pureText.IsFull())
                pureText.DecreaseToHalf();
            pureText.Push(new ShowPureText(p72.GetText(),DateTime.Now));
        }

        private void PH(Packet042 p42)
        {
            if (p42.Q_RBC == false) //终止通信会话
            {
                Message156 m156 = new Message156();//断开通信连接
                SendMsg(m156);
            }
            else //建立通信会话
            {

            }
        }

        private void PH(Packet003 p3)
        {
            TrainInfo.p3 = p3;  //将p3信息保存在TrainInfo中
            // if not in RBC handover procedure,then send M129
            if (mh.CommType == _CommType.RBC && mainForm.DriverConsoler.WorkMode != _M_MODE.FS)
            {
                Message129 m129 = new Message129();
                m129.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                SendMsg(m129);
            }
        }
    }
    /// <summary>
    /// 用于封装纯文本信息及相应的时间信息
    /// </summary>
    public class ShowPureText
    {
        string text;
        DateTime dt;
        public ShowPureText(string text,DateTime dateTime)
        {
            this.text = text;
            dt = dateTime;
        }
        public override string ToString()
        {
            return dt.ToString() + ":" + text;
        }
    }
}
