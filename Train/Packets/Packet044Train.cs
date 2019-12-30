using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 车到地——STM信息
    /// </summary>
    public class Packet044Train : AbstractPacket
    {
        int NID_XUSER;          //9bit
        //其他数据（取决于NID_XUSER）

        public override BitArray Resolve()
        {
            BitArray bitArray = new BitArray(100);
            int[] intArray = new int[] { 8, 13, 24 };
            int[] DataArray = new int[] { NID_PACKET, L_PACKET, NID_XUSER };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }
            return bitArray;
        }
    }
}
