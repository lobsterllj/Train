using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Utilities
{
    /// <summary>
    /// 工具库，针对于bit的操作
    /// </summary>
    public class Bits
    {
        public static BitArray ToBitArray(byte[] data)
        {
            int len = data.Length;
            BitArray bitArray = new BitArray(len * 8);
            for(int i = 0; i < len; i++)
            {
                for (int j = 0; j < 8; j++)
                    bitArray[i * 8 + j] = ((data[i] >> (7 - j)) & 1) == 1;
            }
            return bitArray;
        }
        /// <summary>
        /// 将bitArray从start位置开始，复制length长度
        /// </summary>
        /// <returns>复制的BitArray</returns>
        public static BitArray SubBitArray(BitArray bitArray,int start,int length)
        {
            if (start + length > bitArray.Length)
                throw new ArgumentException("start+length>bitArray.Length");
            BitArray bit = new BitArray(length);
            for (int i = 0; i < length; i++)
            {
                bit[i] = bitArray[i + start];
            }
            return bit;
        }
        public static BitArray Union(BitArray first,BitArray second)
        {
            int len1 = first.Length, len2 = second.Length;
            BitArray res = new BitArray(len1 + len2);
            for (int i = 0; i < len1;) res[i] = first[i++];
            for (int i = len1, j = 0; i < res.Length; i++)
                res[i++] = second[j++];
            return res;
        }
        /// <summary>
        /// 将bitArray中从position位置开始的length个bit转换为一个int。
        /// 按大端方式,高位补0
        /// </summary>
        public static int ToInt(BitArray bitArray, ref int position, int length)
        {
            int result = 0;
            for (int i = 0; i < length; i++)
            {
                if (position + i >= bitArray.Length) break;
                result <<= 1;
                result += bitArray[position + i] ? 1 : 0;
            }
            position += length;
            return result;
        }
        public static long ToLong(BitArray bitArray, ref int position, int length)
        {
            long result = 0;
            for (int i = 0; i < length; i++)
            {
                if (position + i >= bitArray.Length) break;
                result <<= 1;
                result += bitArray[position + i] ? 1 : 0;
            }
            position += length;
            return result;
        }
        /// <summary>
        /// 将data中低length位放到bitArray中
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="data"></param>
        /// <param name="position">bitArray中起点</param>
        /// <param name="length">data长度</param>
        public static void ConvergeBitArray(BitArray bitArray, int data, ref int position, int length)
        {
            int remain = 0;
            for (int i = length-1; i >=0; i--)
            {
                remain = data % 2;
                bitArray[position + i] = (remain == 1);
                data /= 2;
            }
            position += length;
        }
        public static void ConvergeBitArray(BitArray bitArray, long data, ref int position, int length)
        {
            long remain = 0;
            for (int i = length-1; i >=0; i--)
            {
                remain = data % 2;
                bitArray[position + i] = (remain == 1);
                data /= 2;
            }
            position += length;
        }
        public static void ConvergeBitArray(BitArray bitArray, ulong data, ref int position, int length)
        {
            ulong remain = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                remain = data % 2;
                bitArray[position + i] = (remain == 1);
                data /= 2;
            }
            position += length;
        }
        /// <summary>
        /// 将bitArray中数据填充到字节数组sendData中
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="bitArray"></param>
        public static void ToByte(byte[] sendData, BitArray bitArray)
        {
            int pos = 0;
            for (int i = 0; pos+8 <= bitArray.Length; i++)
            {
                sendData[i] = Convert.ToByte(Bits.ToInt(bitArray, ref pos, 8));
            }
            if (pos >= bitArray.Length)
                return;
            // calculate the left bits.
            int rest = 0;
            for(int i = 0; i < 8; i++)
            {
                rest <<= 1;
                if (pos < bitArray.Length)
                {
                    rest += bitArray[pos++] ? 1 : 0;
                }
            }
            sendData[sendData.Length - 1] = Convert.ToByte(rest);
        }
    }
}
