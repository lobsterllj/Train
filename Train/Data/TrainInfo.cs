using System;
using System.Collections.Generic;
using System.Text;
using Train.Packets;

namespace Train
{
    //具体含义见数据库表
    public class TrainInfo
    {
        private static int trainID;
        private static int l_TRAIN = 140;
        private static int nID_ENGINE;
        private static int nID_XUSER;
        private static int nID_OPERATIONAL;    //在地对车信息包140中，RBC会给列车分配车次号
        private static int nC_TRAIN=0; //default
        private static int v_MAXTRAIN;
        private static int m_LOADINGGAUGE;
        private static int m_AXLELOAD;
        private static int m_AIRTIGHT;
        private static List<int> nID_STMList = new List<int>();
        private static List<ulong> nID_RADIOList = new List<ulong>();
        private static List<int> m_TRACTION = new List<int>();
        //RBC会给车发送信息包3，包含了一些配置参数，
        //但目前可能用不到，所以就先保存在p3中
        public static Packet003 p3;

        #region getters/setters

        public static int TrainID
        {
            get
            {
                return trainID;
            }

            set
            {
                trainID = value;
            }
        }

        public static int L_TRAIN
        {
            get
            {
                return l_TRAIN;
            }

            set
            {
                l_TRAIN = value;
            }
        }

        public static int NID_ENGINE
        {
            get
            {
                return nID_ENGINE;
            }

            set
            {
                nID_ENGINE = value;
            }
        }

        public static int NID_XUSER
        {
            get
            {
                return nID_XUSER;
            }

            set
            {
                nID_XUSER = value;
            }
        }

        public static int NID_OPERATIONAL
        {
            get
            {
                return nID_OPERATIONAL;
            }

            set
            {
                nID_OPERATIONAL = value;
            }
        }

        public static int NC_TRAIN
        {
            get
            {
                return nC_TRAIN;
            }

            set
            {
                nC_TRAIN = value;
            }
        }

        public static int V_MAXTRAIN
        {
            get
            {
                return v_MAXTRAIN;
            }

            set
            {
                v_MAXTRAIN = value;
            }
        }

        public static int M_LOADINGGAUGE
        {
            get
            {
                return m_LOADINGGAUGE;
            }

            set
            {
                m_LOADINGGAUGE = value;
            }
        }

        public static int M_AXLELOAD
        {
            get
            {
                return m_AXLELOAD;
            }

            set
            {
                m_AXLELOAD = value;
            }
        }

        public static int M_AIRTIGHT
        {
            get
            {
                return m_AIRTIGHT;
            }

            set
            {
                m_AIRTIGHT = value;
            }
        }

        public static List<int> NID_STMList
        {
            get
            {
                return nID_STMList;
            }

            set
            {
                nID_STMList = value;
            }
        }

        public static List<ulong> NID_RADIOList
        {
            get
            {
                return nID_RADIOList;
            }

            set
            {
                nID_RADIOList = value;
            }
        }

        public static List<int> M_TRACTION
        {
            get
            {
                return m_TRACTION;
            }

            set
            {
                m_TRACTION = value;
            }
        }

        #endregion
    }
}
