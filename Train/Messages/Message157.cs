using System;
using System.Collections;
using System.Text;
using Train.Packets;
using Train.Utilities;
using Train.Data;

namespace Train.Messages
{
    /// <summary>
    /// 车到地——SoM位置报告
    /// </summary>
    public class Message157: AbstractSendMessage
    {
        public const int MESSAGEID = 157;

        int Q_STATUS;               //2bit
        AbstractPacket ap01;        //信息包0/1
        AbstractPacket ap;          //可选择的信息包
                                                //当ATP出现应答器一致性错误或者无线一致性错误时，应在消息157中包含信息包4
       
        public override byte[] Resolve()
        {
            int BitArrayLEN = 8 + 10 + 32 + 24 + 2;
            BitArray bit01 = ap01.Resolve();
            BitArray bit = (ap == null ? null : ap.Resolve());
            BitArrayLEN += (bit01 == null ? 0 : bit01.Length) + (bit == null ? 0 : bit.Length);
            BitArray bitArray = new BitArray(BitArrayLEN);

            Q_STATUS = 1; //设置SoM位置报告状态为1
            NID_MESSAGE = MESSAGEID;
            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);
            int[] intArray = new int[] { 8, 10, 32, 24, 2 };
            int[] DataArray = new int[] { NID_MESSAGE, L_MESSAGE,(int)T_TRAIN, NID_ENGINE,Q_STATUS };
            int pos = 0;
            for (int i = 0; i < intArray.Length; i++)
            {
                Bits.ConvergeBitArray(bitArray, DataArray[i], ref pos, intArray[i]);
            }
            for (int i = 0; i < bit01.Length; i++)
            {
                bitArray[pos++] = bit01[i];
            }
            for (int i = 0;bit!=null && i < bit.Length; i++)
            {
                bitArray[pos++] = bit[i];
            }

            byte[] sendData = new byte[L_MESSAGE];
            Bits.ToByte(sendData, bitArray);

            return sendData;
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
        public void SetPacket0or1(AbstractPacket ap)
        {
            ap01 = ap;
        }
        public void SetAlternativePacket(AbstractPacket ap)
        {
            this.ap = ap;
        }
    }
}
