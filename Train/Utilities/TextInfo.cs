using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Utilities
{
    class TextInfo
    {
        private string msg;
        private DateTime dt;
        private const int QUEUE_LEN = 31;
        private static CircularQueue<TextInfo> textInfoQueue = new CircularQueue<TextInfo>(QUEUE_LEN);

        TextInfo(string msg,DateTime dt)
        {
            this.msg = msg;
            this.dt = dt;
        }

        public static void Add(string msg)
        {
            TextInfo ti = new TextInfo(msg, DateTime.Now);
            DebugInfo.WriteToFile(ti.ToString(), "TextInfo");
            textInfoQueue.Push(ti);
        }

        public static string GetAllText()
        {
            string res = "";
            foreach(TextInfo ti in textInfoQueue)
            {
                res += ti;
            }
            return res;
        }

        public override string ToString()
        {
            return dt.ToString() + ":" + msg + "\r\n";
        }

    }
}
