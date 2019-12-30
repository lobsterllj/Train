using System;
using System.Collections;
using System.Linq;
using System.Text;
using Train.Packets;
using Train.Utilities;

namespace Train.Messages
{
    /// <summary>
    /// 地到车——系统版本
    /// </summary>
    public class Message032:AbstractRecvMessage
    {
        public const int MESSAGEID = 32;
        int M_VERSION;              //7bit

        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24, 7 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }

            NID_MESSAGE = resultArray[0];
            L_MESSAGE = resultArray[1];
            T_TRAIN = (uint)(resultArray[2]);
            if (resultArray[3] == 1)
            {
                M_ACK = true;
            }
            else
            {
                M_ACK = false;
            }
            NID_LRBG = resultArray[4];
            M_VERSION = resultArray[5];
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
        public int GetVersion()
        {
            return M_VERSION;
        }
    }
}
