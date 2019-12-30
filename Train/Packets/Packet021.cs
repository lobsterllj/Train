using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——坡度曲线
    /// </summary>
    public class Packet021:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int[] D_GRADIENT;       //15bit
        bool[] Q_GDIR;          //1bit
        int[] G_A;              //8bit
        int N_ITER;             //5bit
        int D_GRADIENT_BASE;
        bool Q_GDIR_BASE;
        int G_A_BASE;

        public const int LOC = 7;
        public const int ITER = 3;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 1, 8, 5, 15, 1, 8 };
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
                    D_GRADIENT = new int[resultArray[LOC]];
                    Q_GDIR = new bool[resultArray[LOC]];
                    G_A = new int[resultArray[LOC]];

                    D_GRADIENT[0] = resultArray[LOC + 1];
                    if (resultArray[LOC + 2] == 1)
                    {
                        Q_GDIR[0] = true;
                    }
                    G_A[0] = resultArray[LOC + 3];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        D_GRADIENT[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 1]);
                        Q_GDIR[j] = bitArray.Get(pos);
                        pos += 1;
                        G_A[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 3]);
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            D_GRADIENT_BASE = resultArray[4];
            if (resultArray[5] == 1)
            {
                Q_GDIR_BASE = true;
            }
            else
            {
                Q_GDIR_BASE = false;
            }
            G_A_BASE = resultArray[6];
            N_ITER = resultArray[7];
        }
    }
}
