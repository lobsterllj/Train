using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——RBC切换命令
    /// </summary>
    public class Packet131:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int D_RBCTR;            //15bit
        int NID_C;              //10bit
        int NID_RBC;            //14bit
        ulong NID_RADIO;       //64bit
        bool Q_SLEEPSESSION;    //1bit

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 10, 14};
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }
            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            D_RBCTR = resultArray[4];
            NID_C = resultArray[5];
            NID_RBC = resultArray[6];

            NID_RADIO = (ulong)Bits.ToLong(bitArray, ref pos, 64);
            Q_SLEEPSESSION = bitArray[pos++];           
        }

        public double GetDrbctr()
        {
            return D_RBCTR * GetScale(Q_SCALE);
        }
    }
}
