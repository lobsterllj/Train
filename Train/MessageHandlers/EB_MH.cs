using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Train.Messages;

namespace Train.MessageHandlers
{
    /// <summary>
    /// 处理与紧急停车相关的消息，列车冒进防护
    /// </summary>
    public class EB_MH : AbstractMessageHandler
    {
        Dictionary<int, AbstractRecvMessage> dictEB = new Dictionary<int, AbstractRecvMessage>();
        public EB_MH(MessageHandler mh) : base(mh) { }

        public override bool Solve(AbstractRecvMessage arm)
        {
            int msgId = arm.GetMessageID();
            //有条件紧急停车
            if (msgId == 15)
            {
                MH((Message015)arm);
                mainForm.TrainDynamic.DictEB = dictEB;
                return true;
            }
            //无条件紧急停车
            if (msgId == 16)
            {
                MH((Message016)arm);
                mainForm.TrainDynamic.DictEB = dictEB;
                return true;
            }
            //取消紧急停车
            if (msgId==18)
            {
                MH((Message018)arm);
                mainForm.TrainDynamic.DictEB = dictEB;
                return true;
            }
            //确认退出冒进防护模式
            if (msgId == 6)
            {
                if (arm.M_ACK) SendAck(arm);
                //退出PT模式，但不知道该进入什么模式，那就假设是OS模式吧！
                mainForm.BeginInvoke(new EventHandler(mainForm.rbWorkMode_CheckedChanged), _M_MODE.OS, null);
                return true;
            }
            return false;
        }
        private void MH(Message015 m15)
        {
            int nid_em = m15.GetNID_EM();
            dictEB.Add(nid_em, m15);
            //发送紧急停车确认消息147
            Message147 m147 = new Message147();
            m147.SetNID_EM(nid_em);
            m147.SetQ_ES(1);  //考虑有条件紧急停车 使用值1
            m147.SetAbstractPacket(Trains.TrainDynamics.GetPacket0());
            SendMsg(m147);
        }
        private void MH(Message016 m16)
        {
            int nid_em = m16.GetNID_EM();
            if (dictEB.ContainsKey(nid_em)) dictEB.Remove(nid_em);
            dictEB.Add(nid_em, m16);
            //发送紧急停车确认消息147
            Message147 m147 = new Message147();
            m147.SetNID_EM(nid_em);
            m147.SetQ_ES(2);  //无条件紧急停车 使用值2
            m147.SetAbstractPacket(Trains.TrainDynamics.GetPacket0());
            SendMsg(m147);
            //列车进入TR模式（注意要使用异步调用，防止发生阻塞）
            mainForm.BeginInvoke(new EventHandler(mainForm.rbWorkMode_CheckedChanged),_M_MODE.TR, null);
            //发送M136消息（这个消息应该是周期发送的，所以这里不需要再发送）
        }
        private void MH(Message018 m18)
        {
            int nid_em = m18.GetNID_EM();
            if (dictEB.ContainsKey(nid_em)) dictEB.Remove(nid_em);
            //发送确认消息146
            if (m18.M_ACK)
            {
                SendAck(m18);
            }
        }
    }
}
