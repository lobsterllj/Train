﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Packets;
using Train.Utilities;
using Train.Data;

namespace Train.Messages
{
    /// <summary>
    /// 车到地——任务结束
    /// </summary>
    public class Message150: AbstractSendMessage
    {

        public const int MESSAGEID = 150;
        AbstractPacket ap01;        //可选择的信息包0/1

        public override byte[] Resolve()
        {
            int BitArrayLEN = 74;
            BitArray bit01 = ap01.Resolve();
            BitArrayLEN += bit01.Length;
            NID_MESSAGE = MESSAGEID;
            L_MESSAGE = BitArrayLEN / 8 + (BitArrayLEN % 8 == 0 ? 0 : 1);

            BitArray bitArray = new BitArray(BitArrayLEN);
            int[] intArray = new int[] { 8, 10, 32, 24 };
            int[] DataArray = new int[] { NID_MESSAGE, L_MESSAGE, (int)T_TRAIN, NID_ENGINE };
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

        public void SetPacket0or1(AbstractPacket ap)
        {
            ap01 = ap;
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
    }
}
