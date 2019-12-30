using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Data;
using Train.Packets;
using Train.Messages;
using Train.StaticSpeedLimits;

namespace Train.Trains
{
    public partial class TrainDynamics
    {

        //假设应答器是按照距离从小到大排序后放在List中的
        private static List<Balise> downBgList = new List<Balise>();//下行线应答器列表
        private static List<Balise> upBgList = new List<Balise>();
        public static List<Balise> DownBgList { get { return downBgList; } }
        public static List<Balise> UpBgList { get { return upBgList; } }
        private static Balise lrbg;
        private static Balise DEFAULT_BG = new Balise();
        private static Packet000 p0 = new Packet000();

        private volatile MA ma;     //用来进行MA运算
        //在MA_MH中设置
        public MA Ma
        {
            set
            {
                ma = value;
                double pos = lrbg == null ? 0 : lrbg.Position;
                ma.SetStartLoc(currentS1 - pos);      //假设列车沿下行正向行驶（即向右行驶）
                ma.SetStartTime();
            }
        }

        private volatile Dictionary<int, AbstractRecvMessage> dictEB;    //用来存放接收到的紧急停车消息
        public Dictionary<int, AbstractRecvMessage> DictEB { set { dictEB = value; } }  //在EB_MH中设置

        //外部其他模块调用本模块的函数
        public void OperatingVehicle(bool isLeftHead, DriverConsolerState driverConsoler, TrainState trainState)
        {
            bool eBStatus = driverConsoler.EBStatus;
            CalEBStatus(ref eBStatus);
            eBStatus = eBStatus || TSR.TsrEB;
            bool FullServiceStatus = false, FastBrake = false;
            bool b1 = trainState.BManualSpeed, b2 = trainState.BManualAccSpeed;
            DriveDirection driveDirection = driverConsoler.DriveDirection;
            double steerValue = (driverConsoler.SteerValue / 10.0);
            double distance = CountTrain(eBStatus, FullServiceStatus, FastBrake, isLeftHead, b1, b2, trainState.Speed, trainState.AccSpeed, driveDirection, steerValue);
            // calculate TSR status
            double maxV = TSR.Run(currentS0, currentS1);
            TSR.TsrEB = maxV < Math.Abs(currentV);
            if(!eBStatus && TSR.TsrEB)
            {
                Utilities.TextInfo.Add("因限速而紧急制动");
            }

            #region 计算列车位置
            prevS0 = currentS0;
            prevS1 = currentS1;
            CalTrainLocation(distance);
            if (ma != null)
            {
                ma.Run(distance);       //进行MA运算
            }

            {
                trainState.Speed = currentV;
                trainState.AccSpeed = currentA;
                if (trainState.TrainLocation.IsSetLoc)
                {
                    currentS0 = trainState.TrainLocation.LeftLoc;
                    currentS1 = trainState.TrainLocation.RightLoc;
                    CalLrbg(isLeftHead,driverConsoler.Line);
                    trainState.TrainLocation.IsSetLoc = false;      //reset this variable
                }
                else
                {
                    trainState.TrainLocation.LeftLoc = currentS0;
                    trainState.TrainLocation.RightLoc = currentS1;
                    CalLrbg(isLeftHead,driverConsoler.Line);
                }
                trainState.TrainLocation.Lrbg = lrbg;
                trainState.BrakeStatus = eBStatus || (steerValue < 0);
                trainState.EBStatus = eBStatus;
            }
            #endregion
            FillPacket0(driverConsoler,isLeftHead);
        }

        //根据列车是否越过MA范围，或者接收到EB消息来判断是否要紧急制动
        private void CalEBStatus(ref bool EBStatus)
        {
            if (ma != null && ma.GetEB())
            {
                EBStatus = true;
                if (Math.Abs(currentV) < 1e-6)  //列车速度减到0
                    ma = null;      //使MA无效
            }
            if (dictEB != null && dictEB.Count > 0)
            {
                EBStatus = true;
                if (Math.Abs(currentV) < 1e-6)  //列车速度减到0
                    dictEB.Clear();      //清空已有的紧急消息
            }
        }

        /// <summary>
        /// 每次都重新计算LRBG
        /// </summary>
        /// <param name="isLeftHead">true表示列车左驾驶室(CabA)激活</param>
        /// <param name="line">表示是处于上行线还是下行线</param>
        private void CalLrbg(bool isLeftHead, _Line line)
        {
            List<Balise> bgList = line == _Line.UP ? upBgList : downBgList;
            Balise next;
            if (currentV == 0)
            {
                if (isLeftHead)
                    next = GetNextBgLeft(bgList);
                else
                    next = GetNextBgRight(bgList);
            }
            else if (currentV > 0)
            {
                next = GetNextBgRight(bgList);
            }
            else
            {
                next = GetNextBgLeft(bgList);
            }
            if (next != null) lrbg = next;
            else lrbg = DEFAULT_BG;
        }

        // 列车朝左开时
        private Balise GetNextBgLeft(List<Balise> bgList)
        {
            Balise next = null;
            // BgList中应答器组按地理位置升序排序
            foreach (Balise bg in bgList)
            {
                if (bg.N_pig > 1) continue;//只考虑应答器组内第一个应答器
                if (bg.Position > currentS0)
                {
                    next = bg; break;
                }
            }
            return next;
        }
        // 列车朝右开时
        private Balise GetNextBgRight(List<Balise> bgList)
        {
            Balise next = null;
            foreach (Balise bg in bgList)
            {
                if (bg.N_pig > 1) continue;
                if (bg.Position > currentS1) break;
                else next = bg;
            }
            return next;
        }

        //填充packet0各字段信息
        private void FillPacket0(DriverConsolerState driverConsoler , bool isLeftHead)
        {

            p0.Q_SCALE = 1;// meter
            p0.NID_LRBG = (lrbg.Nid_c << 14) | (lrbg.Nid_bg);
            //p0.D_LRBG = lrbg.Position == 0 ? 0 : (int)(currentS1 - lrbg.Position);
            //p0.Q_DIRLRBG = 1; //相对于LRBG方向的列车朝向
            //p0.Q_DLRBG = 1;//估计列车前端位于LRBG哪一侧
            p0.L_DOUBTOVER = 1;//低限
            p0.L_DOUBTUNDER = 1;//高限
            p0.Q_LENGTH = 2;//设为由司机确认的列车完整性
            p0.L_TRAININT = totalLength / 1000;
            p0.V_TRAIN = (int)Math.Abs(currentV * 3.6 / 5);
            //p0.Q_DIRTRAIN = 1;  //相对于LRBG方向的列车运行方向
            p0.M_MODE = (int)driverConsoler.WorkMode;  //车载设备工作模式
            p0.M_LEVEL = (int)driverConsoler.ControlLevel;
            if (p0.M_LEVEL == 1)
                p0.NID_STM = 3;  //按通号设的一个值
            if(driverConsoler.Line == _Line.UP) // 上行线，LRBG正方向为从右到左
            {
                if(currentV == 0)
                {
                    if(isLeftHead)  // CabA
                        SetValueLeftUp();
                    else  // CabB
                        SetValueRightUp();
                }
                else if(currentV > 0) // 列车向右开
                    SetValueRightUp();
                else
                    SetValueLeftUp();
            }
            else  // 下行线，LRBG正方向为从左到右
            {
                if (currentV == 0)
                {
                    if (isLeftHead) SetValueLeftDown();
                    else SetValueRightDown();
                }
                else if (currentV > 0)
                    SetValueRightDown();
                else
                    SetValueLeftDown();
            }
        }

        // left means train run towards left or has the tendency
        /**
         * Q_DLRBG不清楚怎么设置，所以总是设为正向。
         * 当列车向左开时，列车朝向和运动方向都是向左；
         * 当列车向右开时，都是向右。
         * 
         * **/
        private void SetValueLeftDown()
        {
            p0.Q_DIRLRBG = (int)_DIR.REVERSE;
            p0.Q_DIRTRAIN = (int)_DIR.REVERSE;
            p0.Q_DLRBG = (int)_DIR.REVERSE;
            p0.D_LRBG = lrbg.Position == 0 ? 0 : (int)(lrbg.Position - currentS0);
        }

        private void SetValueRightDown()
        {
            p0.Q_DIRLRBG = (int)_DIR.FORWARD;
            p0.Q_DIRTRAIN = (int)_DIR.FORWARD;
            p0.Q_DLRBG = (int)_DIR.FORWARD;
            p0.D_LRBG = lrbg.Position == 0 ? 0 : (int)(currentS1 - lrbg.Position);
        }

        private void SetValueLeftUp()
        {
            p0.Q_DIRLRBG = (int)_DIR.FORWARD;
            p0.Q_DIRTRAIN = (int) _DIR.FORWARD;
            p0.Q_DLRBG = (int)_DIR.FORWARD;
            p0.D_LRBG = lrbg.Position == 0 ? 0 : (int)(lrbg.Position - currentS0);
        }

        private void SetValueRightUp()
        {
            p0.Q_DIRLRBG = (int)_DIR.REVERSE;
            p0.Q_DIRTRAIN = (int)_DIR.REVERSE;
            p0.Q_DLRBG = (int)_DIR.REVERSE;
            p0.D_LRBG = lrbg.Position == 0 ? 0 : (int)(currentS1 - lrbg.Position);
        }

        public static Packet000 GetPacket0()
        {
            return p0.Snapshot();
        }

        public static Balise GetCurrentLRBG()
        {
            return lrbg;
        }

    }
}
