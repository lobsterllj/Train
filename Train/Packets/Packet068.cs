using System;
using System.Collections;
using System.Text;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——线路条件
    /// </summary>
    public class Packet068:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        bool Q_TRACKINIT;       //1bit
        int D_TRACKINIT;        //15bit
        int D_TRACKCOND_BASE; //15bit
        int L_TRACKCOND_BASE;  //15bit
        int M_TRACKCOND_BASE;   //4bit

        int N_ITER;             //5bit
        int[] D_TRACKCOND;      //15bit
        int[] L_TRACKCOND;      //15bit 应忽略应答器信息完整性检查报警的距离
        int[] M_TRACKCOND;      //4bit


        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 1};
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int pos = 0;

            for (int i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }
            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            Q_TRACKINIT = resultArray[4] == 1;

            if (Q_TRACKINIT)
                D_TRACKINIT = Bits.ToInt(bitArray, ref pos, 15);
            else
            {
                D_TRACKCOND_BASE = Bits.ToInt(bitArray, ref pos, 15);
                L_TRACKCOND_BASE = Bits.ToInt(bitArray, ref pos, 15);
                M_TRACKCOND_BASE = Bits.ToInt(bitArray, ref pos, 4);
                N_ITER = Bits.ToInt(bitArray, ref pos, 5);
                D_TRACKCOND = new int[N_ITER];
                L_TRACKCOND = new int[N_ITER];
                M_TRACKCOND = new int[N_ITER];
                for(int i = 0; i < N_ITER; i++)
                {
                    D_TRACKCOND[i] = Bits.ToInt(bitArray, ref pos, 15);
                    L_TRACKCOND[i] = Bits.ToInt(bitArray, ref pos, 15);
                    M_TRACKCOND[i] = Bits.ToInt(bitArray, ref pos, 4);
                }
            }
           
        }
    }
}
