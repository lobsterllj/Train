using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Train.Messages;
using Train.Packets;
using Train.Utilities;

namespace Train.MessageHandlers
{
    /**
     *由于所有的消息处理都是在相应的RecvMsg线程中进行的，
     * 所以需要注意不要阻塞这个线程，以免延迟后续的消息处理。
     * （比如在某个函数里弹出一个MessageBox之类） 
     */
    public abstract class AbstractMessageHandler
    {
        protected AbstractMessageHandler next;
        public AbstractMessageHandler SetNext(AbstractMessageHandler amh) { return next = amh; }
        public AbstractMessageHandler GetNext() { return next; }
        /// <summary>
        /// 处理各类消息的函数
        /// </summary>
        /// <param name="am"></param>
        /// <returns>true表示消息被处理，false表示消息未被处理</returns>
        //子类必须覆盖这个方法
        public abstract bool Solve(Messages.AbstractRecvMessage am);

        protected MessageHandler mh;       //用于区分子类实例是用于RBC还是NRBC
        protected MainForm mainForm;
        protected AbstractMessageHandler(MessageHandler mh)
        {
            this.mh = mh;
            mainForm = MessageHandler.mainForm;
        }
        protected void SendMsg(AbstractSendMessage asm)
        {
            mainForm.SendMsg(asm, mh.CommType);
        }
        public void SendAck(AbstractRecvMessage arm)
        {
            Message146 m146 = new Message146();
            m146.T_TRAIN2 = arm.T_TRAIN;
            SendMsg(m146);
        }

        protected bool IsConnected()
        {
            if (mh.CommType == _CommType.RBC)
                return mainForm.IsRBCConnected && Communication.IsConnected(_CommType.RBC);
            if (mh.CommType == _CommType.NRBC)
                return mainForm.IsNRBCConnected && Communication.IsConnected(_CommType.NRBC);
            return false;
        }

        protected void JudgeDistance()
        {
            dynamic dnm = GetPacket();   // p41 or p131
            string str = (dnm is Packet041) ? "等级转换" : "RBC切换";
            double d_tr = dnm.GetDTr();
            double disToRun = d_tr - Trains.TrainDynamics.GetPacket0().D_LRBG;
            TextInfo.Add("LRBG距离" + str + "点" + d_tr + "m");
            TrainState trainState = mainForm.GetTrainState();
            TrainLocation trainLocation = trainState.TrainLocation;
            double startLoc = trainLocation.LeftLoc;
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                dnm = GetPacket();   // update packet
                if (dnm.GetDTr() != d_tr)
                {
                    d_tr = dnm.GetDTr();
                    TextInfo.Add("LRBG距离"+ str +"点" + d_tr + "m");
                    disToRun = d_tr - Trains.TrainDynamics.GetPacket0().D_LRBG;
                    startLoc = trainLocation.LeftLoc;
                }
                double curLoc = trainLocation.LeftLoc;
                double disRun = Math.Abs(curLoc - startLoc);
                if (disRun > disToRun)
                    break;
                Thread.Sleep(50);
            }
        }

        protected virtual AbstractPacket GetPacket()
        {
            return null;
        }

    }
}
