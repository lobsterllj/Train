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
    /// 车到地——确认
    /// </summary>
    public class Message146 : AbstractSendMessage
    {
        public const int MESSAGEID = 146;

        public override byte[] Resolve()
        {
            const int BitArrayLEN = 8 + 10 + 32 + 24 + 32;
            BitArray bitArray = new BitArray(BitArrayLEN);
            NID_MESSAGE = MESSAGEID;
            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);
            int[] intArray = new int[] { 8, 10, 32, 24, 32 };
            //在调用Resolve函数前，需要先设置T_TRAIN2的值
            int[] DataArray = new int[] { NID_MESSAGE, L_MESSAGE, (int)T_TRAIN, NID_ENGINE, (int)T_TRAIN2 };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }
            byte[] sendData = new byte[L_MESSAGE];
            Bits.ToByte(sendData, bitArray);

            return sendData;
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
