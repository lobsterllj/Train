using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Train.Utilities;

namespace Train.XmlResolve
{
    public class XmlParser
    {
        static XmlDocument senddoc = new XmlDocument();
        static XmlDocument recvdoc = new XmlDocument();
        static string path = System.IO.Directory.GetCurrentDirectory()+"\\..\\..\\XmlResolve";
        public static byte[] SendData(byte[] msg)
        {
            if("".Equals(senddoc.BaseURI))
                senddoc.Load(path+"\\SendData.xml");
             String str = Convert.ToBase64String(msg);
            XmlElement root = senddoc.DocumentElement;
            XmlElement sadata = (XmlElement)senddoc.SelectSingleNode("requests/datas/data/sadata");
            sadata.InnerText=str;
            string ss = senddoc.InnerXml;
            return Encoding.UTF8.GetBytes(ss);
        }
        public static byte[] RecvData(byte[] recvBytes)
        {
            string recv = Encoding.UTF8.GetString(recvBytes);
            recvdoc.LoadXml(recv);

            XmlElement sadata = (XmlElement)recvdoc.SelectSingleNode("indications/datas/data/sadata");
            string str = sadata.InnerText;

            return Convert.FromBase64String(str);
        }
        public static byte[] ConnReq(int ctcsid)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path+"\\ConnReq.xml");
            String str = ""+ctcsid;
            XmlElement element = (XmlElement)doc.SelectSingleNode("requests/connects/connect/ctcsid");
            element.InnerText = str;
            string ss = doc.InnerXml;
            return Encoding.UTF8.GetBytes(ss);
        }
        public static void ConnAck(byte[]recvBytes,out int calledctcsid)
        {
            XmlDocument doc = new XmlDocument();
            string recv = Encoding.UTF8.GetString(recvBytes);
            doc.LoadXml(recv);

            XmlElement element = (XmlElement)doc.SelectSingleNode("indications/connects/connect/callingctcsid");
            string str = element.InnerText;
            calledctcsid = Convert.ToInt32(str);
        }

        public static byte[] Disconnect()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path + "\\Disconnect.xml");
            string ss = doc.InnerXml;
            return Encoding.UTF8.GetBytes(ss);
        }
        // ISDNIF would send disconnect indication initiatively
        public static  bool IsDisconnectRecved(byte[] recvBytes)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                string recv = Encoding.UTF8.GetString(recvBytes);
                doc.LoadXml(recv);

                XmlElement element = (XmlElement)doc.SelectSingleNode("indications/disconnects/disconnect/reason");
                String str = element.InnerText;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
