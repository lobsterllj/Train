using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Packets;
using Train.Utilities;

namespace Train.Messages
{
    /// <summary>
    /// 车到地——紧急停车确认
    /// </summary>
    public class Message147 : AbstractSendMessage
    {
        public const int MESSAGEID = 147;
        int NID_EM;                 //4bit
        int Q_EMERGENCYSTOP;        //2bit（可选）（通号有这个字段）
        AbstractPacket ap01;        //信息包0/1

        public override byte[] Resolve()
        {
            int BitArrayLEN = 74 + 4 + 2;
            BitArray bit01 = ap01.Resolve();
            BitArrayLEN += bit01.Length;
            BitArray bitArray = new BitArray(BitArrayLEN);

            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);
            NID_MESSAGE = MESSAGEID;
            int[] intArray = new int[] { 8, 10, 32, 24, 4, 2 };
            int[] DataArray = new int[] { NID_MESSAGE, L_MESSAGE, (int)T_TRAIN, NID_ENGINE, NID_EM, Q_EMERGENCYSTOP };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }

            for (int i = 0; i < bit01.Length; i++)
            {
                bitArray[pos++] = bit01[i];
            }
         
            byte[] sendData = new byte[L_MESSAGE];
            Bits.ToByte(sendData, bitArray);

            return sendData;
        }
        public void SetNID_EM(int nid_em)
        {
            NID_EM = nid_em;
        }
        public void SetQ_ES(int es)
        {
            Q_EMERGENCYSTOP = es;
        }
        public void SetAbstractPacket(AbstractPacket ap)
        {
            ap01 = ap;
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
