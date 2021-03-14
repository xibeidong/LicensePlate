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
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
        }
        private void button_change_pwd_Click(object sender, EventArgs e)
        {
            if (Manager.Instance.userInfo.level != 0)
            {
                if (Manager.Instance.userInfo.username != textBox_user.Text)
                {
                    MessageBox.Show("您是普通用户，只能修改自己的密码");
                    return;
                }
            }
            if (textBox_new_pwd1.Text.Length<6)
            {
                MessageBox.Show("密码不能小于6位");
                return;
            }
            if (textBox_new_pwd1.Text != textBox_new_pwd2.Text)
            {
                MessageBox.Show("两次输入的密码不一致！");
                return;
            }

            if (Manager.Instance.userInfo.password != textBox_old_pwd.Text)
            {
                MessageBox.Show("旧密码不对！");
                return;
            }

            string sql = $"update account set password = '{textBox_new_pwd2.Text}' where username = '{textBox_user.Text}'";
            int ret = MysqlHelp.Instance.Do(sql);
            if (ret == 1)
            {
                Manager.Instance.userInfo.password = textBox_new_pwd1.Text;
                MessageBox.Show("密码修改成功");
                this.Close();
            }


        }
    }
}
