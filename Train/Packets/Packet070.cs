using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——进路适合性数据
    /// </summary>
    public class Packet070:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        bool Q_TRACKINIT;       //1bit
        int D_TRACKINIT;        //15bit
        int[] D_SUITABILITY;    //15bit
        int[] Q_SUITABILITY;    //2bit
        int[] M_LOADINGGAUGE;   //8bit
        int[] M_AXLELOAD;       //7bit
        int[] M_TRACTION;       //8bit
        int N_ITER;             //5bit
        int D_SUITABILITY_BASE;
        int Q_SUITABILITY_BASE;
        int M_LOADINGGAUGE_BASE;
        int M_AXLELOAD_BASE;
        int M_TRACTION_BASE;

        public const int LOC = 11;
        public const int ITER = 5;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 1, 15, 15, 2, 8, 7, 8, 5, 15, 2, 8, 7, 8 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;

            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);

                if ((i == 4) && (resultArray[i] != 1))
                {
                    i++;
                    continue;
                }
                if (i == 5)
                {
                    break;
                }
                else if (i == 7)
                {
                    if (resultArray[i] == 3)
                    {
                        i += 3;
                        continue;
                    }
                    else if (resultArray[i] == 2)
                    {
                        i += 2;
                        continue;
                    }
                    else if (resultArray[i] == 1)
                    {
                        i += 1;
                        continue;
                    }
                }
                else if (i == 8)
                {
                    i += 2;
                    continue;
                }
                else if (i == 9)
                {
                    i += 1;
                    continue;
                }
                

                if ((i == LOC) && (resultArray[LOC] == 0))
                {
                    i += ITER;
                    continue;
                }

                if (i == LOC + ITER)
                {
                    D_SUITABILITY = new int[resultArray[LOC]];
                    Q_SUITABILITY = new int[resultArray[LOC]];
                    M_LOADINGGAUGE = new int[resultArray[LOC]];
                    M_AXLELOAD = new int[resultArray[LOC]];
                    M_TRACTION = new int[resultArray[LOC]];

                    D_SUITABILITY[0] = resultArray[LOC + 1];
                    Q_SUITABILITY[0] = resultArray[LOC + 2];
                    M_LOADINGGAUGE[0] = resultArray[LOC + 3];
                    M_AXLELOAD[0] = resultArray[LOC + 4];
                    M_TRACTION[0] = resultArray[LOC + 5];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        D_SUITABILITY[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 1]);
                        Q_SUITABILITY[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 2]);
                        if (Q_SUITABILITY[j] == 0)
                        {
                            M_LOADINGGAUGE[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 3]);
                        }
                        else if (Q_SUITABILITY[j] == 1)
                        {
                            M_AXLELOAD[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 4]);
                        }
                        else if (Q_SUITABILITY[j] == 2)
                        {
                            M_TRACTION[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 5]);
                        }
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            if (resultArray[4] == 1)
            {
                Q_TRACKINIT = true;
            }
            else
            {
                Q_TRACKINIT = false;
            }
            D_TRACKINIT = resultArray[5];
            D_SUITABILITY_BASE = resultArray[6];
            Q_SUITABILITY_BASE = resultArray[7];
            M_LOADINGGAUGE_BASE = resultArray[8];
            M_AXLELOAD_BASE = resultArray[9];
            M_TRACTION_BASE = resultArray[10];
            N_ITER = resultArray[11];
        }
    }
}
