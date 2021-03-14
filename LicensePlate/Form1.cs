using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//识别车牌-》红外检测-》称重稳定-》生成记录-》打印  
namespace LicensePlate
{
    public partial class Form1 : Form
    {

        public CameraLicense m_CameraLicense;
        public Loadometer m_Loadometer;
        public RedSwitch m_RedSwitch;
        public LEDControl m_LEDControl;

        float xvalues;
        float yvalues;

        MessageShowForm f1 = new MessageShowForm("入厂围栏有遮挡");
       // MessageShowForm f2 = new MessageShowForm("出厂围栏有遮挡");
        void LogToRichText(string message)
        {
            string t = System.DateTime.Now.ToString("MM-dd HH:mm");
            richTextBox_Log.AppendText(t + ":" + message + "\r\n");
        }
        void ShowHideMessageForm(string message , bool isShow)
        {
            if (isShow)
            {
                f1.Show();
                f1.SetText(message);
                LogToRichText(message);
            }
            else
            {
                f1.Hide();
            }
            
        }
        public Form1()
        {

           if( (0x01 & 0x01) == 0x01)
            {
                Console.WriteLine(1);
            }
           // test();
          
            //0x58  1011000  // 01 02 00 00 00 10 79 c6
            InitializeComponent();
            //0x01,0x02,0x00,0x00,0x01,0x00
           // byte[] data = new byte[] { 0x01, 0x02, 0xf0, 0x9f, 0x70, 0x10 };//121 154 0x79,0x9A
           //Console.WriteLine( BitConverter.ToString(data));
        }

        #region Test
        void testCRC()

        {
            byte[] msg = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00 };//0x98 0x35
            var b = CRC.CRC16(msg);//0x35 0x98,刚好颠倒了
        }
       
        void testSerial()
        {
            byte[] msg = new byte[] { 0x55, 0x53, 0x2C, 0x47,0x53 ,0x2C ,0x2B ,0x30 ,0x30 ,0x30,0x30 ,0x30 ,0x35 ,0x30, 0x6B,0x67,0x0D,0x0A };//50kg
           // byte[] msg = new byte[] { 0x55, 0x53};//50kg

            string str =  Encoding.ASCII.GetString(msg);//US,GS,+0000050kg

           // string str = Encoding.ASCII.GetString(bytes);//US,GS,+0000050kg CR LF
            string[] strs = str.Split(',');
            string w = strs[2];//+0000050kg
            int index = 0;
            if (w.Length >= 8)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (w[i] != '0')
                    {
                        index = i;
                        break;
                    }
                }

                string show_w = w.Substring(index, 8 - index);
                label_in_weight.Text = w.Substring(0,1)+show_w;
            }

        }

        void testSQl()
        {
            string path = System.IO.Directory.GetCurrentDirectory() + "\\cap\\2111.jpg";
            path = path.Replace("\\", "\\\\");
            string sqlStr = string.Format("insert into chepai (in_img,out_img) values ('{0}','{1}') ",
               path,path);
            int newID = MysqlHelp.Instance.DoInsert(sqlStr);


        }

        void test()
        {
            string ttt = "-120";
            int bb = int.Parse(ttt);


            string numStr = "+33.34";
            double n = double.Parse(numStr);
            numStr = "-12.345";
            n = double.Parse(numStr);

            List<string> list1 = new List<string>();
            list1.Add("abc");

            string b = list1.Find(x => x == "bc");

            string a = list1.Find(x => x == "abc");

            list1.Remove(a);

            a = list1.Find(x => x == "abc");

        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
           
          //  testSerial();
            //Log.myLog.Trace("trace");
            //Log.myLog.Info("info");
            //Log.myLog.Warn("warn");

            //Log.myLog.Error("error");
            //Log.myLog.Fatal("fatal");
            this.Resize += new EventHandler(MainForm_Resize); //添加窗体拉伸重绘事件
            xvalues = this.Width;//记录窗体初始大小
            yvalues = this.Height;
            SetTag(this);
            Init();
            button_AddBlackList.Hide();
           
            foreach (ToolStripMenuItem item in this.menuStrip1.Items)
            {
               // Console.WriteLine(item.Name);
                foreach (ToolStripMenuItem subItem in item.DropDownItems)
                {
                   // Console.WriteLine(subItem.Name);
                    if (subItem.Name != "登录ToolStripMenuItem1" )
                    {
                        subItem.Enabled = false;
                    }
                    if (subItem.Name.Contains("检查数据库"))
                    {
                        subItem.Enabled = true;
                    }
                }
            }

            // testSQl();
          
        }

        public void Init()
        {
            if (!IniFiles.iniFile.ExistINIFile())
            {
                MessageBox.Show("配置文件sett.ini缺失！请退出！");
                return;
            }
            Manager.Instance.init();
            Manager.Instance.InsertIndexListView = new Manager.InsertIndexListViewDelegate(insertToListView1);
            Manager.Instance.UpdeteIndexListView = new Manager.UpdeteIndexListViewDelegate(updateListView1);
            Manager.Instance.ShowHideMessage = new Manager.ShowMessageDelegate(ShowHideMessageForm);
            Manager.Instance.GetIDbyInChepai += GetIDbyInchepai;
            Manager.Instance.LogToRichText += LogToRichText;
            Manager.Instance.LoginSuccess += LoginSucess;

            m_CameraLicense = new CameraLicense();
            m_CameraLicense.Init();
            m_CameraLicense.UpdateInChepai = new CameraLicense.UpdateINChepaiLabelDelegate(UpdateInchepaiLabel);
            m_CameraLicense.UpdateOutChepai = new CameraLicense.UpdateOUTChepaiLabelDelegate(UpdateOutchepaiLabel);
            m_CameraLicense.UpdateInChepaiImage = new CameraLicense.UpdateINChepaiImageDelegate(UpdateInchepaiImage);
            m_CameraLicense.UpdateOutChepaiImage = new CameraLicense.UpdateOUTChepaiImageDelegate(UpdateOutchepaiImage);

            m_Loadometer = new Loadometer();
            m_Loadometer.UpdateInweight = new Loadometer.UpdateInweightDelegate(UpdateInweight);
            m_Loadometer.UpdateInweightState = new Loadometer.UpdateInweightStateDelegate(UpdateInweightState);
            m_Loadometer.UpdateInweightConnect = new Loadometer.UpdateInweightConnectDelegate(UpdateInweightConnect);
            m_Loadometer.UpdateOutweight = new Loadometer.UpdateOutweightDelegate(UpdateOutweight);
            m_Loadometer.UpdateOutweightState = new Loadometer.UpdateOutweightStateDelegate(UpdateOutweightState);
            m_Loadometer.UpdateOutweightConnect = new Loadometer.UpdateOutweightStateDelegate(UpdateOutweightConnect);

            m_RedSwitch = new RedSwitch();
            Manager.Instance.redSwitch = m_RedSwitch;
            m_RedSwitch.UpdateRedSwitchUI = new RedSwitch.UpdataRedSwitchUIDelegate(UpdeteRedSwitchUI);

            m_LEDControl = new LEDControl();
            Manager.Instance.ledControl = m_LEDControl;
           

            initListview1();
            initWhiteList();
        }

        private void LoginSucess()
        {
            foreach (ToolStripMenuItem item in this.menuStrip1.Items)
            {
                Console.WriteLine(item.Name);
                foreach (ToolStripMenuItem subItem in item.DropDownItems)
                {
                    Console.WriteLine(subItem.Name);
                    if (subItem.Name != "登录ToolStripMenuItem1")
                    {
                        subItem.Enabled = true;
                    }
                    else
                    {
                        subItem.Enabled = false;
                    }
                }
            }
        }
        private void MainForm_Resize(object sender, EventArgs e)//重绘事件
        {
            float newX = this.Width / xvalues;//获得比例
            float newY = this.Height / yvalues;
            SetControls(newX, newY, this);
        }
        private void SetControls(float newX, float newY, Control cons)//改变控件的大小
        {
            foreach (Control con in cons.Controls)
            {
                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newX;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newY;
                con.Height = (int)a;
                a = Convert.ToSingle(mytag[2]) * newX;
                con.Left = (int)a;
                a = Convert.ToSingle(mytag[3]) * newY;
                con.Top = (int)a;
                Single currentSize = Convert.ToSingle(mytag[4]) * newY;

                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    SetControls(newX, newY, con);
                }
            }
        }
        /// <summary>
        /// 遍历窗体中控件函数
        /// </summary>
        /// <param name="cons"></param>
        private void SetTag(Control cons)
        {
            foreach (Control con in cons.Controls)  //遍历窗体中的控件,记录控件初始大小
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    SetTag(con);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Log.myLog.Info(string.Format("'{0}'", label_in_chepai.Text));
        }

       
        private void 检查数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            MysqlHelp.Instance.check();
        }
        #region 车牌识别
        private void 打开设备1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CameraLicense.OpenDevice1(pictureBox_in_video.Handle);

        }

        private void 打开设备2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CameraLicense.OpenDevice2(pictureBox_out_video.Handle);
        }

        public void UpdateInchepaiLabel(string str)
        {
            label_in_chepai.Text = str;
            Manager.Instance.m_inChepai = label_in_chepai.Text;
        }
        public void UpdateOutchepaiLabel(string str)
        {
            label_out_chepai.Text = str;
            Manager.Instance.m_OutChepai = label_out_chepai.Text;
        }
        public void UpdateInchepaiImage(string path)
        {
            pictureBox_in_img.Image = Image.FromFile(path);
            Manager.Instance.m_inImgPath = path;//写入数据库的时候需要两次转义
                                                                      // Manager.instance.m_inImgPath = path.Replace("\\","\\\\");//写入数据库的时候需要两次转义
        }

        public void UpdateOutchepaiImage(string path)
        {
            pictureBox_out_img.Image = Image.FromFile(path);
            Manager.Instance.m_outImgPath = path;
           // Manager.instance.m_outImgPath = path.Replace("\\", "\\\\");
        }

        public void UpdateInweight(string str)
        {
            label_in_weight.Text = str;
            Manager.Instance.m_inWeight = double.Parse(label_in_weight.Text);
        }

        public void UpdateOutweight(string str)
        {
            label_out_weight.Text = str;
            Manager.Instance.m_outWeight = double.Parse( label_out_weight.Text);
        }

        public void UpdateInweightState(string str)
        {
            if (str.Contains("不稳定"))
            {
                label_in_state.ForeColor = Color.Red;
            }
            else
            {
                label_in_state.ForeColor = Color.Black;
            }
            label_in_state.Text = str;
        }
        public void UpdateOutweightState(string str)
        {
            if (str.Contains("不稳定"))
            {
                label_out_state.ForeColor = Color.Red;
            }
            else
            {
                label_out_state.ForeColor = Color.Black;
            }
            label_out_state.Text = str;
        }

        public void UpdateInweightConnect(string str)
        {
            label_in_connect.Text = str;
        }

        public void UpdateOutweightConnect(string str)
        {
            label_out_connect.Text = str;
        }
        #endregion

        #region ListView
        private void initListview1()
        {
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            ColumnHeader c1 = new ColumnHeader();
            ColumnHeader c2 = new ColumnHeader();
            ColumnHeader c3 = new ColumnHeader();
            ColumnHeader c4 = new ColumnHeader();
            ColumnHeader c5 = new ColumnHeader();
            ColumnHeader c6 = new ColumnHeader();
            ColumnHeader c7 = new ColumnHeader();
            ColumnHeader c8 = new ColumnHeader();
            ColumnHeader c9 = new ColumnHeader();
            c1.Width = 100;
            c2.Width = 200;
            c3.Width = 200;
            c4.Width = 100;
            c5.Width = 100;
            c6.Width = 100;
            c7.Width = 100;
            c8.Width = 200;
            c9.Width = 200;

            c1.Text = "ID";
            c2.Text = "入厂时间";
            c3.Text = "出厂时间";
            c4.Text = "入厂重量";
            c5.Text = "出厂重量";
            c6.Text = "货物重量";
            c7.Text = "车牌号";
            c8.Text = "入厂截图";
            c9.Text = "出厂截图";

            listView1.GridLines = true;  //显示网格线
            listView1.FullRowSelect = true;  //显示全行
            listView1.MultiSelect = false;  //设置只能单选
            listView1.View = View.Details;  //设置显示模式为详细
            listView1.HoverSelection = true;  //当鼠标停留数秒后自动选择

            listView1.Columns.Add(c1);
            listView1.Columns.Add(c2);
            listView1.Columns.Add(c3);
            listView1.Columns.Add(c4);
            listView1.Columns.Add(c5);
            listView1.Columns.Add(c6);
            listView1.Columns.Add(c7);
            listView1.Columns.Add(c8);
            listView1.Columns.Add(c9);

            //for (int i = 0; i < 30; i++)
            //{
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            //insertToListView1("1,2020-03-12 07:05:43,,+11,,,che1,,,,,");
            //insertToListView1("2,234,234,,345,234,che2,,,,,,,");
            //insertToListView1("3,,2020-03-12 08:05:43,,+345,,che3,,,,,,");
            //}

            initListView1Data();

        }
        void initListView1Data()
        {
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
           

            string commdStr = "select * from chepai where out_time is null or in_time is null ";

            MySqlDataReader dr = MysqlHelp.Instance.DoGetReader(commdStr);
            if (dr != null)
            {
                listView1.Items.Clear();
                while (dr.Read())
                {
                    string id = dr["ID"].ToString();
                    string in_time = dr["in_time"].ToString();
                    string out_time = dr["out_time"].ToString();
                    string in_weight = dr["in_weight"].ToString();
                    string out_weight = dr["out_weight"].ToString();
                    string chepai = dr["in_chepai"].ToString();
                    if (chepai=="")
                    {
                      chepai = dr["out_chepai"].ToString();
                    }
                  
                    string in_img = dr["in_img"].ToString();
                    string out_img = dr["out_img"].ToString();
                    string str;
                    //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
                    str = string.Format("{0},{1},{2},{3},{4},,{5},{6},{7}", id, in_time, out_time, in_weight, out_weight, chepai,
                        in_img, out_img);
                    insertToListView1(str);

                    if (in_time == "")
                    {
                        //Manager.instance.m_cheOutList.Add(chepai);
                    }else if (out_time == "")
                    {
                        Manager.Instance.m_cheInList.Add(chepai);
                    }
                 
                }
                dr.Close();
            }

        }

        void initWhiteList()
        {
            string sql = "select chepai from whitelist";
            MySqlDataReader dr = MysqlHelp.Instance.DoGetReader(sql);
            if (dr!=null)
            {
                while (dr.Read())
                {
                    Manager.Instance.m_whiteList.Add(dr["chepai"].ToString());
                }
                dr.Close();
            }
          
        }

        string GetIDbyInchepai(string in_chepai)
        {
            //for (int i = listView1.Items.Count-1;  i>=0; i--)
            //{
            //    if (listView1.Items[i].SubItems[6].Text == in_chepai)
            //    {
            //        return listView1.Items[i].SubItems[0].Text;
            //    }
            //}

            for (int i = 0; i< listView1.Items.Count ;  i++)
            {
                string chepai = listView1.Items[i].SubItems[6].Text;
                string inWeight = listView1.Items[i].SubItems[3].Text;
                if ( chepai== in_chepai && !string.IsNullOrEmpty(inWeight))
                {
                    return listView1.Items[i].SubItems[0].Text;
                }
            }
            return null;
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="update_str">"out_time,out_weight,chepai,out_img"</param>
        /// <param name="condition_str">"chepai,"</param>
        private void updateListView1(string update_str)
        {
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            string[] strs = update_str.Split(',');
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].SubItems[6].Text == strs[2] && listView1.Items[i].SubItems[1].Text != "")
                {
                    listView1.Items[i].SubItems[2].Text = strs[0];
                    listView1.Items[i].SubItems[4].Text = strs[1];
                    listView1.Items[i].SubItems[8].Text = strs[3];

                    //计算重量 3入厂，4出厂 5净重
                    int w_in = int.Parse(listView1.Items[i].SubItems[3].Text);
                    int w_out = int.Parse(listView1.Items[i].SubItems[4].Text);
                    listView1.Items[i].SubItems[5].Text = (w_in-w_out).ToString();
                    listView1.Items[i].BackColor = Color.White;
                    return ;

                }
            }
            return ;
        }

        private void insertToListView1(string str)
        {
            string[] strs = str.Split(',');
            //创建行对象
            ListViewItem li = new ListViewItem(strs[0]);
            if (strs[1]=="") //入厂时间为空
            {
                li.BackColor = Color.Chocolate;
            }
            if (strs[2] == "") //出厂时间为空
            {
                li.BackColor = Color.Cyan;
            }

            for (int i = 1; i < strs.Length; i++)
            {
                li.SubItems.Add(strs[i]);
            }
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            //计算重量
            if (strs[3]!=""&&strs[4]!="")
            {
                try
                {
                    int w1 = int.Parse(strs[3]);
                    int w2 = int.Parse(strs[4]);
                    li.SubItems[5].Text = "" + (w1 - w2);
                }
                catch (Exception e)
                {
                    Log.myLog.Error(" w1= " + strs[3] + " ; w2 = " + strs[4]+" ====>计算重量时出错：" +e.Message);
                    
                }
              

            }
           

            //添加同一行的数据
            //li.SubItems.Add("234");
            //li.SubItems.Add("234");
            //li.SubItems.Add("234");
            //li.SubItems.Add("");
            //li.SubItems.Add("234");
            //li.SubItems.Add("234");
            //将行对象绑定在listview对象中
            //listView1.Items.Add(li); //在后面添加数据
            listView1.Items.Insert(0, li); //永远添加在第一行

            if (listView1.Items.Count>500)
            {
                listView1.Items.RemoveAt(500);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.FocusedItem != null)

            {
                if (this.listView1.SelectedItems != null)
                {
                    foreach (ListViewItem item in this.listView1.SelectedItems)
                    {
                        item.Checked = false;
                        textBox_chepai.Text = item.SubItems[6].Text;
                        
                        //图像显示出来
                        if (item.SubItems[7].Text!="")
                        {
                            if (File.Exists(item.SubItems[7].Text))
                            {
                                pictureBox_in_img.Image = Image.FromFile(item.SubItems[7].Text);
                                Manager.Instance.m_inImgPath = item.SubItems[7].Text;
                            }
                            else
                            {
                                pictureBox_in_img.Image = Image.FromFile("null.jpg");
                                MessageBox.Show("不存在入厂图像：" + item.SubItems[7].Text);
                            }

                        }
                        else
                        {
                            pictureBox_in_img.Image = Image.FromFile("null.jpg");
                        }
                        if (item.SubItems[8].Text != "")
                        {
                            if (File.Exists(item.SubItems[8].Text))
                            {
                                pictureBox_out_img.Image = Image.FromFile(item.SubItems[8].Text);
                                Manager.Instance.m_outImgPath = item.SubItems[8].Text;
                            }
                            else
                            {
                                pictureBox_out_img.Image = Image.FromFile("null.jpg");
                                MessageBox.Show("不存在出厂图像：" + item.SubItems[8].Text);
                            }
                               
                        }
                        else
                        {
                            pictureBox_out_img.Image = Image.FromFile("null.jpg");
                        }

                    }
                }
            }
        }

        private void button_change_Click(object sender, EventArgs e)
        {
            if (this.listView1.FocusedItem != null)

            {
                if (this.listView1.SelectedItems != null)
                {
                    foreach (ListViewItem item in this.listView1.SelectedItems)
                    {
                        // MessageBox.Show(item.SubItems[6].ToString());
                        if (item.SubItems[1].Text==""|| item.SubItems[2].Text=="")//入厂时间和出厂时间有一个为空才能修改车牌，完整记录不能炒作
                        {
                            item.SubItems[6].Text = textBox_chepai.Text;
                            string in_time_str = item.SubItems[1].Text;
                            string id_str = item.SubItems[0].Text;
                            string sql;
                            if (in_time_str=="")
                            {
                                sql = $"update chepai set out_chepai='{textBox_chepai.Text}' where ID={id_str}";
                            }
                            else
                            {
                                sql = $"update chepai set in_chepai='{textBox_chepai.Text}' where ID={id_str}";
                            }
                           int ret = MysqlHelp.Instance.Do(sql);
                            if (ret==1)
                            {
                                MessageBox.Show("修改成功");
                            }
                            else
                            {
                                MessageBox.Show("修改失败");
                            }

                        }
                        else
                        {
                            MessageBox.Show("不允许操作完整记录");
                        }
                       
                    }
                }
            }
        }
        void hebing()
        {
           
            if (listView1.CheckedItems.Count!=2)
            {
                MessageBox.Show("选中了" + listView1.CheckedItems.Count+"行数据，无法合并记录");
                return;
            }
            ListViewItem item1 = listView1.CheckedItems[0];
            ListViewItem item2 = listView1.CheckedItems[1];
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            if (item1.SubItems[6].Text!= item2.SubItems[6].Text)
            {
                MessageBox.Show("两条记录车牌号不同，请修改后重试！");
                return;
            }

            if ((item1.SubItems[1].Text!=""&&item2.SubItems[1].Text!="")||(item1.SubItems[2].Text != "" && item2.SubItems[2].Text != ""))
            {
                MessageBox.Show("合并失败，请检查出入时间是否符合逻辑！");
                return;
            }
           
            string in_time = "";
            string out_time = "";
            string in_weight = "";
            string out_weight = "";
            string suttle = "";
            string chepai = "";
            string in_img = "";
            string out_img = "";
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            if (item1.SubItems[1].Text != "")
            {
                 in_time = item1.SubItems[1].Text;
                 out_time = item2.SubItems[2].Text;
                 in_weight = item1.SubItems[3].Text;
                 out_weight = item2.SubItems[4].Text;
                 suttle = "";
                 chepai = item1.SubItems[6].Text;
                 in_img = item1.SubItems[7].Text;
                 out_img = item2.SubItems[8].Text;
            }
            else
            {
                in_time = item2.SubItems[1].Text;
                out_time = item1.SubItems[2].Text;
                in_weight = item2.SubItems[3].Text;
                out_weight = item1.SubItems[4].Text;
                suttle = "";
                chepai = item2.SubItems[6].Text;
                in_img = item2.SubItems[7].Text;
                out_img = item1.SubItems[8].Text;
            }

            if (Convert.ToDateTime(in_time) > Convert.ToDateTime(out_time))
            {
                MessageBox.Show("合并失败，请检查出入时间是否符合逻辑！");
                return;
            }
            item1.Remove();
            item2.Remove();
            

            //更新一下数据库，先删除两条不完整信息，再写入一条完整的
            string sqlStr = string.Format("delete from chepai where in_time='{0}'", in_time);
            MysqlHelp.Instance.Do(sqlStr);
            sqlStr = string.Format("delete from chepai where out_time='{0}'", out_time);
            MysqlHelp.Instance.Do(sqlStr);

            //string sql_in_img = in_img.Replace("\\", "\\\\");
            //string sql_out_img = out_img.Replace("\\", "\\\\");
            sqlStr = string.Format("insert into chepai (in_time,out_time,in_weight,out_weight,in_chepai,out_chepai,in_img,out_img) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}') ",
                in_time, out_time, in_weight, out_weight, chepai, chepai, in_img.Replace("\\", "\\\\"), out_img.Replace("\\", "\\\\"));
            int newId = MysqlHelp.Instance.DoInsert(sqlStr);

            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            insertToListView1(string.Format("{7},{0},{1},{2},{3},{7},{4},{5},{6}", in_time, out_time, in_weight, out_weight, chepai, in_img, out_img, suttle,newId));
        }
        private void button_hebing_Click(object sender, EventArgs e)
        {
            hebing();
            return;
        }

        #endregion
        #region 地磅
        private void 打开入厂地磅ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Loadometer.OpenDevice1();
        }
      
        private void 打开出厂地磅ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Loadometer.OpenDevice2();
        }

       

        #endregion

     
       

        private void 开启入厂红外围栏检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.Instance. ignore_in_redSwitch = false;
        }

        private void 忽略入厂红外围栏检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.Instance.ignore_in_redSwitch = true;
        }

        private void 开启出厂红外围栏检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.Instance.ignore_out_redSwitch = false;
        }

        private void 忽略出厂红外围栏检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.Instance.ignore_out_redSwitch = true;
        }

        public void UpdeteRedSwitchUI(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8)
        {
            if (Manager.Instance.ignore_in_redSwitch)
            {
                pictureBox_in_left.BackColor = Color.LightGray;
                pictureBox_in_top.BackColor = Color.LightGray;
                pictureBox_in_right.BackColor = Color.LightGray;
                pictureBox_in_bottom.BackColor = Color.LightGray;
            }
            else
            {
                if (n1 > 0)
                {
                    pictureBox_in_left.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_in_left.BackColor = Color.Red;
                }

                if (n2 > 0)
                {
                    pictureBox_in_top.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_in_top.BackColor = Color.Red;
                }
                if (n3 > 0)
                {
                    pictureBox_in_right.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_in_right.BackColor = Color.Red;
                }
                if (n4 > 0)
                {
                    pictureBox_in_bottom.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_in_bottom.BackColor = Color.Red;
                }

            }

            if (Manager.Instance.ignore_out_redSwitch)
            {
                pictureBox_out_left.BackColor = Color.LightGray;
                pictureBox_out_top.BackColor = Color.LightGray;
                pictureBox_out_right.BackColor = Color.LightGray;
                pictureBox_out_bottom.BackColor = Color.LightGray;

            }
            else
            {
                if (n5 > 0)
                {
                    pictureBox_out_left.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_out_left.BackColor = Color.Red;
                }

                if (n6 > 0)
                {
                    pictureBox_out_top.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_out_top.BackColor = Color.Red;
                }
                if (n7 > 0)
                {
                    pictureBox_out_right.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_out_right.BackColor = Color.Red;
                }
                if (n8 > 0)
                {
                    pictureBox_out_bottom.BackColor = Color.Green;
                }
                else
                {
                    pictureBox_out_bottom.BackColor = Color.Red;
                }
            }

        }
        private void 打开入厂红外设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_RedSwitch.OpenDevice();
        }

       
        private void button_in_qi_Click(object sender, EventArgs e)
        {
            m_RedSwitch.RiseIn();
        }

        private void button_in_luo_Click(object sender, EventArgs e)
        {
            m_RedSwitch.DropIn();
        }

        private void button_out_qi_Click(object sender, EventArgs e)
        {
            m_RedSwitch.RiseOut();
        }

        private void button_out_luo_Click(object sender, EventArgs e)
        {
            m_RedSwitch.DropOut();
        }

        private void 历史记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecordForm recordForm = new RecordForm();
            recordForm.Show();
        }

        private void 入厂显示屏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_LEDControl.OpenDevice1();
        }

        private void 连接出厂显示屏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_LEDControl.OpenDevice2();
        }

        private void 临时测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str1 = IniFiles.iniFile.IniReadValue("Test", "chepai");
            string str2 = IniFiles.iniFile.IniReadValue("Test", "weight");
            // m_LEDControl.InLEDTextUpdate(str1, str2);
            m_LEDControl.TestLEDTextUpdate(str1, str2);
            //m_LEDControl.TestLEDTextUpdate2(str1, str2);

        }

        private void pictureBox_in_img_DoubleClick(object sender, EventArgs e)
        {
            FormShowImg temp = new FormShowImg();
            temp.SetImg(Manager.Instance.m_inImgPath);
          
        }

        private void pictureBox_out_img_DoubleClick(object sender, EventArgs e)
        {
            FormShowImg temp = new FormShowImg();
            temp.SetImg(Manager.Instance.m_outImgPath);
           
        }

        private void 校时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_LEDControl.AdjustTime();
        }

        private void 临时测试2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str1 = IniFiles.iniFile.IniReadValue("Test", "chepai");
            string str2 = IniFiles.iniFile.IniReadValue("Test", "weight");
            // m_LEDControl.InLEDTextUpdate(str1, str2);
            m_LEDControl.TestLEDTextUpdate2(str1, str2);
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            int count = listView1.CheckedItems.Count;
            if (count!=1)
            {
                MessageBox.Show("失败！只能删除选中的单条记录！","提示");
                return;
            }
            DialogResult re =  MessageBox.Show("确定删除本条记录吗？删除后将无法回复。", "警告", MessageBoxButtons.OKCancel);
            if (re!=DialogResult.OK)
            {
                return;
            }
            string id =  listView1.CheckedItems[0].SubItems[0].Text;
            string sqlStr = "delete from chepai where ID=" + id;
            int ret = MysqlHelp.Instance.Do(sqlStr);
            if (ret>0)
            {
                listView1.CheckedItems[0].Remove();
            }
            else
            {
                MessageBox.Show("删除失败！失败原因请查看控制台输出项。");
            }


        }

        private void 弹窗测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.Instance.ShowHideMessage("这是一条提示！", true);
        }

        private void 一键启动入厂设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideMessageForm("正在打开入厂车牌识别设备(1/4)", true);
            m_CameraLicense.OpenDevice1(pictureBox_in_video.Handle);
            Thread.Sleep(1500);
            ShowHideMessageForm("正在打开入厂地磅(2/4)", true);
            m_Loadometer.OpenDevice1();
            Thread.Sleep(1500);
            ShowHideMessageForm("正在打开红外围栏检测(3/4)", true);
            m_RedSwitch.OpenDevice();
            Thread.Sleep(1500);
            ShowHideMessageForm("正在连接入厂LED(4/4)", true);
            m_LEDControl.OpenDevice1();
            Thread.Sleep(1500);
            ShowHideMessageForm("入厂设备启动完成", true);

        }

        private void 一键启动出厂设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideMessageForm("正在打开出厂车牌识别设备(1/4)", true);
            m_CameraLicense.OpenDevice2(pictureBox_out_video.Handle);
            Thread.Sleep(1500);
            ShowHideMessageForm("正在打开出厂地磅(2/4)", true);
            m_Loadometer.OpenDevice2();
            Thread.Sleep(1500);
            ShowHideMessageForm("正在打开红外围栏检测(3/4)", true);
            m_RedSwitch.OpenDevice();
            Thread.Sleep(1500);
            ShowHideMessageForm("正在连接出厂LED(4/4)", true);
            m_LEDControl.OpenDevice2();
            Thread.Sleep(1500);
            ShowHideMessageForm("出厂设备启动完成", true);
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            测试ToolStripMenuItem.Text = "测试👌";
            new PrintPage().PrintPageTest();
        }

        private void 预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (预览ToolStripMenuItem.Text.Contains("√"))
            {
                Manager.Instance.isPreviewPrint = false;
                预览ToolStripMenuItem.Text = "预览";
            }
            else
            {
                Manager.Instance.isPreviewPrint = true;
                预览ToolStripMenuItem.Text = "预览 √";
            }
           
        }

        private void button_AddBlackList_Click(object sender, EventArgs e)
        {
            string selectChepai = textBox_chepai.Text;
            if (selectChepai.Length>5&&selectChepai.Length<10)
            {
                if (button_AddBlackList.Text.Contains("取消"))
                {
                   bool ret =  Manager.Instance.RemoveBlackList(selectChepai);
                    if (ret)
                    {
                        button_AddBlackList.Text = "拉黑";
                        MessageBox.Show("已加入白名单！");
                    }
                    else
                    {
                        MessageBox.Show("加入白名单失败！");
                    }
                }
                else
                {
                   bool ret = Manager.Instance.AddBlackList(selectChepai);
                    if (ret)
                    {
                        button_AddBlackList.Text = "取消拉黑";
                        MessageBox.Show("已加入黑名单！");
                    }
                    else
                    {
                        MessageBox.Show("加入黑名单失败！");
                    }
                }

            }
            else
            {
                MessageBox.Show("无效的车牌号！");
            }
        }

        private void textBox_chepai_TextChanged(object sender, EventArgs e)
        {
            string str = textBox_chepai.Text;
            str = Manager.Instance.m_blackList.Find((data) => data == str.Trim());
          
            if (!string.IsNullOrEmpty(str))
            {
                button_AddBlackList.Text = "取消拉黑";
            }
            else
            {
                button_AddBlackList.Text = "拉黑";
            }
        }

        private void 黑名单ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("不可用！请操作白名单！");
            //BlackListForm formBlack = new BlackListForm();
            //formBlack.Show();
        }

        private void 登录ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            form.Show();
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserForm form = new UserForm();
            form.Show();
        }

        private void 添加用户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddUserForm form = new AddUserForm();
            form.Show();
        }

        private void 白名单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WhiteListForm form = new WhiteListForm();
            form.Show();
        }
    }
}

//1.合并之后，ID不见了  2.数据库记录出错的时候不产生出厂记录了，
//程序打开时直接加载全部state=0的数据到临时列表，临时列表12小时刷新一次。
//临时列表提供删除不合理记录的操作