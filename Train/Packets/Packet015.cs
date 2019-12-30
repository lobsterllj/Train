using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——CTCS-3级的行车许可
    /// </summary>
    public class Packet015:AbstractPacket
    {
        int Q_DIR;                  //2bit
        int Q_SCALE;                //2bit
        int V_LOA;                  //7bit
        int T_LOA;                  //10bit
        int N_ITER;                 //5bit
        int[] L_SECTION;            //15bit
        bool[] Q_SECTIONTIMER;      //1bit
        int[] T_SECTIONTIMER;       //10bit
        int[] D_SECTIONTIMERSTOPLOC;//15bit
        int L_ENDSECTION;           //15bit
        bool Q_ENDSECTIONTIMER;       //末区段有效时间信息
        int T_ENDSECTIONTIMER;
        int D_ENDSECTIONTIMERSTOPLOC;
        bool Q_ENDTIMER;            //1bit     末区段保持时间信息
        int T_ENDTIMER;             //10bit
        int D_ENDTIMERSTARTLOC;     //15bit
        bool Q_DANGERPOINT;         //1bit
        int D_DP;                   //15bit
        int V_RELEASEDP;            //7bit
        bool Q_OVERLAP;             //1bit
        int D_STARTOL;              //15bit
        int T_OL;                   //10bit
        int D_OL;                   //15bit
        int V_RELEASEOL;            //7bit

        public override void Resolve(BitArray bitArray)
        {
            int pos = 0;
            NID_PACKET = Bits.ToInt(bitArray, ref pos, 8);
            Q_DIR = Bits.ToInt(bitArray, ref pos, 2);
            L_PACKET = Bits.ToInt(bitArray, ref pos, 13);
            Q_SCALE = Bits.ToInt(bitArray, ref pos, 2);
            V_LOA = Bits.ToInt(bitArray, ref pos, 7);
            T_LOA = Bits.ToInt(bitArray, ref pos, 10);
            //普通区段相关信息
            N_ITER = Bits.ToInt(bitArray, ref pos, 5);
            L_SECTION = new int[N_ITER];
            Q_SECTIONTIMER = new bool[N_ITER];
            T_SECTIONTIMER = new int[N_ITER];
            D_SECTIONTIMERSTOPLOC = new int[N_ITER];
            for(int i = 0; i < N_ITER; i++)
            {
                L_SECTION[i] = Bits.ToInt(bitArray, ref pos, 15);
                Q_SECTIONTIMER[i] = bitArray[pos++];
                if (Q_SECTIONTIMER[i] == true)
                {
                    T_SECTIONTIMER[i]= Bits.ToInt(bitArray, ref pos, 10);
                    D_SECTIONTIMERSTOPLOC[i]= Bits.ToInt(bitArray, ref pos, 15);
                }
            }
            //末区段相关信息
            L_ENDSECTION = Bits.ToInt(bitArray, ref pos, 15);
            //末区段有效时间
            Q_ENDSECTIONTIMER = bitArray[pos++];
            if (Q_ENDSECTIONTIMER)
            {
                T_ENDSECTIONTIMER = Bits.ToInt(bitArray, ref pos, 10);
                D_ENDSECTIONTIMERSTOPLOC = Bits.ToInt(bitArray, ref pos, 15);
            }
            //末区段保持时间
            Q_ENDTIMER = bitArray[pos++];
            if (Q_ENDTIMER)
            {
                T_ENDTIMER = Bits.ToInt(bitArray, ref pos, 10);
                D_ENDTIMERSTARTLOC = Bits.ToInt(bitArray, ref pos, 15);
            }
            //危险点信息
            Q_DANGERPOINT = bitArray[pos++];
            if (Q_DANGERPOINT)
            {
                D_DP = Bits.ToInt(bitArray, ref pos, 15);
                V_RELEASEDP = Bits.ToInt(bitArray, ref pos, 7);
            }
            //保护区段信息
            Q_OVERLAP = bitArray[pos++];
            if (Q_OVERLAP)
            {
                D_STARTOL = Bits.ToInt(bitArray, ref pos, 15);
                T_OL = Bits.ToInt(bitArray, ref pos, 10);
                D_OL = Bits.ToInt(bitArray, ref pos, 15);
                V_RELEASEOL = Bits.ToInt(bitArray, ref pos, 7);
            }
        }

        public void SetValueTo(MA ma)
        {
            ma.VLOA = V_LOA * 5 / 3.6;  //  化为m/s
            if (T_LOA == 1023) ma.TLOA = int.MaxValue; //表示时间不受限制，使用int最大值能保证不会超时
            else ma.TLOA = T_LOA;

            double scale = GetScale(Q_SCALE);   //使距离/长度统一使用米为单位
            for(int i = 0; i < N_ITER; i++)
            {
                ma.SectionLengthList.Add(L_SECTION[i]*scale);
                ma.SectionTimerExistsList.Add(Q_SECTIONTIMER[i]);
                if (Q_SECTIONTIMER[i])
                {
                    if (T_SECTIONTIMER[i] == 1023) ma.SectionTimerList.Add(int.MaxValue);
                    else ma.SectionTimerList.Add(T_SECTIONTIMER[i]);
                    ma.SectionTimerStopLocList.Add(D_SECTIONTIMERSTOPLOC[i] * scale);
                }
                else
                {   //这只是为了在List中占个位置，实际其值不能被使用
                    ma.SectionTimerList.Add(-1);
                    ma.SectionTimerStopLocList.Add(-1);
                }
            }

            ma.EndSectionLength = L_ENDSECTION * scale;
            ma.EndSectionTimerExists = Q_ENDSECTIONTIMER;
            if (Q_ENDSECTIONTIMER)
            {
                if (T_ENDSECTIONTIMER == 1023) ma.EndSectionTimer = int.MaxValue;
                else ma.EndSectionTimer = T_ENDSECTIONTIMER;
                ma.EndSectionTimerStopLoc = D_ENDSECTIONTIMERSTOPLOC * scale;
            }
            else
            {
                //此时不填无效值也可以，实际在类MA中不会被用到
            }
            ma.EndTimerExists = Q_ENDTIMER;
            if (Q_ENDTIMER)
            {
                if (T_ENDTIMER == 1023) ma.EndTimer = int.MaxValue;
                else ma.EndTimer = T_ENDTIMER;
                ma.EndTimerStartLoc = D_ENDTIMERSTARTLOC * scale;
            }
            else { }

            ma.DpExists = Q_DANGERPOINT;
            if (Q_DANGERPOINT)
            {
                ma.DpLength = D_DP * scale;
                ma.VReleaseDP = V_RELEASEDP * 5 / 3.6;
            }

            ma.OverlapExists = Q_OVERLAP;
            if (Q_OVERLAP)
            {
                ma.OlStart = D_STARTOL * scale;
                if (T_OL == 1023) ma.OlTimer = int.MaxValue;
                else ma.OlTimer = T_OL;
                ma.OlLength = D_OL * scale;
                ma.VReleaseOL = V_RELEASEOL * 5 / 3.6;
            }
        }




        public int GetPacketLength() { return L_PACKET; }
    }
}
