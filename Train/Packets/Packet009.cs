using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 车到地——CTCS-3级转换信息
    /// </summary>
    public class Packet009 : AbstractPacket
    {
        int NID_LTRBG;          //24bit

        public Packet009()
        {
            NID_PACKET = 9;
            L_PACKET = 45;
        }

        public override BitArray Resolve()
        {
            BitArray bitArray = new BitArray(L_PACKET);
            int[] intArray = new int[] { 8, 13, 24 };
            int[] DataArray = new int[] { NID_PACKET, L_PACKET, NID_LTRBG };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }
            return bitArray;
        }
        public void SetLTRBG(int nid_ltrbg)
        {
            NID_LTRBG = nid_ltrbg;
        }
    }
}