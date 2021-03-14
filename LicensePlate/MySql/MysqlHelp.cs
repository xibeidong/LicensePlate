using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;

namespace LicensePlate
{
    public class MysqlHelp
    {
        public static MysqlHelp Instance = new MysqlHelp();
        private MysqlHelp()
        {

        }
        ~MysqlHelp()
        {
            if (sqlConn.State == ConnectionState.Open)
            {
                sqlConn.Close();
            }
           
        }
        private string datasource = "localhost";
        private string user = "root";
        private string password = "123456";
        private string database = "licenseplate";
        private string connStr = "Database=licenseplate;datasource=127.0.0.1;port=3306;user=root;pwd=123456;";

        private MySqlConnection sqlConn;
        private System.Timers.Timer t;

        private bool isinit = false;

        private void Readini()
        {
            datasource = IniFiles.iniFile.IniReadValue("mysql", "datasource");
            user = IniFiles.iniFile.IniReadValue("mysql", "user");
            password = IniFiles.iniFile.IniReadValue("mysql", "password");
            database = IniFiles.iniFile.IniReadValue("mysql", "database");
            connStr = string.Format("Database={0};datasource={1};port=3306;user={2};pwd={3};", database, datasource, user, password);

        }

        public  void check()
        {
            Readini();
            connStr = string.Format("datasource={0};port=3306;user={1};pwd={2};",  datasource, user, password);

            //尝试连接Mysql服务
            MySqlConnection myConnnect = new MySqlConnection(connStr);
            try
            {
                myConnnect.Open();
                myConnnect.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "（无法连接Mysql服务）");
                return;
               
            }

            //尝试连接库licenseplate
            connStr = string.Format("Database={0};datasource={1};port=3306;user={2};pwd={3};", database, datasource, user, password);
            MySqlConnection myConnnect2 = new MySqlConnection(connStr);
            try
            {
                myConnnect2.Open();
                myConnnect2.Close();
                MessageBox.Show("数据库正常");
              //  return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message+ "(数据库连接成功，正在更新)");
            }

            //初始化数据库(建库建表)
            string connectStr = string.Format("datasource={0};port=3306;user={1};pwd={2};", datasource, user, password);
            MySqlConnection conn = new MySqlConnection(connectStr);
            string commandStr = string.Format("CREATE DATABASE IF NOT EXISTS {0};", database);
            MySqlCommand cmd = new MySqlCommand(commandStr, conn);
            
            conn.Open();
            int ret = cmd.ExecuteNonQuery();
            if(ret == 1)
            {
                Log.myLog.Info(string.Format("{0}建立成功", database));
            }
          

            conn.Close();
            // 建表 

           
          //  CreatTable1();

            //表一：
            string sql = "CREATE TABLE IF NOT EXISTS blacklist (chepai varchar(50) not null, add_time datetime,do_user varchar(50),PRIMARY KEY (chepai))";
            bool ret2 = CreatTableBySqlStr(sql);
            if (!ret2)
            {
                MessageBox.Show("初始化失败，创建表1失败！");
            }
            //表二：
            sql = "CREATE TABLE IF NOT EXISTS account(username varchar(50) not null,password varchar(50) not null,add_time datetime not null,level int ,PRIMARY KEY (username)) ";
            ret2=CreatTableBySqlStr(sql);
            if (!ret2)
            {
                MessageBox.Show("初始化失败，创建表2失败！");
            }
            /*
           *表三：id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图

          */
            sql = "CREATE TABLE IF NOT EXISTS chepai (ID int not null auto_increment, in_time datetime,out_time datetime,in_weight double,out_weight double,suttle double,in_chepai varchar(50),out_chepai varchar(50),in_img varchar(200),out_img varchar(200),state tinyint(10) DEFAULT 0, PRIMARY KEY (ID))";
            ret2 = CreatTableBySqlStr(sql);
            if (!ret2)
            {
                MessageBox.Show("初始化失败，创建表3失败！");
            }

            //表四：
            sql = "CREATE TABLE IF NOT EXISTS whitelist (chepai varchar(50) not null, add_time datetime,do_user varchar(50),PRIMARY KEY (chepai))";
            ret2 = CreatTableBySqlStr(sql);
            if (!ret2)
            {
                MessageBox.Show("初始化失败，创建表4失败！");
            }

            sql = "INSERT IGNORE INTO account ()VALUES('admin','123456',NOW(),0) ";
            ret2 = CreatTableBySqlStr(sql);
            if (!ret2)
            {
                MessageBox.Show("初始化失败，admin写入失败！");
            }
        }

        private void CreatTable1()
        {
            connStr = string.Format("Database={0};datasource={1};port=3306;user={2};pwd={3};", database, datasource, user, password);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            // 建表 

            /*
           *表三：id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图

          */
            string creatTable1 = "CREATE TABLE chepai (ID int not null auto_increment, in_time datetime,out_time datetime,in_weight double,out_weight double,suttle double,in_chepai varchar(50),out_chepai varchar(50),in_img varchar(200),out_img varchar(200),state tinyint(10) DEFAULT 0, PRIMARY KEY (ID))";

            using (MySqlCommand cmd2 = new MySqlCommand(creatTable1, conn))
            {
               int ret = cmd2.ExecuteNonQuery();
                if (ret >= 0){
                    MessageBox.Show("初始化完成");
                }
            }
            conn.Close();

        }

        private bool CreatTableBySqlStr(string sql)
        {
            connStr = string.Format("Database={0};datasource={1};port=3306;user={2};pwd={3};", database, datasource, user, password);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            using (MySqlCommand cmd2 = new MySqlCommand(sql, conn))
            {
                int ret = cmd2.ExecuteNonQuery();
                if (ret >= 0)
                {
                    //MessageBox.Show("建表成功！");
                }
                else
                {
                    conn.Close();
                   
                    return false;
                }
            }
            conn.Close();
            return true;

        }
        public void init()
        {
            if (isinit)
            {
                return;
            }
            Readini();
            sqlConn = new MySqlConnection(connStr);
            try
            {
                sqlConn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("数据库连接失败，记录将不被保存！"+e.Message);
                return;
            }
         


            t = new System.Timers.Timer(1000*3600);
            t.Elapsed += new System.Timers.ElapsedEventHandler(theout);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
            isinit = true;
        }
        //定时执行一下，防止连接停止活跃
        private void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("mysql 连接检测");
            string str = "SELECT COUNT(*) FROM chepai";
            using (MySqlCommand cmd2 = new MySqlCommand(str, sqlConn))
            {
               // int ret = cmd2.ExecuteNonQuery();
                var ret =  cmd2.ExecuteScalar();
                Log.myLog.Info("SELECT COUNT(*) FROM chepai 执行结果："+ret.ToString());
            }
        }

        public int DoInsert(string commdStr)
        {
            int ret = -1;
            using (MySqlCommand cmd2 = new MySqlCommand(commdStr, sqlConn))
            {
                try
                {
                    ret = cmd2.ExecuteNonQuery();
                   // Log.myLog.Info(commdStr + " 执行结果：" + ret.ToString());

                    long newid = cmd2.LastInsertedId;
                    return (int)newid;
                }
                catch (Exception e)
                {

                    Log.myLog.Error(commdStr + " 执行错误：" + e.Message);
                }

            }
            return ret;
        }
        public int Do(string commdStr)
        {
            int ret = 0;
            using (MySqlCommand cmd2 = new MySqlCommand(commdStr, sqlConn))
            {
                try
                {
                    ret = cmd2.ExecuteNonQuery();
                    //Log.myLog.Info(commdStr + " 执行结果：" + ret.ToString());
                }
                catch (Exception e)
                {

                    Log.myLog.Error(commdStr + " 执行错误：" + e.Message);
                }
               
            }
            return ret;
        }

        public MySqlDataReader DoGetReader(string commdStr)
        {
            MySqlDataReader ret = null;
            using (MySqlCommand cmd2 = new MySqlCommand(commdStr, sqlConn))
            {
                try
                {
                    ret = cmd2.ExecuteReader();
                  //  Log.myLog.Info(commdStr + " 执行结果：" + ret.ToString());
                }
                catch (Exception e)
                {

                    Log.myLog.Error(commdStr + " 执行错误：" + e.Message);
                }

            }
            return ret;
        }
       
    }
}
