using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Train.Messages;
using Train.Packets;

namespace Train.MessageHandlers
{
    /// <summary>
    /// 通信会话相关
    /// </summary>
    class CommSession_MH: AbstractMessageHandler
    {
        public CommSession_MH(MessageHandler mh):base(mh){}
        public override bool Solve(AbstractRecvMessage am)
        {
            int msgId = am.GetMessageID();
            //系统版本，收到此消息后车载设备应认为通信会话已经建立 
            if (msgId == 32)     
            {
                //如果版本不一致
                if (!CheckVersion((Message032)am))
                {
                    //发送M154 版本不兼容
                    SendMsg(new Message154());
                    //发送M156 通信会话结束
                    SendMsg(new Message156());
                    return true;
                }
                //车载设备应发送消息159：通信会话已建立（含车载设备电话号码:Packet003Train）
                Message159 m159 = new Message159();
                m159.SetAlternativePacket(new Packet003Train());
                SendMsg(m159);
                _M_MODE mode = mainForm.DriverConsoler.WorkMode;
                // RBC handover and C2->C3 switch should not send M157,instead send M129
                if (mh.CommType == _CommType.NRBC || mode != _M_MODE.FS
                    && mode != _M_MODE.SN) 
                {
                    //接着发送M157：SoM位置报告（含位置信息：Packet000）
                    Message157 m157 = new Message157();
                    m157.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                    SendMsg(m157);
                }
                else  //处于FS模式而收到M32,说明是在RBC移交过程中
                {
                    Message129 m129 = new Message129();
                    m129.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                    SendMsg(m129);
                }
                return true;
            }
            //通信会话开始，这是由RBC 发起的
            if(msgId==38)
            {
                //车载设备应发送消息：通信会话已建立（不必发送车载设备电话号码）
                Message159 m159 = new Message159();
                SendMsg(m159);
                return true;
            }
            //通信会话结束确认
            if (msgId == 39)
            {
                Communication.Disconnect(mh.CommType);
                return true;
            }
            //拒绝列车
            if(msgId == 40)
            {
                Communication.Disconnect(mh.CommType);
                return true;
            }
            //接受列车
            if (msgId == 41)
            {
                if (am.M_ACK)
                {
                    SendAck(am);
                }
                //按SoM（任务开始）流程图，在收到M41后要发送M129
                //Message129 m129 = new Message129();
                //m129.SetPacket0or1(Trains.TrainDynamics.GetPacket0());
                //SendMsg(m129);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查RBC版本是否与车载版本一致
        /// </summary>
        /// <param name="m32"></param>
        /// <returns>true if consistent,else false</returns>
        public bool CheckVersion(Message032 m32)
        {
            int sysVersion = m32.GetVersion();
            return mainForm.IsVersionCompatible;
        }
    }
}
