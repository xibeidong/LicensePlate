using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicensePlate
{
    public partial class WhiteListForm : Form
    {
        public WhiteListForm()
        {
            InitializeComponent();
        }

        void initListview()
        {
            //id，车牌号,拉黑时间，操作员
            ColumnHeader c1 = new ColumnHeader();
            ColumnHeader c2 = new ColumnHeader();
            ColumnHeader c3 = new ColumnHeader();
            ColumnHeader c4 = new ColumnHeader();

            c1.Width = 50;
            c2.Width = 80;
            c3.Width = 130;
            c4.Width = 80;


            c1.Text = "ID";
            c2.Text = "车牌";
            c3.Text = "加入时间";
            c4.Text = "执行人";


            listView1.GridLines = true;  //显示网格线
            listView1.FullRowSelect = true;  //显示全行
            listView1.MultiSelect = false;  //设置只能单选
            listView1.View = View.Details;  //设置显示模式为详细
            listView1.HoverSelection = true;  //当鼠标停留数秒后自动选择

            listView1.Columns.Add(c1);
            listView1.Columns.Add(c2);
            listView1.Columns.Add(c3);
            listView1.Columns.Add(c4);


            initListViewData();
           
        }
        void initListViewData()
        {
            int count = 0;
            string sql = "select * from whitelist";
            MySqlDataReader dr = MysqlHelp.Instance.DoGetReader(sql);
            if (dr != null)
            {
                while (dr.Read())
                {
                    count++;
                    insertToListView1($"{count},{dr["chepai"].ToString()},{dr["add_time"].ToString()},{dr["do_user"].ToString()}");

                }
                dr.Close();
            }
        }
        private void insertToListView1(string str)
        {
            string[] strs = str.Split(',');
            //创建行对象
            ListViewItem li = new ListViewItem(strs[0]);
            int id = 0;
            bool isID = int.TryParse(strs[0], out id);
            if (isID)
            {
                if (id % 2 == 1)
                {
                   // li.BackColor = Color.Cyan;
                }
                else
                {
                   // li.BackColor = Color.LightYellow;
                }
            }


            for (int i = 1; i < strs.Length; i++)
            {
                li.SubItems.Add(strs[i]);
            }

            listView1.Items.Insert(0, li); //永远添加在第一行
        }

        private void button_find_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
               
                if (listView1.Items[i].SubItems[1].Text == textBox_chepai.Text)
                {
                  
                    listView1.Items[i].Selected = true;
                  
                    listView1.Items[i].EnsureVisible();
                    return;
                }
            }
            MessageBox.Show("没找到");
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            if (textBox_chepai.Text.Trim().Length<2)
            {
                MessageBox.Show("输入的车牌不规范！");
                return;
            }
            if ((int)Manager.Instance.userInfo.level > 1)
            {
                MessageBox.Show("当前用户没有管理员权限！");
                return;
            }
            string user = Manager.Instance.userInfo.username;
            string sql = $"insert into whitelist values ('{textBox_chepai.Text}',now(),'{user}')";
            int ret = MysqlHelp.Instance.Do(sql);
            if (ret == 1)
            {
                Manager.Instance.m_whiteList.Add(textBox_chepai.Text);
                insertToListView1($"{listView1.Items.Count+1},{textBox_chepai.Text},{DateTime.Now.ToString()},user");
                MessageBox.Show("添加成功");
            }
            else
            {
                MessageBox.Show("失败！请先查询白名单中是否已存在: "+textBox_chepai.Text);
            }
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            if (textBox_chepai.Text.Trim().Length < 2)
            {
                MessageBox.Show("输入的车牌不规范！");
                return;
            }
            if ((int)Manager.Instance.userInfo.level > 1)
            {
                MessageBox.Show("当前用户没有管理员权限！");
                return;
            }
            string sql = $"delete from whitelist where chepai = '{textBox_chepai.Text}'";
            int ret = MysqlHelp.Instance.Do(sql);
            if (ret == 1)
            {
                Manager.Instance.m_whiteList.Remove(textBox_chepai.Text);
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                   
                    if (listView1.Items[i].SubItems[1].Text == textBox_chepai.Text)
                    {
                        listView1.Items.RemoveAt(i);

                        break;
                    }
                }

                MessageBox.Show("已删除！");
            }
            else
            {
                MessageBox.Show("无法删除！");
            }
        }

        private void WhiteListForm_Load(object sender, EventArgs e)
        {
            initListview();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //ListViewItem item = (ListViewItem)sender;
            //string str = item.SubItems[0].Text;
            //foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
            //{
            //    Console.WriteLine(subItem.Text);
            //}
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            
            int selectCount = listView1.SelectedItems.Count; //选中的行数目，listview1是控件名。
            if (selectCount == 0) return;

            string chepai = listView1.SelectedItems[0].SubItems[1].Text;//第2列
            Console.WriteLine(chepai);
            textBox_chepai.Text = chepai;
        }
    }
}
