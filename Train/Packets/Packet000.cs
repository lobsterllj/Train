using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 车到地——位置报告
    /// </summary>
    public class Packet000:AbstractPacket
    {
        public int Q_SCALE =2 ;            //2bit
        public int NID_LRBG = -1;           //24bit
        public int D_LRBG = -1;             //15bit
        public int Q_DIRLRBG = 2;          //2bit
        public int Q_DLRBG = 2;            //2bit
        public int L_DOUBTOVER = 0;        //15bit
        public int L_DOUBTUNDER = 0;       //15bit
        public int Q_LENGTH = 2;           //2bit
        public int L_TRAININT = -1;         //15bit
        public int V_TRAIN = -1;            //7bit
        public int Q_DIRTRAIN = 2;         //2bit
        public int M_MODE = -1;             //4bit
        public int M_LEVEL = -1;            //3bit
        public int NID_STM = 1;            //8bit

        private const int BASE_LEN = 8 + 13 + 2 + 24 + 15 + 2 + 2 + 15 + 15 + 2 + 7 + 2 + 4 + 3;

        public Packet000()
        {
            NID_PACKET = 0;
        }

        public override BitArray Resolve()
        {
            L_PACKET = BASE_LEN;
            if (Q_LENGTH == 1 || Q_LENGTH == 2) L_PACKET += 15;
            if (M_LEVEL == 1) L_PACKET += 8;
            BitArray bitArray = new BitArray(L_PACKET);
            int[] intArray = new int[] { 8, 13, 2, 24, 15, 2, 2, 15, 15, 2, 15, 7, 2, 4, 3, 8 };
            int[] DataArray = new int[] { NID_PACKET, L_PACKET, Q_SCALE, NID_LRBG, D_LRBG,
                Q_DIRLRBG, Q_DLRBG, L_DOUBTOVER, L_DOUBTUNDER, Q_LENGTH, L_TRAININT,
                V_TRAIN, Q_DIRTRAIN, M_MODE, M_LEVEL, NID_STM };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
                //Q_LENGTH
                if ((i == 9) && (DataArray[i] != 1) && (DataArray[i] != 2))
                {
                    i++;
                    continue;
                }
                //M_LEVEL
                if ((i == 14) && (DataArray[i] != 1))
                {
                    i++;
                    continue;
                }
            }
            return bitArray;
        }

        public Packet000 Snapshot()
        {
            Packet000 dst = new Packet000();
            dst.L_PACKET = L_PACKET;
            dst.Q_SCALE = Q_SCALE;
            dst.NID_LRBG = NID_LRBG;
            dst.D_LRBG = D_LRBG;
            dst.Q_DIRLRBG = Q_DIRLRBG;
            dst.Q_DLRBG = Q_DLRBG;
            dst.L_DOUBTOVER = L_DOUBTOVER;
            dst.L_DOUBTUNDER = L_DOUBTUNDER;
            dst.Q_LENGTH = Q_LENGTH;
            dst.L_TRAININT = L_TRAININT;
            dst.V_TRAIN = V_TRAIN;
            dst.Q_DIRTRAIN = Q_DIRTRAIN;
            dst.M_MODE = M_MODE;
            dst.M_LEVEL = M_LEVEL;
            dst.NID_STM = NID_STM;
            return dst;
        }
    }
}
