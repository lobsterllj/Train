using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Trains
{
    public enum SwitchletDirection
    {
        FW = 0, //反位
        DW = 1, //定位
    }
    public class Switchlet
    {
        private int id;
        private int trackletID1, trackletID2, trackletID3;
        private SwitchletDirection direction;
        private static Dictionary<int, Switchlet> switchletDict = new Dictionary<int, Switchlet>();

        public static  void Init()
        {
            AddTestData();
        }
        public static void AddTestData()
        {
            switchletDict.Add(0,new Switchlet(0, 2, 1, 3, SwitchletDirection.DW));
            switchletDict.Add(1, new Switchlet(1, 18, 19, 20, SwitchletDirection.FW));
            switchletDict.Add(2, new Switchlet(2, 10, 9, 11, SwitchletDirection.DW));
            switchletDict.Add(3, new Switchlet(3, 12, 13, 14, SwitchletDirection.FW));
            switchletDict.Add(4, new Switchlet(4, 22, 21, 23, SwitchletDirection.FW)); //这条和数据库不一致
        }
        public Switchlet(int _id,int _trackletID1,int _trackletID2,int _trackletID3,
            SwitchletDirection _direction)
        {
            id = _id;
            trackletID1 = _trackletID1;
            trackletID2 = _trackletID2;
            trackletID3 = _trackletID3;
            direction = _direction;
        }

        public static Switchlet GetSwitchlet(int id)
        {
            if(switchletDict.ContainsKey(id))
                return switchletDict[id];
            return null;
        }
        public int GetNextTrackletID(int prevTrackletID)
        {
            if (prevTrackletID == trackletID1)
            {
                if (direction == SwitchletDirection.FW)
                    return trackletID2;
                else
                    return trackletID3;
            }
            return trackletID1;
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

        public int TrackletID1
        {
            get
            {
                return trackletID1;
            }

            set
            {
                trackletID1 = value;
            }
        }

        public int TrackletID2
        {
            get
            {
                return trackletID2;
            }

            set
            {
                trackletID2 = value;
            }
        }

        public int TrackletID3
        {
            get
            {
                return trackletID3;
            }

            set
            {
                trackletID3 = value;
            }
        }
    }
}
