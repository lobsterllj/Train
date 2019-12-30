using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Utilities;

namespace Train.Packets
{
    /// <summary>
    /// 地到车——配置参数
    /// </summary>
    public class Packet003:AbstractPacket
    {
        int Q_DIR;              //2bit传输数据的有效性方向
        int Q_SCALE;            //2bit距离/长度信息单位的限定词
        int D_VALIDNV;          //15bit至配置参数生效地点的距离
        int N_ITER;             //5bit数据包中跟随该变量的数组的重复次数
        int[] NID_C=new int[4];            //10bit地区号
        int V_NVSHUNT;          //7bit调车模式允许速度
        int V_NVSTFF;           //7bit目视行车模式的允许速度
        int V_NVONSIGHT;        //7bit引导模式的允许速度
        int V_NVUNFIT;          //7bit未装备模式的允许速度
        int V_NVREL;            //7bit开口速度的速度容限
        int D_NVROLL;           //15bit溜逸距离限制
        bool Q_NVSRBKTRG;       //1bit当监控制动到目标点时，允许实施常用制动
        bool Q_NVEMRRLS;        //1bit紧急制动缓解限定词
        int V_NVALLOWOVTRP;     //7bit允许司机选择“越行EOA”功能的最大速度界限
        int V_NVSUPOVTRP;       //7bit当“越行EOA”功能被激活时，监督允许速度界限
        int D_NVOVTRP;          //15bit禁止列车冒进防护的最大距离
        int T_NVOVTRP;          //8bit禁止列车冒进防护的最大时间
        int D_NVPOTRP;          //15bit在冒进后防护模式下退行的最大距离
        int M_NVCONTACT;        //2bitT_NVCONTACT反应
        int T_NVCONTACT;        //8bit无新的“安全”消息的最大时间
        bool M_NVDERUN;         //1bit运行时允许输入司机号
        int D_NVSTFF;           //15bit目视行车模式的最大运行距离
        bool Q_NVDRIVER_ADHES;  //1bit司机修改轨旁粘着因子的限定词

        public const int LOC = 5; //循环数字段位置
        public const int ITER = 1;//重复部分变量个数

        public override void Resolve(BitArray bitArray)
        {
            int[] intArray = new int[] { 8, 2, 13, 2, 15, 5, 10, 7, 7, 7, 7, 7, 15, 1, 1, 7, 7, 15, 8, 15, 2, 8, 1, 15, 1 };
            int Len = intArray.Length;
            int[] resultArray = new int[Len];
            int i = 0, pos = 0;
            for (i = 0; i < Len; i++)
            {
                resultArray[i] = Bits.ToInt(bitArray, ref pos, intArray[i]);

                if ((i == LOC) && (resultArray[LOC] == 0))
                {
                    i += ITER;
                    continue;
                }
        
                if (i == (LOC + ITER))
                {
                    NID_C = new int[resultArray[LOC]];

                    NID_C[0] = resultArray[LOC + 1];

                    for (int j = 1; j < resultArray[LOC]; j++)
                    {
                        NID_C[j] = Bits.ToInt(bitArray, ref pos, intArray[LOC + 1]);
                    }
                }
            }

            NID_PACKET = resultArray[0];
            Q_DIR = resultArray[1];
            L_PACKET = resultArray[2];
            Q_SCALE = resultArray[3];
            D_VALIDNV = resultArray[4];
            N_ITER = resultArray[5];
            V_NVSHUNT = resultArray[7];
            V_NVSTFF = resultArray[8];
            V_NVONSIGHT = resultArray[9];
            V_NVUNFIT = resultArray[10];
            V_NVREL = resultArray[11];
            D_NVROLL = resultArray[12];
            Q_NVSRBKTRG = resultArray[13] == 1;
            Q_NVEMRRLS = resultArray[14] == 1;
            V_NVALLOWOVTRP = resultArray[15];
            V_NVSUPOVTRP = resultArray[16];
            D_NVOVTRP = resultArray[17];
            T_NVOVTRP = resultArray[18];
            D_NVPOTRP = resultArray[19];
            M_NVCONTACT = resultArray[20];
            T_NVCONTACT = resultArray[21];
            M_NVDERUN = resultArray[22] == 1;
            D_NVSTFF = resultArray[23];
            Q_NVDRIVER_ADHES = resultArray[24] == 1;

        }

    }
}
