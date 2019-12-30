using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Trains
{
    public enum _TrackletType
    {
        ORDINARY = 0, //普通区段
        DC = 1,              //道岔
        END = 2            //尽头
    }

    public class Tracklet
    {
        private static bool leftWard = true;//列车行驶方向向左则为true
        private int id;
        private double location1;//区段左端相对于左边起点的绝对位置
        private double location2;//区段右端相对于左边起点的绝对位置
        private double trackletLength;
        private _TrackletType leftType, rightType;//该tracklet的左右相邻tracklet的类型
        private int leftIndex, rightIndex;//该tracklet的左右相邻tracklet
        private int tags;//该区段应答器数量
        private List<int> tagIndex = new List<int>();

        private static Dictionary<int, Tracklet> trackletDict = new Dictionary<int, Tracklet>();

        public static void Init()
        {
            AddTestData();
        }
        public static void AddTestData()
        {
            trackletDict.Add(5, new Tracklet(5, 4.43, 200.43, 196.00, _TrackletType.END, 0, -1, 6));
            trackletDict.Add(6, new Tracklet(6, 200.43, 755.64, 555.21, 0, 0, 5, 16));
            trackletDict.Add(16, new Tracklet(16, 755.64, 921.24, 165.60, 0, 0, 6, 17));
            trackletDict.Add(17, new Tracklet(17, 921.24, 1178.40, 257.16, 0, 0, 16, 18));
            trackletDict.Add(18, new Tracklet(18, 1178.40, 1186.57, 8.17, 0, _TrackletType.DC, 17, 1));
            trackletDict.Add(19, new Tracklet(19, 1186.57, 1248.39, 61.82, _TrackletType.DC, 0, 1, 21));
            trackletDict.Add(21, new Tracklet(21, 1248.39, 1313.67, 65.28, 0, _TrackletType.DC, 19, 4));
            trackletDict.Add(22, new Tracklet(22, 1313.67, 1358.28, 44.61, _TrackletType.DC, 0, 4, 24));
            trackletDict.Add(24, new Tracklet(24, 1358.28, 1629.75, 271.47, 0, _TrackletType.END, 22, -1));
        }
        public Tracklet(int id, double loc1, double loc2, double len,
            _TrackletType ltype, _TrackletType rtype, int lindex, int rindex)
        {
            this.id = id;
            location1 = loc1; location2 = loc2;
            trackletLength = len;
            leftType = ltype; rightType = rtype;
            leftIndex = lindex; rightIndex = rindex;
        }

        public Tracklet GetNext()
        {
            if (LeftWard)
            {
                if (leftType == _TrackletType.ORDINARY)
                    return trackletDict[leftIndex];
                else if (leftType == _TrackletType.DC)
                {
                    Switchlet s = Switchlet.GetSwitchlet(leftIndex);
                    if (s == null) { return null; }//数据出错
                    return trackletDict[s.GetNextTrackletID(id)];
                }
                return null;            //到线路终点
            }
            //朝右行驶
            if (rightType == _TrackletType.ORDINARY)
                return trackletDict[rightIndex];
            else if (rightType == _TrackletType.DC)
            {
                Switchlet s = Switchlet.GetSwitchlet(rightIndex);
                if (s == null) { return null; }//数据出错
                return trackletDict[s.GetNextTrackletID(id)];
            }
            return null;            //到线路终点
        }

        public static Tracklet GetTracklet(int id)
        {
            if (trackletDict.ContainsKey(id))
                return trackletDict[id];
            return null;
        }

        public static int GetCount()
        {
            return trackletDict.Count;
        }


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

        public double TrackletLength
        {
            get
            {
                return trackletLength;
            }

            set
            {
                trackletLength = value;
            }
        }

        public int Tags
        {
            get
            {
                return tags;
            }

            set
            {
                tags = value;
            }
        }

        public List<int> TagIndex
        {
            get
            {
                return tagIndex;
            }

            set
            {
                tagIndex = value;
            }
        }

        public double Location1
        {
            get
            {
                return location1;
            }

            set
            {
                location1 = value;
            }
        }

        public double Location2
        {
            get
            {
                return location2;
            }

            set
            {
                location2 = value;
            }
        }


        public _TrackletType LeftType
        {
            get
            {
                return leftType;
            }

            set
            {
                leftType = value;
            }
        }

        public _TrackletType RightType
        {
            get
            {
                return rightType;
            }

            set
            {
                rightType = value;
            }
        }

        public int LeftIndex
        {
            get
            {
                return leftIndex;
            }

            set
            {
                leftIndex = value;
            }
        }

        public int RightIndex
        {
            get
            {
                return rightIndex;
            }

            set
            {
                rightIndex = value;
            }
        }

        public static bool LeftWard
        {
            get
            {
                return leftWard;
            }

            set
            {
                leftWard = value;
            }
        }
    }
}
