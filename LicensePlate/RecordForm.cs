using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
namespace LicensePlate
{
    public partial class RecordForm : Form
    {

        float xvalues;
        float yvalues;
        string path1, path2;
        public RecordForm()
        {
            InitializeComponent();
            initListview();
        }
        void initListview()
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
            c1.Width = 80;
            c2.Width = 150;
            c3.Width = 150;
            c4.Width = 80;
            c5.Width = 80;
            c6.Width = 80;
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


            insertToListView1("1,2020-03-12 07:05:43,,+11,,,che1,,,,,");
            insertToListView1("2,234,234,,345,234,che2,,,,,,,");
            insertToListView1("3,,2020-03-12 08:05:43,,+345,,che3,,,,,,");
        }

        private void insertToListView1(string str)
        {
            string[] strs = str.Split(',');
            //创建行对象
            ListViewItem li = new ListViewItem(strs[0]);
            int id = 0;
            bool isID = int.TryParse(strs[0],out id);
            if (isID)
            {
                if (id%2==1)
                {
                    li.BackColor = Color.Cyan;
                }
                else
                {
                    li.BackColor = Color.LightYellow;
                }
            }
           

            for (int i = 1; i < strs.Length; i++)
            {
                li.SubItems.Add(strs[i]);
            }
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            //计算重量
            if (strs[3] != "" && strs[4] != "")
            {
                try
                {
                    int w1 = int.Parse(strs[3].Remove(0, 1));
                    int w2 = int.Parse(strs[4].Remove(0, 1));
                    li.SubItems[5].Text = "" + (w1 - w2);
                }
                catch (Exception e)
                {
                    Log.myLog.Error(" w1= " + strs[3] + " ; w2 = " + strs[4] + " ====>计算重量时出错：" + e.Message);

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
        }

        private void RecordForm_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(MainForm_Resize); //添加窗体拉伸重绘事件
            xvalues = this.Width;//记录窗体初始大小
            yvalues = this.Height;
            SetTag(this);

            richTextBox_report.Text = " 开始时间：xxxx\r\n  截止时间：xxxx\r\n 不限车牌号\r\n 一共处理污水nkg，运输n次，平均每次处理污水nkg，违规n次。";
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

        private void button_find_Click(object sender, EventArgs e)
        {
            //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
            DateTime t1 = dateTimePicker_begin.Value;
            DateTime t2 = dateTimePicker_end.Value;
            string cpai = textBox_chepai.Text.Trim();
            bool useCepai = checkBox_chepai.Checked;

            string commdStr="select * from chepai limit 100";

            if (useCepai)
            {
                commdStr = string.Format("select * from chepai where in_chepai='{0}' and in_time>'{1}' and in_time<'{2}' order by in_time desc limit 100", cpai, t1, t2);
            }
            else
            {
                commdStr = string.Format("select * from chepai where  in_time>'{1}' and in_time<'{2}' order by in_time desc limit 100", cpai, t1, t2);

            }

            MySqlDataReader dr = MysqlHelp.Instance.DoGetReader(commdStr);
            if (dr!=null)
            {
                listView1.Items.Clear();
                while (dr.Read())
                {
                    string id = dr["ID"].ToString();
                    string in_time = dr["in_time"].ToString();
                    string out_time = dr["out_time"].ToString();
                    string in_weight = dr["in_weight"].ToString();
                    string out_weight = dr["out_weight"].ToString();
                    string in_chepai = dr["in_chepai"].ToString();
                    string in_img = dr["in_img"].ToString();
                    string out_img = dr["out_img"].ToString();
                    string str;
                    str = string.Format("{0},{1},{2},{3},{4},,{5},{6},{7}", id, in_time, out_time, in_weight, out_weight, in_chepai,
                        in_img, out_img);
                    insertToListView1(str);
                    //dr[""]
                }
                dr.Close();
            }

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
           
            FormShowImg temp = new FormShowImg();
            if (path1 != null)
            {
                temp.SetImg(path1);
            }
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            FormShowImg temp = new FormShowImg();
            if (path2 != null)
            {
                temp.SetImg(path2);
            }
          
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
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
                        if (item.SubItems[7].Text != "")
                        {
                            if (File.Exists(item.SubItems[7].Text))
                            {
                                pictureBox1.Image = Image.FromFile(item.SubItems[7].Text);
                                path1 = item.SubItems[7].Text;
                            }
                            else
                            {
                                MessageBox.Show("不存在入厂图像：" + item.SubItems[7].Text);
                            }
                          
                           
                        }
                        if (item.SubItems[8].Text != "")
                        {
                            if (File.Exists(item.SubItems[8].Text))
                            {
                                pictureBox2.Image = Image.FromFile(item.SubItems[8].Text);
                                path2 = item.SubItems[8].Text;
                            }
                            else
                            {
                                MessageBox.Show("不存在入厂图像：" + item.SubItems[8].Text);
                            }
                                
                        }

                    }
                }
            }

        }
    }
}
