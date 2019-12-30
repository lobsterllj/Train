using System;
using System.Collections;
using System.Text;
using Train.Utilities;
using Train.Data;
using System.Collections.Generic;

namespace Train.Packets
{
    /// <summary>
    /// 车到地——确认后的列车数据
    /// </summary>
    public class Packet011:AbstractPacket
    {
        long NID_OPERATIONAL;   //32bit
        int NC_TRAIN;           //15bit
        int L_TRAIN;            //12bit
        int V_MAXTRAIN;         //7bit
        int M_LOADINGGAUGE;     //8bit
        int M_AXLELOAD;         //7bit
        int M_AIRTIGHT;         //2bit
        int N_ITER1;            //5bit
        int N_ITER2;            //5bit
        List<int> M_TRACTION;       //8bit
        List<int> NID_STM;          //8bit

        private const int BASE_LEN = 8 + 13 + 32 + 15 + 12 + 7 + 8 + 7 + 2 + 5 + 5;

        public Packet011()
        {
            NID_PACKET = 11;
        }

        public override BitArray Resolve()
        {
            L_PACKET = BASE_LEN;
            Fill(); //must invoke this method first!
            L_PACKET +=  (N_ITER1+N_ITER2)* 8;

            BitArray bitArray = new BitArray(L_PACKET);
            int[] intArray = new int[] { 8, 13, 32, 15, 12, 7, 8, 7, 2 };
            int[] DataArray = new int[] { NID_PACKET, L_PACKET, 0, NC_TRAIN,
                L_TRAIN, V_MAXTRAIN, M_LOADINGGAUGE, M_AXLELOAD, M_AIRTIGHT };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                if (i == 2)
                {
                    Bits.ConvergeBitArray(bitArray, NID_OPERATIONAL, ref pos, intArray[i]);
                }
                else
                {
                    Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
                }
            }
            
            Bits.ConvergeBitArray(bitArray, N_ITER1, ref pos, 5);
            for (int i = 0; i < N_ITER1; i++)
            {
                Bits.ConvergeBitArray(bitArray, M_TRACTION[i], ref pos, 8);
            }
            Bits.ConvergeBitArray(bitArray, N_ITER2, ref pos, 5);
            for (int i = 0; i < N_ITER2; i++)
            {
                Bits.ConvergeBitArray(bitArray, NID_STM[i], ref pos, 8);
            }

            return bitArray;
        }
        private void Fill()
        {

            NID_OPERATIONAL = TrainInfo.NID_OPERATIONAL;
            NC_TRAIN = TrainInfo.NC_TRAIN;
            L_TRAIN = TrainInfo.L_TRAIN;
            V_MAXTRAIN = TrainInfo.V_MAXTRAIN;
            M_LOADINGGAUGE = TrainInfo.M_LOADINGGAUGE;
            M_AXLELOAD = TrainInfo.M_AXLELOAD;
            M_AIRTIGHT = TrainInfo.M_AIRTIGHT;
            N_ITER1 = TrainInfo.M_TRACTION.Count;
            M_TRACTION = TrainInfo.M_TRACTION;
            N_ITER2 = TrainInfo.NID_STMList.Count;
            NID_STM = TrainInfo.NID_STMList;
        }
    }
}
