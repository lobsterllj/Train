using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Train.Messages;
namespace Train.MessageHandlers
{
    public class MessageHandler
    {
        private CommSession_MH mhCommSession ;
        private EB_MH mhEB ;
        private General_MH mhVersion ;
        private MA_MH mhMa ;
        private LocReport_MH mhLocReport ;
        private Shunt_MH mhShunt;
        private UltimateHandler_MH mhUltimateHandler ;

        public static MainForm mainForm;
        private _CommType commType;
        public MessageHandler(_CommType commType)
        {
            this.commType = commType;
        }

        //设置责任链
        public void Init(MainForm mf)
        {
            mainForm = mf;
            mhCommSession = new CommSession_MH(this);
            mhEB = new EB_MH(this);
            mhVersion = new General_MH(this);
            mhMa = new MA_MH(this);
            mhLocReport = new LocReport_MH(this);
            mhShunt = new Shunt_MH(this);
            mhUltimateHandler = new UltimateHandler_MH(this);
            mhCommSession.SetNext(mhEB).SetNext(mhVersion)
                .SetNext(mhMa).SetNext(mhLocReport)
                .SetNext(mhShunt)
                .SetNext(mhUltimateHandler).SetNext(null);
        }
        //处理接收到的报文
        public void Handling(Messages.AbstractRecvMessage arm)
        {
            AbstractMessageHandler amh = mhCommSession;
            FilterInfo(arm);
            while (!amh.Solve(arm))
                amh = amh.GetNext();
        }

        private void FilterInfo(AbstractRecvMessage arm)
        {
            List<Packets.AbstractPacket> list;
            dynamic dnm = arm;
            try
            {
                list = dnm.GetAlternativePacket();
            }catch(Exception e)
            {
                // Message has no that method,so just return.
                return;
            }
            // no info needed to record
            if (list == null || list.Count == 0)
                return;
            string msg = "received Message" + arm.GetMessageID() + " contained:";
            foreach(Packets.AbstractPacket ap in list)
            {
                msg += "Packet" + ap.NID_PACKET + " ";
            }
            Train.Utilities.TextInfo.Add(msg);
        }

        public _CommType CommType { get { return commType; } }
        public MA_MH MhMa { get { return mhMa; } }
        public LocReport_MH MhLocReport { get { return mhLocReport; } }
    }
}
