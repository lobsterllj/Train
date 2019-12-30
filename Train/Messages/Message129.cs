using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Packets;
using Train.Utilities;
using Train.Data;

namespace Train.Messages
{
    /// <summary>
    /// 车到地——经过确认的列车数据
    /// </summary>
    public class Message129: AbstractSendMessage
    {
        public const int MESSAGEID = 129;
                 
        AbstractPacket ap01;        //信息包0/1，必须设置
        static Packet011 p11 = new Packet011();

        public override byte[] Resolve()
        {
            int BitArrayLEN = 74;
            BitArray bit01 = ap01.Resolve();
            BitArray bit = p11.Resolve();
            BitArrayLEN += bit01.Length + bit.Length;
            BitArray bitArray = new BitArray(BitArrayLEN);
            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);
            NID_MESSAGE = MESSAGEID;

            int[] intArray = new int[] { 8, 10, 32, 24 };
            int[] DataArray = new int[] { NID_MESSAGE, L_MESSAGE, 0, NID_ENGINE };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                if (i == 2)
                {
                    Bits.ConvergeBitArray(bitArray, T_TRAIN, ref pos, intArray[i]);
                }
                else
                {
                    Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
                }
            }
            for (int i = 0; i < bit01.Length; i++)
            {
                bitArray[pos++] = bit01[i];
            }
            for (int i = 0; i < bit.Length; i++)
            {
                bitArray[pos++] = bit[i];
            }

            byte[] sendData = new byte[L_MESSAGE];
            Bits.ToByte(sendData, bitArray);

            return sendData;
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
        public void SetPacket0or1(AbstractPacket ap)
        {
            ap01 = ap;
        }
    }
}
