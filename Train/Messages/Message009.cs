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
    /// 地到车——请求缩短MA
    /// </summary>
    public class Message009 : AbstractRecvMessage
    {
        public const int MESSAGEID = 9;
        Packet015 p15 = new Packet015();
        AbstractPacket ap;          //可选择的信息包

        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24 };
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
            M_ACK = resultArray[3] == 1;
            NID_LRBG = resultArray[4];

            bitArray = Bits.SubBitArray(bitArray, pos, bitArray.Length - pos);
            p15.Resolve(bitArray);
            pos = p15.GetPacketLength();
            int ID = 80;
            ap = AbstractPacket.GetPacket(ID);
            bitArray = Bits.SubBitArray(bitArray, pos, bitArray.Length - pos);
            ap.Resolve(bitArray);
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
