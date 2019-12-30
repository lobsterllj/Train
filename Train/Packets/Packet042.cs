using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——通信会话管理
    /// </summary>
    public class Packet042:AbstractPacket
    {
        int Q_DIR;              //2bit
        public bool Q_RBC;             //1bit
        int NID_C;              //10bit
        int NID_RBC;            //14bit
        ulong NID_RADIO;         //64bit
        bool Q_SLEEPSESSION;    //1bit

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 1, 10, 14 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int pos = 0;
            for (int i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_RBC = resultArray[3] == 1;
            NID_C = resultArray[4];
            NID_RBC = resultArray[5];
            NID_RADIO = (ulong)Bits.ToLong(bitArray,ref pos,64);
            Q_SLEEPSESSION = bitArray[pos++];
        }
    }
}
