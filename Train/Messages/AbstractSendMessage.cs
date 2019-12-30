using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Train.Messages
{
    /// <summary>
    /// 所有车载设备发送给RBC的消息的父类
    /// </summary>
    public abstract class AbstractSendMessage
    {
        private static readonly DateTime BASE_TIME = new DateTime(2019, 7, 1);
        private static uint lastTimestamp = 0;
        private int nID_MESSAGE; //8bit
        private int l_MESSAGE; //10bit
        private static int nID_ENGINE = TrainInfo.NID_ENGINE;//24bit
        //车载设备发送消息的时间戳，32bit,unit is 10ms
        private uint t_TRAIN; 
        private uint t_TRAIN2;//被确认消息的时间戳，32bit

        public AbstractSendMessage()
        {
            uint tmp = (uint)(DateTime.Now.Subtract(BASE_TIME).TotalMilliseconds / 10);
            // timestamp must be increasing.
            t_TRAIN = Math.Max(tmp, lastTimestamp + 1);
            lastTimestamp = t_TRAIN;
        }

        public  int NID_MESSAGE
        {
            get { return nID_MESSAGE; }
            protected set { nID_MESSAGE = value; }
        }
        public int L_MESSAGE
        {
            get { return l_MESSAGE; }
            protected set { l_MESSAGE = value; }
        }
        public static  int NID_ENGINE
        {
            get { return nID_ENGINE; }
            set { nID_ENGINE = value; }
        }
        public uint T_TRAIN
        {
            get { return t_TRAIN; }
        }

        public uint T_TRAIN2
        {
            get{ return t_TRAIN2; }
            set{ t_TRAIN2 = value; }
        }


        /// <summary>
        /// 车到地消息包需要覆盖此方法
        /// </summary>
        /// <returns>返回组装好信息的ByteArray</returns>
        public abstract byte[] Resolve();
        public virtual int GetMessageID()
        {
            throw new NotSupportedException();
        }
        public override string ToString()
        {
            string s = "";
            Type t = this.GetType();
            //父类私有字段不好获取，改为获取其公有属性
            PropertyInfo[] p = t.GetProperties();
            foreach (PropertyInfo fi in p)
            {
                //除了这3个类的其他类不显示T_TRAIN2
                if (fi.Name.Equals("T_TRAIN2") && !((this is Message137) || (this is Message138) || (this is Message146)))
                    continue;
                s += fi.Name + ":" + fi.GetValue(this) + "\r\n";
            }
            s += "NID_ENGINE:" + NID_ENGINE + "\r\n";
            //获取子类私有字段
            FieldInfo[] f = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (FieldInfo fi in f)
            {
                s += fi.Name + ":" + fi.GetValue(this) + "\r\n";
            }
            return s;
        }
    }
}
