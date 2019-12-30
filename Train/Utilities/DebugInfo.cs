using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Train.Utilities
{
    public class DebugInfo
    {
        private static string dir = System.IO.Directory.GetCurrentDirectory()+"\\Tmp";
        private static TextWriterTraceListener defTextWriter = new TextWriterTraceListener(GetFilePath("debug", "txt"));
        private static Dictionary<string, TextWriterTraceListener> writeFileDict = new Dictionary<string, TextWriterTraceListener>();

        static DebugInfo()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        public static void WriteToFile(string message,string fileName)
        {
            TextWriterTraceListener t;
            if (writeFileDict.ContainsKey(fileName)) t = writeFileDict[fileName];
            else
            {
                t = new TextWriterTraceListener(GetFilePath(fileName, "txt"));
                writeFileDict.Add(fileName, t);
                Debug.Listeners.Add(t);
            }
            DateTime dt = DateTime.Now;
            t.WriteLine(dt.ToString()+"."+String.Format("{0:D3}",dt.Millisecond) + ": " + message);
            t.Flush();
        }

        public static void WriteToFile(string message)
        {
            if (!Debug.Listeners.Contains(defTextWriter))
            {
                Debug.Listeners.Add(defTextWriter);
            }
            defTextWriter.WriteLine(DateTime.Now.ToString() + ": " + message);
            defTextWriter.Flush();
        }

        public static void Assert(bool condition,string message)
        {
            Debug.Assert(condition, message);
        }

        public static bool Exists(string path)
        {          
            return File.Exists(path);
        }
        private static string GetFilePath(string baseName,string suffix)
        {
            DateTime dt = DateTime.Now;
            string dtStr = dt.Year.ToString();
            dtStr += string.Format("{0:D2}{1:D2}{2:D2}{3:D2}{4:D2}", dt.Month,
                dt.Day, dt.Hour, dt.Minute, dt.Second);
            return dir + "\\" + baseName + "_" +dtStr+"."+ suffix;
        }
    }
}
