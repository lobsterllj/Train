using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——静态速度曲线
    /// </summary>
    public class Packet027:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int[] D_STATIC;         //15bit
        int[] V_STATIC;         //7bit
        bool[] Q_FRONT;         //1bit
        int[] N_ITER;           //5bit
        int[][] NC_DIFF;        //4bit
        int[][] V_DIFF;         //7bit
        int D_STATIC_BASE;
        int V_STATIC_BASE;
        bool Q_FRONT_BASE;
        int N_ITER_BASE;
        int[] NC_DIFF_BASE;
        int[] V_DIFF_BASE;

        int N_ITER2;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 7, 1 };
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
            D_STATIC_BASE = resultArray[4];
            V_STATIC_BASE = resultArray[5];
            Q_FRONT_BASE = resultArray[6] == 1;

            N_ITER_BASE = Bits.ToInt(bitArray, ref pos, 5);
            NC_DIFF_BASE = new int[N_ITER_BASE];
            V_DIFF_BASE = new int[N_ITER_BASE];
            for (i = 0; i < N_ITER_BASE; i++)
            {
                NC_DIFF_BASE[i] = Bits.ToInt(bitArray, ref pos, 4);
                V_DIFF_BASE[i] = Bits.ToInt(bitArray, ref pos, 7);
            }
            N_ITER2 = Bits.ToInt(bitArray, ref pos, 5);
            D_STATIC = new int[N_ITER2];
            V_STATIC = new int[N_ITER2];
            Q_FRONT = new bool[N_ITER2];
            N_ITER = new int[N_ITER2];

            NC_DIFF = new int[N_ITER2][];
            V_DIFF = new int[N_ITER2][];
            for (i = 0; i < N_ITER2; i++)
            {
                D_STATIC[i] = Bits.ToInt(bitArray, ref pos, 15);
                V_STATIC[i] = Bits.ToInt(bitArray, ref pos, 7);
                Q_FRONT[i] = bitArray.Get(pos);
                pos++;
                N_ITER[i] = Bits.ToInt(bitArray, ref pos, 5);
                NC_DIFF[i] = new int[N_ITER[i]];
                V_DIFF[i] = new int[N_ITER[i]];
                for (int j = 0; j < N_ITER[i]; j++)
                {
                    NC_DIFF[i][j] = Bits.ToInt(bitArray, ref pos, 4);
                    V_DIFF[i][j] = Bits.ToInt(bitArray, ref pos, 7);
                }
            }
        }
    }
}
