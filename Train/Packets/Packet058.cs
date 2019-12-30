using System;
using System.Collections;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——位置报告参数
    /// </summary>
    public class Packet058:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int T_CYCLOC;           //8bit
        int D_CYCLOC;           //15bit
        int M_LOC;              //3bit
        int N_ITER;             //5bit
        int[] D_LOC;            //15bit
        bool[] Q_LGTLOC;        //1bit

        public const int LOC = 7;
        public const int ITER = 2;

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 8, 15, 3, 5 };
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
            T_CYCLOC = resultArray[4];
            D_CYCLOC = resultArray[5];
            M_LOC = resultArray[6];
            N_ITER = resultArray[7];
            D_LOC = new int[N_ITER];
            Q_LGTLOC = new bool[N_ITER];
            for (int i = 0; i < N_ITER; i++)
            {
                D_LOC[i]= Bits.ToInt(bitArray, ref pos, 15);
                Q_LGTLOC[i] = bitArray.Get(pos++);
            }
        }
        public int GetTcycLoc() { return T_CYCLOC; }
        public _M_LOC GetMloc() { return (_M_LOC)M_LOC; }
    }
    /// <summary>
    /// 列车必须报告其位置的特殊地点/时刻
    /// </summary>
    
    /*目前只在LocReport_MH中进行时间的周期判断，
     * 后面会将基于M_LOC的位置报告的判断加在TrainDynamics类中
     * （通号的M_LOC的值只会取1）
     */
    public enum _M_LOC
    {
        NOW=0,      //收到p58就发送
        EVERYBG=1,  //每经过一个BG时发送
        NOTBG=2,    //在每个BG地点停止发送
        LAST=3,     //使用前一个M_LOC的值
        UNUSED      //其它数值为未使用
    }
}
