using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——调车模式区段的应答器列表
    /// </summary>
    public class Packet049:AbstractPacket
    {
        int Q_DIR;              //2bit
        int N_ITER;             //5bit
        bool[] Q_NEWCOUNTRY;    //1bit
        int[] NID_C;            //10bit
        int[] NID_BG;           //14bit

        public const int LOC = 3;
        public const int ITER = 3;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 5, 1, 10, 14 };
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
                    Q_NEWCOUNTRY = new bool[resultArray[LOC]];
                    NID_C = new int[resultArray[LOC]];
                    NID_BG = new int[resultArray[LOC]];

                    if (resultArray[LOC + 1] == 1)
                    {
                        Q_NEWCOUNTRY[0] = true;
                    }
                    NID_C[0] = resultArray[LOC + 2];
                    NID_BG[0] = resultArray[LOC + 3];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        Q_NEWCOUNTRY[j] = bitArray.Get(pos);
                        pos += 1;
                        if (Q_NEWCOUNTRY[j] == true)
                        {
                            NID_C[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 2]);
                        }
                        NID_BG[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 3]);
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            N_ITER = resultArray[3];
        }
    }
}
