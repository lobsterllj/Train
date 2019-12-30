using System;
using System.Collections.Generic;
using System.Linq;
using Train.Packets;

namespace Train
{
    /// <summary>
    /// 用来进行列车MA运算的类，每收到一个MA消息（M03，M33等）都重新生成一个MA实例
    /// </summary>
    public class MA
    {
        double vLOA, tLOA;
        List<double> sectionLengthList = new List<double>();
        List<bool> sectionTimerExistsList = new List<bool>();
        List<double> sectionTimerList = new List<double>();
        List<double> sectionTimerStopLocList = new List<double>();
        double endSectionLength;
        bool endSectionTimerExists;    //是否存在末区段有效时间
        double endSectionTimer, endSectionTimerStopLoc;
        bool endTimerExists;               //是否存在末区段保持时间
        double endTimer, endTimerStartLoc;
        bool dpExists;
        double dpLength, vReleaseDP;
        bool overlapExists;//是否存在保护区段
        double olStart;//保护区段保持时间开始计时地点至MA终点距离
        double olTimer;//保护区段有效时间
        double olLength;//MA终点至保护区段末端的距离（即OL长度）
        double vReleaseOL;//保护区段开口速度

        private DateTime maStartTime = System.DateTime.Now;//从接收到MA消息开始算
        private double  maStartLoc=0;   //接收到MA消息时，列车估计前端至LRBG距离（即至MA起点距离）
        private double lDoubtOver=1, lDoubtUnder=1;//负误差、正误差，分别用于计算列车最小、最大安全前端
        private double totalDistance = 0;//列车走行总距离
        private bool isAtEndSection = false;
        private double endSectionStartLoc = 0;//末区段与MA起点距离
        private DateTime endSectionKeepTimeTimer = DateTime.MinValue;//末区段保持时间定时器
        private DateTime olSectionKeepTimeTimer = DateTime.MinValue;   //保护区段保持时间定时器
        private double targetSpeed = 0;  //EOA/LOA处目标速度
        private bool EB = false; //如果时间超时，则列车EB
        //
        public void SetStartTime()
        {
            maStartTime = System.DateTime.Now;
        }
        public void SetStartLoc(double loc)
        {
            maStartLoc = loc;
        }
        public void GetValueFrom(Packet015 p15)
        {
            p15.SetValueTo(this);
        }

        public bool GetEB() { return EB; } //如果检测到EB为true，则列车EB，MA直接无效
        public void Run(double distance)
        {
            totalDistance += distance;
            CheckLOASpeed();
            if (isAtEndSection == false)
            {
                CheckNormalSection();
                return;
            }
            CheckEndSection();
            CheckDangerPoint();
            CheckOverlapSection();
        }
        private void CheckLOASpeed()
        {
            if (DateTime.Now.Subtract(maStartTime).TotalSeconds > tLOA)
                targetSpeed = 0;
        }
        //检查普通区段（在末区段之前的区段）
        private void CheckNormalSection()
        {
            double leastSafeFront = maStartLoc - lDoubtOver + totalDistance;
            double totalSecLen = 0;
            bool flag = true;
            for(int i = 0; i < sectionLengthList.Count && flag; i++)
            {   //判断列车最小前端是否在此区段
                if(leastSafeFront >= sectionLengthList[i]+totalSecLen)
                {
                    totalSecLen += sectionLengthList[i];
                    continue;
                }
                flag = false;
                if (sectionTimerExistsList[i])
                {   //列车的最小安全前端通过了区段有效时间计时停止位置
                    if (leastSafeFront > sectionTimerStopLocList[i]+totalSecLen)
                    {
                        sectionTimerExistsList[i] = false;//设为false，停止此计时器，下次不用检查
                    }
                    else if(DateTime.Now.Subtract(maStartTime).TotalSeconds>sectionTimerList[i])
                    {
                        if (sectionTimerList[i] == int.MaxValue) continue; //时间被设为无超时，这种情况可以不用考虑
                        //超时
                        TimeExceedHandling();
                    }
                }
            }
            isAtEndSection = flag;
            if (!flag) return;//列车最小安全前端不在末区段
            endSectionStartLoc = totalSecLen;
        }

        private void CheckEndSection()
        {
            double leastSafeFront = maStartLoc - lDoubtOver + totalDistance;
            if (leastSafeFront > endSectionStartLoc + endSectionLength)
            {   //已驶出末区段
                return;
            }
            //判断末区段有效时间
            if (endSectionTimerExists)
            {
                if (leastSafeFront > endSectionStartLoc + endSectionTimerStopLoc)
                    endSectionTimerExists = false;
                else if (DateTime.Now.Subtract(maStartTime).TotalSeconds > endSectionTimer)
                {   //超时
                    TimeExceedHandling();
                }
            }
            double mostSafeFront = maStartLoc + lDoubtUnder + totalDistance;
            //判断末区段保持时间
            if(endTimerExists)
            {
                if (mostSafeFront > endSectionStartLoc + endSectionLength - endTimerStartLoc)
                {
                    if (endSectionKeepTimeTimer == DateTime.MinValue)   //设置定时器初值
                        endSectionKeepTimeTimer = DateTime.Now;
                    //程序执行到此处，说明列车最小安全前端仍在末区段
                    if (DateTime.Now.Subtract(endSectionKeepTimeTimer).TotalSeconds > endTimer)
                    {       //超时
                        TimeExceedHandling();
                    }
                }
            }
           
        }
        private void CheckDangerPoint()
        {
            if (!dpExists) return;

        }
        private void CheckOverlapSection()
        {
            if (!overlapExists) return;
            double mostSafeFront = maStartLoc + lDoubtUnder + totalDistance;
            if (mostSafeFront > endSectionStartLoc + endSectionLength + olLength)
            {   //已驶出保护区段
                return;
            }
            //判断保护区段保持时间
            if (mostSafeFront > endSectionStartLoc + endSectionLength - olStart)
            {
                if (olSectionKeepTimeTimer == DateTime.MinValue)
                    olSectionKeepTimeTimer = DateTime.Now;
                if (olTimer != int.MaxValue)
                {
                    if (DateTime.Now.Subtract(olSectionKeepTimeTimer).TotalSeconds > olTimer)
                    {   //超时
                        TimeExceedHandling();
                    }
                }
            }
        }
        private void TimeExceedHandling()
        {
            targetSpeed = 0;
            EB = true;
        }

        #region getters/setters

        public double VLOA
        {
            get
            {
                return vLOA;
            }

            set
            {
                vLOA = value;
            }
        }

        public double TLOA
        {
            get
            {
                return tLOA;
            }

            set
            {
                tLOA = value;
            }
        }

        public List<double> SectionLengthList
        {
            get
            {
                return sectionLengthList;
            }

            set
            {
                sectionLengthList = value;
            }
        }

        public List<bool> SectionTimerExistsList
        {
            get
            {
                return sectionTimerExistsList;
            }

            set
            {
                sectionTimerExistsList = value;
            }
        }

        public List<double> SectionTimerList
        {
            get
            {
                return sectionTimerList;
            }

            set
            {
                sectionTimerList = value;
            }
        }

        public List<double> SectionTimerStopLocList
        {
            get
            {
                return sectionTimerStopLocList;
            }

            set
            {
                sectionTimerStopLocList = value;
            }
        }

        public double EndSectionLength
        {
            get
            {
                return endSectionLength;
            }

            set
            {
                endSectionLength = value;
            }
        }

        public bool EndSectionTimerExists
        {
            get
            {
                return endSectionTimerExists;
            }

            set
            {
                endSectionTimerExists = value;
            }
        }

        public double EndSectionTimer
        {
            get
            {
                return endSectionTimer;
            }

            set
            {
                endSectionTimer = value;
            }
        }

        public double EndSectionTimerStopLoc
        {
            get
            {
                return endSectionTimerStopLoc;
            }

            set
            {
                endSectionTimerStopLoc = value;
            }
        }

        public bool EndTimerExists
        {
            get
            {
                return endTimerExists;
            }

            set
            {
                endTimerExists = value;
            }
        }

        public double EndTimer
        {
            get
            {
                return endTimer;
            }

            set
            {
                endTimer = value;
            }
        }

        public double EndTimerStartLoc
        {
            get
            {
                return endTimerStartLoc;
            }

            set
            {
                endTimerStartLoc = value;
            }
        }

        public bool DpExists
        {
            get
            {
                return dpExists;
            }

            set
            {
                dpExists = value;
            }
        }

        public double DpLength
        {
            get
            {
                return dpLength;
            }

            set
            {
                dpLength = value;
            }
        }

        public double VReleaseDP
        {
            get
            {
                return vReleaseDP;
            }

            set
            {
                vReleaseDP = value;
            }
        }

        public bool OverlapExists
        {
            get
            {
                return overlapExists;
            }

            set
            {
                overlapExists = value;
            }
        }

        public double OlStart
        {
            get
            {
                return olStart;
            }

            set
            {
                olStart = value;
            }
        }

        public double OlTimer
        {
            get
            {
                return olTimer;
            }

            set
            {
                olTimer = value;
            }
        }

        public double OlLength
        {
            get
            {
                return olLength;
            }

            set
            {
                olLength = value;
            }
        }

        public double VReleaseOL
        {
            get
            {
                return vReleaseOL;
            }

            set
            {
                vReleaseOL = value;
            }
        }
        #endregion
    }

}
