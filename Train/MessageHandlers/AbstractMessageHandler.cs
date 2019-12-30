using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Train.Messages;

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

        protected void JudgeDistance(double dis)
        {
            TrainLocation trainLocation = mainForm.GetTrainState().TrainLocation;
            double startLoc = trainLocation.LeftLoc;
            const int ERROR = 1;
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                double curLoc = trainLocation.LeftLoc;
                double disRun = Math.Abs(curLoc - startLoc);
                if (Math.Abs(disRun - dis) < ERROR)
                    break;
                Thread.Sleep(10);
            }
        }

    }
}
