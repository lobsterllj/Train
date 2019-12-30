using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——行车许可请求参数
    /// </summary>
    public class Packet057:AbstractPacket
    {
        int Q_DIR;              //2bit
        
        int T_MAR;              //8bit
        int T_TIMEOUTRQST;      //10bit
        int T_CYCRQST;          //8bit

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 8, 10, 8 };
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
            T_MAR = resultArray[3];
            T_TIMEOUTRQST = resultArray[4];
            T_CYCRQST = resultArray[5];
        }
        public int GetTcycRqst() { return T_CYCRQST; }

    }
}
