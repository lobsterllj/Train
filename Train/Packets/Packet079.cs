using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——地理位置信息
    /// </summary>
    public class Packet079:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        bool[] Q_NEWCOUNTRY;    //1bit
        int[] NID_C;            //10bit
        int[] NID_BG;           //14bit
        int[] D_POSOFF;         //15bit
        bool[] Q_MPOSITION;     //1bit
        int[] M_POSITION;       //20bit
        int N_ITER;             //5bit
        bool Q_NEWCOUNTRY_BASE;
        int NID_C_BASE;
        int NID_BG_BASE;
        int D_POSOFF_BASE;
        bool Q_MPOSITION_BASE;
        int M_POSITION_BASE;

        public const int LOC = 10;
        public const int ITER = 6;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 1, 10, 14, 15, 1, 20, 5, 1, 10, 14, 15, 1, 20 };
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

                if ((i == LOC) && (resultArray[LOC] == 0))
                {
                    i += ITER;
                    continue;
                }

                if (i == LOC + ITER)
                {
                    Q_NEWCOUNTRY = new bool[resultArray[LOC]];
                    NID_C = new int[resultArray[LOC]];
                    NID_BG = new int[resultArray[LOC]];
                    D_POSOFF = new int[resultArray[LOC]];
                    Q_MPOSITION = new bool[resultArray[LOC]];
                    M_POSITION = new int[resultArray[LOC]];

                    if (resultArray[LOC + 1] == 1)
                    {
                        Q_NEWCOUNTRY[0] = true;
                    }
                    NID_C[0] = resultArray[LOC + 2];
                    NID_BG[0] = resultArray[LOC + 3];
                    D_POSOFF[0] = resultArray[LOC + 4];
                    if (resultArray[LOC + 5] == 1)
                    {
                        Q_MPOSITION[0] = true;
                    }
                    M_POSITION[0] = resultArray[LOC + 6];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        Q_NEWCOUNTRY[j] = bitArray.Get(pos);
                        pos += 1;
                        if (Q_NEWCOUNTRY[j] == true)
                        {
                            NID_C[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 2]);
                        }
                        NID_BG[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 3]);
                        D_POSOFF[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 4]);
                        Q_MPOSITION[j] = bitArray.Get(pos);
                        pos += 1;
                        M_POSITION[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 6]);
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            if (resultArray[4] == 1)
            {
                Q_NEWCOUNTRY_BASE = true;
            }
            else
            {
                Q_NEWCOUNTRY_BASE = false;
            }
            NID_C_BASE = resultArray[5];
            NID_BG_BASE = resultArray[6];
            D_POSOFF_BASE = resultArray[7];
            if (resultArray[8] == 1)
            {
                Q_MPOSITION_BASE = true;
            }
            else
            {
                Q_MPOSITION_BASE = false;
            }
            M_POSITION_BASE = resultArray[9];
            N_ITER = resultArray[10];
        }
    }
}
