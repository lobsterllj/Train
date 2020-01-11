using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Train.Data;

namespace Train
{
    /// <summary>车载设备工作模式</summary>
    //参考7.4.1.75 M_MODE字段取值
    public enum _M_MODE
    {
        /// <summary>完全监控模式</summary>
        FS = 0, 
        /// <summary>引导模式</summary>
        CO = 1,
        /// <summary>目视行车模式</summary>
        OS = 2,         // 通号用SR表示
        /// <summary>调车模式</summary>
        SH = 3,
        /// <summary>休眠模式</summary>
        SL = 5,        
        /// <summary>待机模式</summary>
        SB = 6,
        /// <summary>隔离模式</summary>
        IS = 10,        // 通号没有这个模式
        /// <summary>冒进防护模式</summary>
        TR = 7,
        /// <summary>冒进后防护模式</summary>
        PT = 8,
        /// <summary>非C3模式？</summary>
        SN = 13,      // 通号用SN表示非C3等级下的模式，但规范中没有
    }
    // 按通号规范修改的值
    public enum _ControlLevel
    {
        CTCS_0 = 0,
      //  STM=1,
      //  CTCS_1=2,
        CTCS_2 = 1,
        CTCS_3 = 3,
        CTCS_4 = 4
    }

    /// <summary>关于方向的限定词</summary>
    public enum _DIR:byte
    {
        /// <summary>反向</summary>
        REVERSE = 0,
        /// <summary>正向</summary>
        FORWARD = 1,
        /// <summary>未知</summary>
        UNKNOWN = 2,
        /// <summary>未使用</summary>
        UNUSED = 3
    }

    /// <summary>SoM位置报告状态</summary>
    //参考7.4.1.139 Q_STATUS字段取值
    public enum _Q_STATUS
    {
        /// <summary>无效</summary>
        INVALID = 0,
        /// <summary>有效</summary>
        VALID = 1,
        /// <summary>未知</summary>
        UNKNOWN = 2,
        /// <summary>未使用</summary>
        UNUSED = 3
    }

    public enum DriveDirection : byte
    {
        RMF = 0x01,                //RMF（向前）
        ZERO = 0x02,             //0
        RMR = 0x00               //RMR（向后）
    }
    public enum _Line
    {
        UP = 0,     // 上行
        DOWN = 1    // 下行
    }

    public class DriverConsolerState
    {
        private static DriverConsolerState driverConsolerA = new DriverConsolerState(true );
        private static DriverConsolerState driverConsolerB = new DriverConsolerState(true );
        private static  DriverConsolerState NULL = new DriverConsolerState(false);
        private DriverConsolerState(bool active)
        {
            CabActive = active;
        }
        public static DriverConsolerState GetDriverConsolerA() { return driverConsolerA; }
        public static DriverConsolerState GetDriverConsolerB() { return driverConsolerB; }
        public static DriverConsolerState GetNULL() { return NULL; }
        /// <summary>
        /// 本意是想防止状态切换后，变量状态与界面显示状态不一致的情形
        /// 但这样做会导致使用三个实例分别保存三种状态没有意义。。。
        /// </summary>
        /// <param name="dcs">驾驶台上次状态</param>
        /// <returns></returns>
        public DriverConsolerState CopyOf(DriverConsolerState dcs)
        {
            if (dcs == null) return null;
            driveDirection = dcs.driveDirection;
            steerValue = dcs.steerValue;
            eBStatus = dcs.eBStatus;
            workMode = dcs.workMode;
            controlLevel = dcs.controlLevel;
            line = dcs.line;
            return this;
        }

        bool cabActive;
        DriveDirection driveDirection;
        double steerValue;
        bool eBStatus;      //从驾驶台实施的紧急制动（即点击EB按钮）
        _M_MODE workMode = _M_MODE.SB;
        _ControlLevel controlLevel = _ControlLevel.CTCS_2;
        _Line line;

        public bool CabActive
        {
            get
            {
                return cabActive;
            }

            set
            {
                cabActive = value;
            }
        }

        public DriveDirection DriveDirection
        {
            get
            {
                return driveDirection;
            }

            set
            {
                driveDirection = value;
            }
        }

        public double SteerValue
        {
            get
            {
                return steerValue;
            }

            set
            {
                steerValue = value;
            }
        }

        public bool EBStatus
        {
            get
            {
                return eBStatus;
            }

            set
            {
                eBStatus = value;
            }
        }

        public _M_MODE WorkMode
        {
            get
            {
                return workMode;
            }

            set
            {
                workMode = value;
            }
        }

        public _ControlLevel ControlLevel
        {
            get
            {
                return controlLevel;
            }

            set
            {
                controlLevel = value;
            }
        }

        public _Line Line
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }
    }
    public class TrainState
    {
        double speed, accSpeed;
        bool bManualSpeed;
        bool bManualAccSpeed;
        bool brakeStatus, eBStatus;
        bool trainIntegrity;
        TrainLocation trainLocation = new TrainLocation();

        public double Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }

        public double AccSpeed
        {
            get
            {
                return accSpeed;
            }

            set
            {
                accSpeed = value;
            }
        }

        public bool BManualSpeed
        {
            get
            {
                return bManualSpeed;
            }

            set
            {
                bManualSpeed = value;
            }
        }

        public bool BManualAccSpeed
        {
            get
            {
                return bManualAccSpeed;
            }

            set
            {
                bManualAccSpeed = value;
            }
        }

        public bool BrakeStatus
        {
            get
            {
                return brakeStatus;
            }

            set
            {
                brakeStatus = value;
            }
        }

        public bool EBStatus
        {
            get
            {
                return eBStatus;
            }

            set
            {
                eBStatus = value;
            }
        }

        public TrainLocation TrainLocation
        {
            get
            {
                return trainLocation;
            }

            set
            {
                trainLocation = value;
            }
        }

        public bool TrainIntegrity
        {
            get
            {
                return trainIntegrity;
            }

            set
            {
                trainIntegrity = value;
            }
        }
    }
    public class TrainLocation
    {
        private double leftLoc;  //列车左端位置，以米为单位
        private double rightLoc;//列车右端位置
        private Balise lrbg; //最近相关应答器组
        private bool isSetLoc;  //如在SetLocForm中设置了列车位置，则此标识为true
        public static String LocToString(double loc)
        {
            return String.Format("K{0}+{1:f2}", (int)(loc / 1000), loc % 1000);
        }

        public double LeftLoc
        {
            get
            {
                return leftLoc;
            }

            set
            {
                leftLoc = value;
            }
        }

        public double RightLoc
        {
            get
            {
                return rightLoc;
            }

            set
            {
                rightLoc = value;
            }
        }

        public Balise Lrbg
        {
            get
            {
                return lrbg;
            }

            set
            {
                lrbg = value;
            }
        }

        public bool IsSetLoc
        {
            get
            {
                return isSetLoc;
            }

            set
            {
                isSetLoc = value;
            }
        }
    }
}
