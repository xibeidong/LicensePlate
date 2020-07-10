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
namespace LicensePlate
{
    public partial class BlackListForm : Form
    {
        public BlackListForm()
        {
            InitializeComponent();
        }

        private void BlackListForm_Load(object sender, EventArgs e)
        {
            initListview();
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
            //insertToListView1("1,2020-03-12 07:05:43,,+11,,,che1,,,,,");
            //insertToListView1("2,234,234,,345,234,che2,,,,,,,");
            //insertToListView1("3,,2020-03-12 08:05:43,,+345,,che3,,,,,,");
        }
        void initListViewData()
        {
            int count = 0;
            string sql = "select * from blacklist";
            MySqlDataReader dr =  MysqlHelp.Instance.DoGetReader(sql);
            if (dr!=null)
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
           
            listView1.Items.Insert(0, li); //永远添加在第一行
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
                        if (Manager.instance.userInfo.level!=RootLevel.Highest)
                        {
                            MessageBox.Show("需要最高权限，当前用户权限不够，不能操作黑名单","警告");
                            return;
                        }
                        string chepai = item.SubItems[1].Text;
                        if (DialogResult.OK == MessageBox.Show($"确定将 \"{chepai} \" 移除黑名单？", "提示", MessageBoxButtons.OKCancel))
                        {
                            string sql = $"delete from blacklist where chepai='{chepai}'";
                            int ret = MysqlHelp.Instance.Do(sql);
                            if (ret>0)
                            {
                                Manager.instance.m_blackList.Remove(chepai);
                                MessageBox.Show($"{chepai} 成功加入白名单");
                                item.Remove();
                            }
                            else
                            {
                                MessageBox.Show("操作失败！");
                            }
                        }
                       

                    }
                }
            }
        }
    }
}
