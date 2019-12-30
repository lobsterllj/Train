using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Data
{
    /// <summary>
    /// 表示应答器信息的类
    /// </summary>
    public class Balise:IComparable<Balise>
    {
        //
        private string baliseName;      //应答器名称
        /*
         *编号格式如：040-1-04-018-1
         * 040：大区号（7bit），1：分区号（3bit），此两者组成NID_C
         * 04：车站编号（6bit），018：应答器单元编号（8bit），此两者组成NID_BG
         * 1：应答器组内编号，对于单应答器组，这一项没有
         */
        private string baliseNumber;    //应答器编号
        private int position;       //里程，应答器位置，相对于线路最左端，单位：米
        private bool isSourced;     //设备类型，有源/无源
        private string usage;       //用途
        private string remark1;     //备注1
        private string remark2;     //备注2，该应答器属于哪个站
        
        //这三项从baliseNumber中得到,全1表示位置未知
        private int nid_bg = 0x3fff;     //应答器组编号
        private int nid_c = 0x3ff;          //地区号，应统一赋值
        private int n_pig=0;     //应答器组内编号，0表示这是一个单应答器组

        private static List<Balise> bgList = new List<Balise>();

        private void ResolveInfoFromBaliseNumber()
        {
            string[] sa = baliseNumber.Split('-');
            if (sa == null || sa.Length < 4)
                throw new FormatException("应答器编号格式不对："+baliseNumber);
            nid_c = (Convert.ToInt32(sa[0]) << 3) | (Convert.ToInt32(sa[1]) & 7);
            nid_bg = (Convert.ToInt32(sa[2]) << 8) | (Convert.ToInt32(sa[3]) & 0xff);
            if (sa.Length >= 5)
                n_pig = Convert.ToInt32(sa[4]);
        }

        /// <summary>
        /// implements IComparable interface
        /// </summary>
        /// <param name="balise"></param>
        /// <returns></returns>
        public int CompareTo(Balise balise)
        {
            // ascending
            return position.CompareTo(balise.position);
        }

        public int Nid_lrbg
        {
            get { return (nid_c << 14) | nid_bg; }
        }

        public int Nid_bg
        {
            get
            {
                return nid_bg;
            }
        }

        public int Nid_c
        {
            get
            {
                return nid_c;
            }
        }

        public int Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public string BaliseName
        {
            get
            {
                return baliseName;
            }

            set
            {
                baliseName = value;
            }
        }

        public string BaliseNumber
        {
            get
            {
                return baliseNumber;
            }

            set
            {
                baliseNumber = value;
                ResolveInfoFromBaliseNumber();
            }
        }
        public bool IsSourced
        {
            get
            {
                return isSourced;
            }

            set
            {
                isSourced = value;
            }
        }

        public string Usage
        {
            get
            {
                return usage;
            }

            set
            {
                usage = value;
            }
        }

        public string Remark1
        {
            get
            {
                return remark1;
            }

            set
            {
                remark1 = value;
            }
        }

        public string Remark2
        {
            get
            {
                return remark2;
            }

            set
            {
                remark2 = value;
            }
        }

        public int N_pig
        {
            get
            {
                return n_pig;
            }
        }

    }
}
