using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——轴重速度曲线
    /// </summary>
    public class Packet051:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        bool Q_TRACKINIT;       //1bit
        int[] D_AXLELOAD;       //15bit
        int[] L_AXLELOAD;       //15bit
        bool[] Q_FRONT;         //1bit
        int[] N_ITER;           //5bit
        int[][] M_AXLELOAD;     //7bit
        int[][] V_AXLELOAD;     //7bit 若列车轴重>=M_AXLELOAD，则使用速度限制
        int D_AXLELOAD_BASE;
        int L_AXLELOAD_BASE;
        bool Q_FRONT_BASE;
        int N_ITER_BASE;
        int[] M_AXLELOAD_BASE;
        int[] V_AXLELOAD_BASE;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 1, 15, 15, 1 };
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
            if (resultArray[4] == 1)
            {
                Q_TRACKINIT = true;
            }
            else
            {
                Q_TRACKINIT = false;
            }
            D_AXLELOAD_BASE = resultArray[5];
            L_AXLELOAD_BASE = resultArray[6];
            if (resultArray[7] == 1)
            {
                Q_FRONT_BASE = true;
            }
            else
            {
                Q_FRONT_BASE = false;
            }

            N_ITER_BASE = Bits.ToInt(bitArray, ref pos, 5);
            M_AXLELOAD_BASE = new int[N_ITER_BASE];
            V_AXLELOAD_BASE = new int[N_ITER_BASE];
            for (i = 0; i < N_ITER_BASE; i++)
            {
                M_AXLELOAD_BASE[i] = Bits.ToInt(bitArray, ref pos, 7);
                V_AXLELOAD_BASE[i] = Bits.ToInt(bitArray, ref pos, 7);
            }
            N_ITER_BASE = Bits.ToInt(bitArray, ref pos, 5);
            D_AXLELOAD = new int[N_ITER_BASE];
            L_AXLELOAD = new int[N_ITER_BASE];
            Q_FRONT = new bool[N_ITER_BASE];
            N_ITER = new int[N_ITER_BASE];
            for (i = 0; i < N_ITER_BASE; i++)
            {
                D_AXLELOAD[i] = Bits.ToInt(bitArray, ref pos, 15);
                L_AXLELOAD[i] = Bits.ToInt(bitArray, ref pos, 15);
                Q_FRONT[i] = bitArray.Get(pos);
                pos++;
                N_ITER[i] = Bits.ToInt(bitArray, ref pos, 5);
                M_AXLELOAD[i] = new int[N_ITER[i]];
                V_AXLELOAD[i] = new int[N_ITER[i]];
                for (int j = 0; j < N_ITER[i]; j++)
                {
                    M_AXLELOAD[i][j] = Bits.ToInt(bitArray, ref pos, 7);
                    V_AXLELOAD[i][j] = Bits.ToInt(bitArray, ref pos, 7);
                }
            }
        }
    }
}
