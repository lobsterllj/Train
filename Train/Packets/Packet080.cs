using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——引导/调车区段的设置信息
    /// </summary>
    public class Packet080:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int[] D_MAMODE;         //15bit
        int[] M_MAMODE;         //2bit
        int[] V_MAMODE;         //7bit
        int[] L_MAMODE;         //15bit
        int[] L_ACKMAMODE;      //15bit
        int N_ITER;             //5bit
        int D_MAMODE_BASE;
        int M_MAMODE_BASE;
        int V_MAMODE_BASE;
        int L_MAMODE_BASE;
        int L_ACKMAMODE_BASE;

        public const int LOC = 9;
        public const int ITER = 5;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 2, 7, 15, 15, 5, 15, 2, 7, 15, 15 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);

                if ((i == LOC) && (resultArray[LOC] == 0))
                {
                    i += ITER;
                    continue;
                }

                if (i == LOC + ITER)
                {
                    D_MAMODE = new int[resultArray[LOC]];
                    M_MAMODE = new int[resultArray[LOC]];
                    V_MAMODE = new int[resultArray[LOC]];
                    L_MAMODE = new int[resultArray[LOC]];
                    L_ACKMAMODE = new int[resultArray[LOC]];

                    D_MAMODE[0] = resultArray[LOC + 1];
                    M_MAMODE[0] = resultArray[LOC + 2];
                    V_MAMODE[0] = resultArray[LOC + 3];
                    L_MAMODE[0] = resultArray[LOC + 4];
                    L_ACKMAMODE[0] = resultArray[LOC + 5];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        D_MAMODE[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 1]);
                        M_MAMODE[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 2]);
                        V_MAMODE[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 3]);
                        L_MAMODE[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 4]);
                        L_ACKMAMODE[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 5]);
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            D_MAMODE_BASE = resultArray[4];
            M_MAMODE_BASE = resultArray[5];
            V_MAMODE_BASE = resultArray[6];
            L_MAMODE_BASE = resultArray[7];
            L_ACKMAMODE_BASE = resultArray[8];
            N_ITER = resultArray[9];
        }
    }
}
