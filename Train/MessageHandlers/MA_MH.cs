using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Train.Packets;
using Train.Messages;
using System.Windows.Forms;
using Train.Utilities;

namespace Train.MessageHandlers
{
    /// <summary>
    /// 处理MA相关报文信息
    /// </summary>
    public class MA_MH : AbstractMessageHandler
    {
        public Packet057 p57;
        Thread maRequestThread = null;
        int timer = 0;
        public MA_MH(MessageHandler mh):base(mh)
        {
            maRequestThread = new Thread(new ThreadStart(SendMARequest));
            maRequestThread.IsBackground = true;
            //it is not a good idea to start a thread in constructors
            maRequestThread.Start();
        }
        public override bool Solve(AbstractRecvMessage arm)
        {
            int msgId = arm.GetMessageID();

            if (msgId == 3 || msgId==33)
            {
                MH(arm);
                return true;
            }
            //确认前方轨道空闲
            if(msgId == 34)
            {
                if (arm.M_ACK)
                    SendAck(arm);
                //发送前方轨道空闲确认
                Message149 m149 = new Message149();
                m149.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                SendMsg(m149);
                //应该还要加上模式切换（OS、FS等），目前还不知道怎么做
                return true;
            }
            return false;
        }

        //处理M3或M33消息
        void MH(AbstractRecvMessage msg)
        {
            dynamic dnm = msg;  //不想写两个函数分别处理，就用了dynamic
            if (msg.M_ACK)
            {
                SendAck(msg);
            }
            foreach (AbstractPacket ap in dnm.GetAlternativePacket())
            {
                Type type = ap.GetType();
                if (type == typeof(Packet041)) PH((Packet041)ap);
                else if (type == typeof(Packet015)) PH((Packet015)ap);
                else if (type == typeof(Packet021)) PH((Packet021)ap);
                else if (type == typeof(Packet027)) PH((Packet027)ap);
                else if (type == typeof(Packet068)) PH((Packet068)ap);
                else if (type == typeof(Packet005)) PH((Packet005)ap);
                else if (type == typeof(Packet065)) General_MH.PH((Packet065)ap);
                else if (type == typeof(Packet066)) General_MH.PH((Packet066)ap);
                else if (type == typeof(Packet003)) PH((Packet003)ap);
                else
                {
                    DebugInfo.WriteToFile("Unhandled Packet in Message003:" + type.ToString(), "MA_MH");
                }
            }
        }
        
        void PH(Packet015 p15)
        {
            MA ma = new MA();
            ma.GetValueFrom(p15);
            mainForm.TrainDynamic.Ma = ma;
        }
        private Packet041 p41;
        // p41 will be received many times
        void PH(Packet041 p41)
        {
            if (this.p41 == null)
            {
                this.p41 = p41;  // set value before thread start.
                //开启一个线程处理等级切换
                Thread t = new Thread(new ThreadStart(HandleC2C3Switch));
                t.IsBackground = true;
                t.Start();
            }
            this.p41 = p41;
        }
        void PH(Packet021 p21){}
        void PH(Packet027 p27){}
        void PH(Packet005 p5){}
        void PH(Packet068 p68) { TextInfo.Add(p68.GetText()); }

        // 在C2->C3等级转换流程中，M3中会包含p3
        void PH(Packet003 p3)
        {
            TrainInfo.p3 = p3;
        }

        // 处理ATP等级转换C2->C3 或 C3->C2
        private void HandleC2C3Switch()    // 必须用object作为形参类型
        {
            JudgeDistance();
            TextInfo.Add("开始切换到" + (p41.GetMLevelTr()==1?"CTCS2":"CTCS3") + "等级");
            if (FaultInjection.IsNoLevelTr)
            {
                TextInfo.Add("故障注入,不执行等级转换");
                return;
            }
            if (p41.GetMLevelTr()==1)//switch to C2
            {
                mainForm.BeginInvoke(new EventHandler(mainForm.rbControlLevel_CheckedChanged),_ControlLevel.CTCS_2, null);
            }else if (p41.GetMLevelTr() == 3)//switch to C3
            {
                mainForm.BeginInvoke(new EventHandler(mainForm.rbControlLevel_CheckedChanged), _ControlLevel.CTCS_3, null);
                mainForm.BeginInvoke(new EventHandler(mainForm.rbWorkMode_CheckedChanged), _M_MODE.FS, null);
            }
            p41 = null;
        }

        protected override AbstractPacket GetPacket()
        {
            return p41;
        }


        //根据p57包周期发送MA请求的线程
        public void SendMARequest()
        {
            while(Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                if (!IsConnected())
                    p57 = null;
                //列车还未收到p57包时，不需要周期判断MA请求
                if (p57 == null)
                {
                    timer = 0;
                    Thread.Sleep(100);
                    continue;
                }
                //目前先只考虑T_CYCRQST参数
                if (timer == 0 )
                {
                    Message132 m132 = new Message132();
                    m132.SetTrackdel(false);//未删除线路描述
                    m132.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                    SendMsg(m132);
                    timer = p57.GetTcycRqst();//定时器清零
                }
                Thread.Sleep(1000);
                timer--;
            }
        }

    }

}
