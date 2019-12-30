using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Packets;
using Train.Utilities;

namespace Train.Messages
{
    /// <summary>
    /// 地到车——授权调车模式
    /// </summary>
    public class Message028:AbstractRecvMessage
    {      
        public const int MESSAGEID = 28;
        uint T_TRAIN2;
        //AbstractPacket ap;          //可选择的信息包

        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24, 32 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }

            NID_MESSAGE = resultArray[0];
            L_MESSAGE = resultArray[1];
            T_TRAIN=(uint)(resultArray[2]);
            M_ACK = (resultArray[3] == 1);
            NID_LRBG = resultArray[4];
            T_TRAIN2 = Convert.ToUInt32(resultArray[5]);
        }

        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
