using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——粘着因子
    /// </summary>
    public class Packet071:AbstractPacket
    {
        int Q_DIR;              //2bit
        int Q_SCALE;            //2bit
        int D_ADHESION;         //15bit
        int L_ADHESION;         //15bit
        bool M_ADHESION;        //1bit

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 15, 1 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            D_ADHESION = resultArray[4];
            L_ADHESION = resultArray[5];
            if (resultArray[6] == 1)
            {
                M_ADHESION = true;
            }
            else
            {
                M_ADHESION = false;
            }
        }
    }
}
