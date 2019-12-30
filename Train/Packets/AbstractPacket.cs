using System;
using System.Reflection;
using System.Collections;
using Train.Utilities;
using System.Collections.Generic;

namespace Train.Packets
{
    public abstract class AbstractPacket
    {
        private int nID_PACKET; //8bit
        private int l_PACKET;           //13bit

        public int NID_PACKET
        {
            get
            {
                return nID_PACKET;
            }

            set
            {
                nID_PACKET = value;
            }
        }

        public int L_PACKET
        {
            get
            {
                return l_PACKET;
            }

            set
            {
                l_PACKET = value;
            }
        }




        /// <summary>
        /// 地到车信息包需要覆盖此方法
        /// </summary>
        /// <param name="bitArray">存放收到的信息</param>
        public virtual void Resolve(BitArray bitArray)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 车到地信息包需要覆盖此方法
        /// </summary>
        /// <returns>返回组装好信息的BitArray</returns>
        public virtual BitArray Resolve()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 通过反射获取实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AbstractPacket GetPacket(int id)
        {
            String className = "Train.Packets.Packet" + String.Format("{0:D3}", id);
            Type t = Type.GetType(className);
            Object obj = t.GetConstructor(Type.EmptyTypes).Invoke(new Object[0]);
            AbstractPacket packet = (AbstractPacket)obj;
            return packet;
        }

        public override string ToString()
        {
            string s = "\r\n";
            Type t = this.GetType();
            //父类私有字段不好获取，改为获取其公有属性
            PropertyInfo[] p = t.GetProperties();
            foreach (PropertyInfo fi in p)
            {
                s += fi.Name + ":" + fi.GetValue(this) + "\r\n";
            }
            //获取子类非公有字段
            FieldInfo[] f = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic|BindingFlags.Public);
            foreach (FieldInfo fi in f)
            {
                s += fi.Name + ":";
                if (fi.FieldType.IsArray)//数组类型
                {
                    object tmp = fi.GetValue(this);
                    if (tmp == null) s += "null array";
                    else if (fi.FieldType == typeof(int[]))
                        s += (tmp as int[]).Length==0?"[]": string.Join(",", tmp as int[]);
                    else if (fi.FieldType == typeof(bool[]))
                        s += (tmp as bool[]).Length == 0 ? "[]" : string.Join(",", tmp as bool[]);
                }
                else if(fi.FieldType == typeof(List<int>))
                {
                    List<int> tmp = (List<int>)fi.GetValue(this);
                    s += "[";
                    if(tmp!=null && tmp.Count >= 1)
                    {
                        int i = 0;
                        for (i = 0; i < tmp.Count - 1; i++)
                            s += tmp[i] + ",";
                        s += tmp[i];
                    }
                    s += "]";
                }
                else if (fi.FieldType == typeof(List<ulong>))
                {
                    List<ulong> tmp = (List<ulong>)fi.GetValue(this);
                    s += "[";
                    if (tmp != null && tmp.Count >= 1)
                    {
                        int i = 0;
                        for (i = 0; i < tmp.Count - 1; i++)
                            s += string.Format("{0:x}", tmp[i]) + ",";
                        s += string.Format("{0:x}", tmp[i]);
                    }
                    s += "]";
                }
                else//值类型
                {
                    // only for NID_RADIO,to print it in BCD code
                    if(fi.FieldType==typeof(ulong))
                        s += string.Format("{0:x}", fi.GetValue(this));
                    else
                        s += fi.GetValue(this);
                }
                s+= "\r\n";
            }
            return s;
        }

        //一个经常会使用到的函数，暂放在父类中
        public double GetScale(int scale)
        {
            if (scale >= 3) throw new InvalidValueException("Q_SCALE="+scale);
            return 0.1 * Math.Pow(10, scale);
        }
    }
}
