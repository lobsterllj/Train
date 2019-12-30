using System;
using System.Collections.Generic;
using System.Reflection;
using Train.Packets;

namespace Train.Messages
{
    /// <summary>
    /// 所有从RBC接收到的消息的父类
    /// </summary>
    public abstract class AbstractRecvMessage
    {
        private int nID_MESSAGE; //8bit
        private int l_MESSAGE; //10bit
        private uint t_TRAIN;//32bit
        private bool m_ACK;//1bit
        private int nID_LRBG;//24bit

        public int NID_MESSAGE
        {
            get { return nID_MESSAGE; }
            protected set { nID_MESSAGE = value; }
        }
        public int L_MESSAGE
        {
            get { return l_MESSAGE; }
            protected set { l_MESSAGE = value; }
        }
        public uint T_TRAIN
        {
            get { return t_TRAIN; }
            protected set { t_TRAIN = value; }
        }
        public bool M_ACK
        {
            get { return m_ACK; }
            protected set { m_ACK = value; }
        }
        public int NID_LRBG
        {
            get { return nID_LRBG; }
            protected set { nID_LRBG = value; }
        }




        /// <summary>
        /// 地到车消息包需要覆盖此方法，只是提供一个接口
        /// </summary>
        /// <param name="recvData">接收到的信息</param>
        public abstract void Resolve(byte[] recvData);
        

        /// <summary>
        /// 用于返回Message实例的方法
        /// </summary>
        /// <param name="recvData">接收到的信息字节</param>
        /// <returns></returns>
        public static AbstractRecvMessage GetMessage(byte[] recvData)
        {
            int id = recvData[0];
            String className = "Train.Messages.Message" + String.Format("{0:D3}", id);
            Type t = Type.GetType(className);
            Object obj = t.GetConstructor(Type.EmptyTypes).Invoke(new Object[0]);
            AbstractRecvMessage message = (AbstractRecvMessage)obj;
            message.Resolve(recvData);
            return message;
        }
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
                s += fi.Name + ":" + fi.GetValue(this) + "\r\n";
            }
            //获取子类私有字段
            FieldInfo[] f = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo fi in f)
            {
                if(fi.FieldType == typeof(List<AbstractPacket>))
                {
                    s += "\r\n" + fi.Name + ":\r\n";
                    List<AbstractPacket> tmp = (List<AbstractPacket>)fi.GetValue(this);
                    if(tmp != null && tmp.Count >= 1)
                    {
                        foreach(AbstractPacket ap in tmp)
                        {
                            s += "Packet" + String.Format("{0:D3}", ap.NID_PACKET)+":";
                            s += ap.ToString() + "\r\n";
                        }
                    }
                }
                else
                {
                    s += fi.Name + ":" + fi.GetValue(this) + "\r\n";
                }
            }
            return s;
        }
    }
}
