using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Train.Utilities;

namespace Train.Data
{
    public class Database
    {
        const string DOWN_BALISE = "select Name,BaliseNum,Kilo,Type,`Use`,PS1,PS2 from down_balise;";//use 是mysql中关键字，使用时要用反引号``括起来
        const string UP_BALISE = "select Name,BaliseNum,Kilo,Type,`Use`,PS1,PS2 from up_balise;";

        public void Init()
        {
            ReadIniFile();
            Connect();
            InitTrainInfo();
            InitBaliseGroup(DOWN_BALISE, Trains.TrainDynamics.DownBgList);
            Trains.TrainDynamics.DownBgList.Sort();
            InitBaliseGroup(UP_BALISE, Trains.TrainDynamics.UpBgList);
            Trains.TrainDynamics.UpBgList.Sort();
        }

        public static void InitTrainInfo()
        {
            TrainInfo.TrainID = 0x123456;
            TrainInfo.L_TRAIN = 140;//设为140米
            //TrainInfo.NID_ENGINE = 10020632;
            TrainInfo.NID_XUSER = 123;
            TrainInfo.NID_OPERATIONAL = 0x12345678;
            TrainInfo.NC_TRAIN = 0;
            TrainInfo.V_MAXTRAIN = 70;// 70*5 km/h
            TrainInfo.M_LOADINGGAUGE = 0x00;
            TrainInfo.M_AXLELOAD = 40 * 2;
            TrainInfo.M_AIRTIGHT = 0;
            TrainInfo.NID_STMList.Add(3);//假定STM编号为3, CTCS-2
            TrainInfo.NID_RADIOList.Add(0x1234567890123456);//假定值
            // TrainInfo.M_TRACTION.Add(0x12);//假定值

        }

        public void InitBaliseGroup(string querySql, List<Balise> list)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                return;
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = querySql;
            //cmd.ExecuteNonQuery();        //用于增删改
            MySqlDataReader data = cmd.ExecuteReader();   //用于查找
            while (data.Read())
            {
                Balise balise = new Balise();
                balise.BaliseName=data.GetString(0);
                balise.BaliseNumber = data.GetString(1);
                string[] kilo = data.GetString(2).Split('+');
                balise.Position = Convert.ToInt32(kilo[0].Trim('k', 'K')) * 1000 + Convert.ToInt32(kilo[1]);
                balise.IsSourced = "有源".Equals(data[3]);
                balise.Usage = (data[4] == DBNull.Value) ? null : data.GetString(4);
                balise.Remark1 = (data[5] == DBNull.Value) ? null : data.GetString(5);
                balise.Remark2 = (data[6] == DBNull.Value) ? null : data.GetString(6);
                if (balise.Remark1 != null && (balise.Remark1.Contains("侧线") || balise.Remark1.Contains("三四线")))
                    continue;
                list.Add(balise);
            }
            data.Close();
        }

        public void ReadIniFile()
        {
            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\CommConfig.ini";
            IniFile file = new IniFile(fileName);
            if (file.ExistINIFile())
            {
                connectionString = file.IniReadValue("DataBase", "connstr");
            }
            else
            {
                throw new Exception("通信配置文件不存在，请确认配置文件路径是否正确");
            }
        }

        MySqlConnection conn;
        String connectionString;

        public void Connect()
        {
            conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                //MessageBox.Show("数据库连接成功！");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //conn.Close();
            }
        }
        public void Close()
        {
            if (conn != null)
                conn.Close();
        }

    }
}
