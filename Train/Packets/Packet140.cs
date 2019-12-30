using System;
using System.Collections;
using System.Text;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——来自RBC的列车车次号
    /// </summary>
    public class Packet140:AbstractPacket
    {
        int Q_DIR;              //2bit
        int NID_OPERARIONAL;   //32bit

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 32 };
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
            NID_OPERARIONAL = resultArray[3];
            TrainInfo.NID_OPERATIONAL = NID_OPERARIONAL;
        }
    }
}
