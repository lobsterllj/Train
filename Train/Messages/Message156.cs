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
    /// 车到地——通信会话结束
    /// </summary>
    public class Message156: AbstractSendMessage
    {
        public const int MESSAGEID = 156;

        public override byte[] Resolve()
        {
            const int BitArrayLEN = 74;
            BitArray bitArray = new BitArray(BitArrayLEN);
            int[] intArray = new int[] { 8, 10, 32, 24 };
            NID_MESSAGE = MESSAGEID;
            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);
            int[] DataArray = new int[] { NID_MESSAGE, L_MESSAGE, (int)T_TRAIN, NID_ENGINE };
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
