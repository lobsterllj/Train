using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 车到地——车载设备电话号码
    /// </summary>
    public class Packet003Train : AbstractPacket
    {
        int N_ITER;             //5bit
        List<ulong> NID_RADIO;      //64bit

        private const int BASE_LEN = 8 + 13 + 5;

        public Packet003Train()
        {
            NID_PACKET = 3;
        }

        public override BitArray Resolve()
        {
            L_PACKET = BASE_LEN;
            Fill();
            L_PACKET += N_ITER * 64;
            BitArray bitArray = new BitArray(L_PACKET);
            int[] intArray = new int[] { 8, 13 };
            int[] DataArray = new int[] { NID_PACKET, L_PACKET };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }
            Bits.ConvergeBitArray(bitArray, N_ITER, ref pos, 5);
            for (int i = 0; i < N_ITER; i++)
            {
                Bits.ConvergeBitArray(bitArray, NID_RADIO[i], ref pos, 64);
            }
            return bitArray;
        }
        private void Fill()
        {
            N_ITER = TrainInfo.NID_RADIOList.Count;
            NID_RADIO = TrainInfo.NID_RADIOList;
        }
    }
}
