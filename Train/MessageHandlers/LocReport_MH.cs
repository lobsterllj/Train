using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Train.Messages;
using Train.Packets;

namespace Train.MessageHandlers
{
    /// <summary>
    /// 处理发送列车位置报告
    /// </summary>
    public class LocReport_MH : AbstractMessageHandler
    {
        public Packet058 p58;
        int timer = 0;
        int lastLrbgId = 0;
        Thread thread = null;

        public LocReport_MH(MessageHandler mh):base(mh)
        {
            thread = new Thread(new ThreadStart(SendLocReport));
            thread.IsBackground = true;
            thread.Start();//在构造函数里启动一个线程不好，
        }
        public override bool Solve(AbstractRecvMessage arm)
        {

            return false;
        }
        public void SendLocReport()
        {
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                if (!IsConnected())
                    p58 = null;
                //列车还未收到p58包时，不需要周期判断发送位置报告
                if (p58 == null)
                {
                    timer = 0;
                    Thread.Sleep(100);
                    continue;
                }
                //目前先只考虑T_CYCLOC参数
                if (timer/10 >= p58.GetTcycLoc())
                {
                    SendM136();
                    timer = 0;//定时器清零
                }
                //基于M_LOC的判断
                if (p58.GetMloc() == _M_LOC.EVERYBG)
                {
                    if(Trains.TrainDynamics.GetCurrentLRBG().Nid_lrbg != lastLrbgId)
                    {
                        SendM136();
                        lastLrbgId = Trains.TrainDynamics.GetCurrentLRBG().Nid_lrbg;
                    }
                }
                Thread.Sleep(100);
                timer++;
            }
        }

        private void SendM136()
        {
            Message136 m136 = new Message136();
            m136.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
            SendMsg(m136);
        }

    }
}
