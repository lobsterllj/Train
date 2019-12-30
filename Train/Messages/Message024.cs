using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Train.Packets;
using Train.Utilities;

namespace Train.Messages
{
    /// <summary>
    /// 地到车——通常信息
    /// </summary>
    public class Message024:AbstractRecvMessage
    {

        public const int MESSAGEID = 24;
        //可选择的信息包，可能有多个
        List<AbstractPacket> apList = new List<AbstractPacket>();         

        public override void Resolve(byte[] recvData)
        {
            BitArray bitArray = Bits.ToBitArray(recvData);

            int[] intArray = new int[] { 8, 10, 32, 1, 24 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);
            }
            NID_MESSAGE = resultArray[0];
            L_MESSAGE = resultArray[1];
            T_TRAIN = (uint)(resultArray[2]);
            M_ACK = resultArray[3] == 1;
            NID_LRBG = resultArray[4];
            //获取还未被解析的数据
            bitArray = Bits.SubBitArray(bitArray, pos, bitArray.Length - pos);
            //由于填充数据一定小于8bit，所以当
            //bitArray长度大于等于8时就认为还有信息包需要解析
            while (bitArray.Length >= 8)  
            {
                pos = 0;
                int ID = Bits.ToInt(bitArray, ref pos, 8); //NID_PACKET
                pos += 2;   //地对车信息包Q_DIR信息
                int pktLen = Bits.ToInt(bitArray, ref pos, 13);//L_PACKET
                AbstractPacket ap = AbstractPacket.GetPacket(ID);
                ap.Resolve(bitArray);
                apList.Add(ap);
                pos = pktLen;//已解析的数据长度
                //获取还未被解析的数据
                bitArray = Bits.SubBitArray(bitArray, pos, bitArray.Length - pos);
            }
            
        }
        public override int GetMessageID()
        {
            return MESSAGEID;
        }
        public List<AbstractPacket> GetAlternativePacket()
        {
            return apList;
        }
    }
}
