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
    /// 地到车——确认前方轨道空闲
    /// </summary>
    public class Message034:AbstractRecvMessage
    {
       
        public const int MESSAGEID = 34;

        int Q_SCALE;                //2bit
        int D_REF;                  //16bit
        int Q_DIR;                  //2bit
        int D_TAFDISPLAY;           //15bit
        int L_TAFDISPLAY;           //15bit

        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24, 2, 16, 2, 15, 15 };
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
            if (resultArray[3] == 1)
            {
                M_ACK = true;
            }
            else
            {
                M_ACK = false;
            }
            NID_LRBG = resultArray[4];
            Q_SCALE = resultArray[5];
            D_REF = resultArray[6];
            Q_DIR = resultArray[7];
            D_TAFDISPLAY = resultArray[8];
            L_TAFDISPLAY = resultArray[9];
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
