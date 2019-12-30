using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Train.Messages;

namespace Train.ShowMsg
{
    public class ListViewContent
    {
        private DateTime time;
        private int msgId;
        private _CommType commType;
        private AbstractRecvMessage arm = null; //arm和asm不能同时使用，懒得分开写成两个类
        private AbstractSendMessage asm = null;
        public ListViewContent() { }
        public ListViewContent(DateTime time,int msgId,_CommType commType,AbstractSendMessage asm): this(time, msgId, commType)
        {
            this.asm = asm;
        }
        public ListViewContent(DateTime time, int msgId, _CommType commType, AbstractRecvMessage arm) : this(time, msgId, commType)
        {
            this.arm = arm;
        }
        private ListViewContent(DateTime time, int msgId, _CommType commType)
        {
            this.time = time;
            this.msgId = msgId;
            this.commType = commType;
        }
        public DateTime Time
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }

        public int MsgId
        {
            get
            {
                return msgId;
            }

            set
            {
                msgId = value;
            }
        }

        public _CommType CommType
        {
            get
            {
                return commType;
            }

            set
            {
                commType = value;
            }
        }

        public AbstractRecvMessage Arm
        {
            get
            {
                return arm;
            }
        }

        public AbstractSendMessage Asm
        {
            get
            {
                return asm;
            }
        }
    }
}
