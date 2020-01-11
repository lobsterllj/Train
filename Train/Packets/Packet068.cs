using System;
using System.Collections;
using System.Text;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>线路条件类型</summary>
    // 7.4.1.80
    public enum _M_TRACKCOND
    {
        /// <summary>隧道</summary>
        TUNNEL = 0,
        /// <summary>桥梁</summary>
        BRIDGE = 1,
        /// <summary>无电区</summary>
        POWEROFF = 9
    }


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

        public string GetText()
        {
            string text = "";
            if (Q_TRACKINIT) return text;
            text += GetText((_M_TRACKCOND)M_TRACKCOND_BASE, D_TRACKCOND_BASE, L_TRACKCOND_BASE);
            for(int i = 0; i < N_ITER; i++)
            {
                text += "\r\n" + GetText((_M_TRACKCOND)M_TRACKCOND[i], D_TRACKCOND[i], L_TRACKCOND[i]);
            }
            return text;
        }

        private string GetText(_M_TRACKCOND mTrackCond,int dTrackCond, int lTrackCond)
        {
            string text = "" ;
            switch (mTrackCond)
            {
                case _M_TRACKCOND.TUNNEL:
                    text = "隧道"; break;
                case _M_TRACKCOND.BRIDGE:
                    text = "桥梁"; break;
                case _M_TRACKCOND.POWEROFF:
                    text = "无电区"; break;
                default:
                    text = "未知线路条件";break;
            }
            text += ",距离为"+ dTrackCond +"m,长度为" + lTrackCond +"m";
            return text;
        }

    }
}
