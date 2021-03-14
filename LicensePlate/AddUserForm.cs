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
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

     
        private void button_add_user_Click(object sender, EventArgs e)
        {
            if ((int)Manager.Instance.userInfo.level > 1)
            {
                MessageBox.Show("当前用户不是管理员！");
                return;
            }
            if (textBox_root.Text !="1" && textBox_root.Text != "2")
            {
                MessageBox.Show("权限只能输入 1(管理员) 或 2(普通用户)");
                return;
            }
            if (textBox_pwd1.Text.Length<6)
            {
                MessageBox.Show("密码长度最低6位！");
                return;
            }
            if (textBox_pwd1.Text!=textBox_pwd2.Text)
            {
                MessageBox.Show("两次输入的密码不一致！");
                return;
            }
            string sql = $"insert into account  values ('{textBox_user.Text}','{textBox_pwd1.Text}',now(),'{textBox_root.Text}')";

            int ret = MysqlHelp.Instance.Do(sql);

            if (ret == 1)
            {
                MessageBox.Show("添加用户成功！");
                this.Close();
            }
            else
            {
                MessageBox.Show("失败！用户已存在！");
            }

        }
    }
}
