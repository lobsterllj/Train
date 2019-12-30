using System;
using System.Collections;
using System.Text;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——链接，应答器组的链接信息
    /// </summary>
    public class Packet005:AbstractPacket
    {
        int Q_DIR;                  //2bit
        int Q_SCALE;                //2bit

        int N_ITER;                 //5bit
        //实际数量为N_ITER+1
        int[] D_LINK;               //15bit
        bool[] Q_NEWCOUNTRY;        //1bit
        int[] NID_C;                //10bit
        int[] NID_BG;               //14bit
        bool[] Q_LINKORIENTATION;   //1bit
        int[] Q_LINKREACTION;       //2bit
        int[] Q_LOCACC;             //6bit


        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2};
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
            int offset = pos + 15;
            if (bitArray[offset]) //judge Q_NEWCOUNTRY=1
                offset += 10;
            offset += 1 + 14 + 1 + 2 + 6;

            N_ITER = Bits.ToInt(bitArray, ref offset, 5);
            D_LINK = new int[N_ITER+1];
            Q_NEWCOUNTRY = new bool[N_ITER + 1];
            NID_C = new int[N_ITER + 1];
            NID_BG = new int[N_ITER + 1];
            Q_LINKORIENTATION = new bool[N_ITER + 1];
            Q_LINKREACTION = new int[N_ITER + 1];
            Q_LOCACC = new int[N_ITER + 1];
            for(int i = 0; i < N_ITER + 1; i++)
            {
                D_LINK[i] = Bits.ToInt(bitArray, ref pos, 15);
                Q_NEWCOUNTRY[i] = bitArray[pos++];
                if (Q_NEWCOUNTRY[i])
                    NID_C[i] = Bits.ToInt(bitArray, ref pos, 10);
                NID_BG[i] = Bits.ToInt(bitArray, ref pos, 14);
                Q_LINKORIENTATION[i] = bitArray[pos++];
                Q_LINKREACTION[i] = Bits.ToInt(bitArray, ref pos, 2);
                Q_LOCACC[i] = Bits.ToInt(bitArray, ref pos, 6);
                if (i == 0) pos += 5;   //加上N_ITER导致的偏移量
            }
            

       
        }
    }
}
