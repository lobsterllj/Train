using System;
using System.Collections;
using Train.Packets;
using Train.Utilities;
using Train.Data;

namespace Train.Messages
{
    /// <summary>
    /// 车到地——通信会话已建立
    /// </summary>
    public class Message159: AbstractSendMessage
    {
        public const int MESSAGEID = 159;
        AbstractPacket ap;          //可选择的信息包

        public override byte[] Resolve()
        {
            int BitArrayLEN = 8 + 10 + 32 + 24;
            BitArray bit = null;
            if (ap != null) bit = ap.Resolve();
            if (bit != null) BitArrayLEN += bit.Length;
            BitArray bitArray = new BitArray(BitArrayLEN);
            int[] intArray = new int[] { 8, 10, 32, 24 };

            NID_MESSAGE = MESSAGEID;
            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);

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
            for (int i = 0;bit!=null && i < bit.Length; i++)
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
        public void SetAlternativePacket(AbstractPacket ap)
        {
            this.ap = ap;
        }
    }
}
