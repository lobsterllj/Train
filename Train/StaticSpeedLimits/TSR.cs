using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.StaticSpeedLimits
{
    /// <summary>
    /// 临时限速
    /// </summary>
    public class TSR : StaticSpeedLimit
    {
        private int id;
        private double length;
        private double distance;
        private double velocity;
        private bool front;  // false表示考虑末端，true表示只考虑列车前端
        private int qdir;  // 0反向有效，1正向有效，2双向有效

        private _Line line;
        private double totalDis=0;//接收到报文后，列车走行距离
        private double leftEndPoint;    // 限速区域的左端的绝对位置
        private double rightEndPoint;   // 限速区域的右端的绝对位置
        private static Dictionary<int, TSR> tsrDict = new Dictionary<int, TSR>();
        private static bool tsrEB = false;
        public static bool TsrEB { get { return tsrEB; } set { tsrEB = value; } }

        public TSR(Packets.Packet065 p65)
        {
            p65.SetValueTo(this);
            Data.Balise lrbg = Trains.TrainDynamics.GetCurrentLRBG();
            if (lrbg.Nid_lrbg % 2 == 1) line = _Line.DOWN;
            else line = _Line.UP;
            if(IsRunLeft()) CalEndPointRunLeft(lrbg.Position);
            else CalEndPointRunRight(lrbg.Position);
        }

        private bool IsRunLeft()
        {
            return line == _Line.DOWN && qdir == 0 ||
                line == _Line.UP && qdir == 1;
        }

        // 列车朝左开时
        private void CalEndPointRunLeft(double position)
        {
            rightEndPoint = position - distance;
            leftEndPoint = position - distance - length;
        }
        // 列车朝右开时
        private void CalEndPointRunRight(double position)
        {
            leftEndPoint = position + distance;
            rightEndPoint = position + distance + length;
        }

        public static double Run(double curS0,double curS1)
        {
            double maxV = 600 / 3.6;
            
            for(int i=0;i<tsrDict.Values.Count;i++)
            {
                TSR tsr = tsrDict.Values.ElementAt(i);
                if (tsr.IsInTheRegion(curS0, curS1))
                    maxV = Math.Min(maxV, tsr.velocity);
            }
            return maxV;
        }

        private bool IsInTheRegion(double curS0, double curS1)
        {
            if (IsRunLeft())
            {
                if (curS0 > rightEndPoint) return false; // 还未到
                if (curS0 >= leftEndPoint) return true;  // 车头在限速区
                // 车头已越过限速区
                if (front)  //不考虑车尾
                {
                    Utilities.TextInfo.Add("无车尾保持，越过限速区域，删除命令id:" + id);
                    tsrDict.Remove(id);    //已经越过限速区域
                }
                else  // 考虑车尾
                {
                    if (curS1 >= leftEndPoint) return true;
                    else
                    {
                        Utilities.TextInfo.Add("车尾保持，越过限速区域，删除命令id:" + id);
                        tsrDict.Remove(id);
                    }
                }
            }
            else  // 朝右开
            {
                if (curS1 < leftEndPoint) return false;
                if (curS1 <= rightEndPoint) return true;
                // 车头已越过限速区
                if (front)
                {
                    Utilities.TextInfo.Add("无车尾保持，越过限速区域，删除命令id:" + id);
                    tsrDict.Remove(id);
                }
                else
                {
                    if (curS0 <= rightEndPoint) return true;
                    else
                    {
                        Utilities.TextInfo.Add("车尾保持，越过限速区域，删除命令id:" + id);
                        tsrDict.Remove(id);
                    }
                }
            }
            return false;
        }

        public static void Add(TSR tsr)
        {
            Remove(tsr.id);  // TSRS may send TSRs with same id,remove the old add the new.
            tsrDict.Add(tsr.id, tsr);
        }
        public static void Remove(int id)
        {
            if(tsrDict.ContainsKey(id))
                tsrDict.Remove(id);
        }
        /// <summary>
        /// 列车运行方向改变时，之前的所有TSR都应删除
        /// </summary>
        public static void Clear()
        {
            tsrDict.Clear();
        }

        #region setters/getters
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public double Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }

        public double Distance
        {
            get
            {
                return distance;
            }

            set
            {
                distance = value;
            }
        }

        public double Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }

        public bool Front
        {
            get
            {
                return front;
            }

            set
            {
                front = value;
            }
        }

        public int Qdir
        {
            get
            {
                return qdir;
            }

            set
            {
                qdir = value;
            }
        }
        #endregion
    }
}
