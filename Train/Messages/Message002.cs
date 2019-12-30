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
    /// 地到车——目视行车模式授权
    /// </summary>
    public class Message002:AbstractRecvMessage
    {
        public const int MESSAGEID = 2;

        int Q_SCALE;                //2bit
        int D_SR;                   //15bit
        AbstractPacket ap;          //可选择的信息包
        
        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24, 2, 15 };
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
            Q_SCALE = resultArray[5];
            D_SR = resultArray[6];

            bitArray = Bits.SubBitArray(bitArray, pos, bitArray.Length - pos);
            pos = 0;
            int ID = Bits.ToInt(bitArray, ref pos, 8); //NID_PACKET
            ap = AbstractPacket.GetPacket(ID);
            ap.Resolve(bitArray);
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
