using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 车到地——错误报告
    /// </summary>
    public class Packet004:AbstractPacket
    {
        int M_ERROR;            //8bit

        public Packet004()
        {
            NID_PACKET = 4;
            L_PACKET = 29;
        }

        public override BitArray Resolve()
        {
            BitArray bitArray = new BitArray(29);
            int[] intArray = new int[] { 8, 13, 8 };
            int[] DataArray = new int[] { NID_PACKET, L_PACKET, M_ERROR };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }
            return bitArray;
        }
    }
}
