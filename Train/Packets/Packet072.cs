using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——发送纯文本消息的信息包
    /// </summary>
    public class Packet072:AbstractPacket
    {
        /**
         * 不知道M_LEVELTR取值从何处获得，暂时使用
         * m_leveltr=5（一个无效值）代替。
         *通号的实现版本是直接忽略掉两个NID_STM字段的。
         * 
         * 为简单起见，很多无用信息就没有解析，只解析有用的信息。
         */
        int L_TEXT;     //8bit，文本信息长度，每个中文字符占2个字符长度（通号）
        string X_TEXT;//8bit，文本信息，中文字符采用GB18030编码（通号）

        public override void Resolve(BitArray bitArray)
        {
            int m_leveltr = 5;
            int pos = 0;
            NID_PACKET = Bits.ToInt(bitArray, ref pos, 8);
            pos += 2 + 13 + 2 + 2 + 1 + 15 + 4 + 3 + 15 + 10 + 4 + 3 + 2;
            if (m_leveltr == 1)
                pos += 8 * 2;
            L_TEXT = Bits.ToInt(bitArray, ref pos, 8);
            byte[] text = new byte[L_TEXT];
            for (int i = 0; i < L_TEXT; i++)
                text[i] = (byte)Bits.ToInt(bitArray, ref pos, 8);
            //codePage:54936,name:GB18030
            X_TEXT = Encoding.UTF8.GetString(
                Encoding.Convert(Encoding.GetEncoding("GB18030"), Encoding.UTF8, text));
        }

        public string GetText()
        {
            return X_TEXT;
        }
    }
}
