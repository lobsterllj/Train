using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——等级转换命令
    /// </summary>
    public class Packet041:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int D_LEVELTR;          //15bit
        int[] M_LEVELTR;        //3bit
        int[] NID_STM;          //8bit
        int[] L_ACKLEVELTR;     //15bit
        int N_ITER;             //5bit
        int M_LEVELTR_BASE;
        int NID_STM_BASE;
        int L_ACKLEVELTR_BASE;

        public const int LOC = 8;
        public const int ITER = 3;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 3, 8, 15, 5, 3, 8, 15 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);

                if ((i == 5) && (resultArray[i] != 1))
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
                    M_LEVELTR = new int[resultArray[LOC]];
                    NID_STM = new int[resultArray[LOC]];
                    L_ACKLEVELTR = new int[resultArray[LOC]];

                    M_LEVELTR[0] = resultArray[LOC + 1];             
                    NID_STM[0] = resultArray[LOC + 2];
                    L_ACKLEVELTR[0] = resultArray[LOC + 3];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        M_LEVELTR[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 1]);
                        if (M_LEVELTR[j] == 1)
                        {
                            NID_STM[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 2]);
                        }
                        L_ACKLEVELTR[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 3]);
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            D_LEVELTR = resultArray[4];
            M_LEVELTR_BASE = resultArray[5];
            NID_STM_BASE = resultArray[6];
            L_ACKLEVELTR_BASE = resultArray[7];
            N_ITER = resultArray[8];
        }

        //获得等级转换点距LRBG的距离
        public int GetDLevelTr()
        {
            //直接取整，影响不大
            return (int)(D_LEVELTR * base.GetScale(Q_SCALE));
        }
        //返回应切换的等级
        public int GetMLevelTr()
        {
            return M_LEVELTR_BASE;
        }
    }
}
