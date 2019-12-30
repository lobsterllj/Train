using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——设置临时限速
    /// </summary>
    public class Packet065:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int NID_TSR;            //8bit
        int D_TSR;              //15bit
        int L_TSR;              //15bit
        bool Q_FRONT;           //1bit
        int V_TSR;              //7bit

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 8, 15, 15, 1, 7 };
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
            NID_TSR = resultArray[4];
            D_TSR = resultArray[5];
            L_TSR = resultArray[6];
            Q_FRONT = resultArray[7] == 1;
            V_TSR = resultArray[8];
        }

        public void SetValueTo(StaticSpeedLimits.TSR tsr)
        {
            tsr.Id = NID_TSR;
            double scale = GetScale(Q_SCALE);
            tsr.Length = L_TSR * scale;
            tsr.Distance = D_TSR * scale;
            tsr.Velocity = V_TSR * 5 / 3.6;
            tsr.Front = Q_FRONT;
            tsr.Qdir = Q_DIR;
        }

    }
}
