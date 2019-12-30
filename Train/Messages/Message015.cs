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
    /// 地到车——有条件紧急停车
    /// </summary>
    public class Message015:AbstractRecvMessage
    {
        public const int MESSAGEID = 15;

        int NID_EM;                 //4bit
        int Q_SCALE;                //2bit
        int D_REF;                  //16bit
        int Q_DIR;                  //2bit
        int D_EMERGENCYSTOP;        //15bit

        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24, 4, 2, 16, 2, 15 };
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
            NID_EM = resultArray[5];
            Q_SCALE = resultArray[6];
            D_REF = resultArray[7];
            Q_DIR = resultArray[8];
            D_EMERGENCYSTOP = resultArray[9];
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
        public int GetNID_EM() { return NID_EM; }
    }
}
