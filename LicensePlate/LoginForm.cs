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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
           
        }

        private void button1_login(object sender, EventArgs e)
        {
            string user = textBox_user.Text;
            string pwd = textBox_pwd.Text;
            string sql = $"select * from account where username='{user}' and password='{pwd}'";
            MySqlDataReader dr = MysqlHelp.Instance.DoGetReader(sql);
            if (dr!=null)
            {

                while (dr.Read())
                {
                    Manager.Instance.userInfo.username = user;
                    Manager.Instance.userInfo.isLogin = true;
                    Manager.Instance.userInfo.password = pwd;
                    Manager.Instance.userInfo.level = (LicensePlate.RootLevel)dr["level"];
                 
                }
                dr.Close();
               
            }
            if (Manager.Instance.userInfo.isLogin)
            {
                Manager.Instance.LoginSuccess();
                MessageBox.Show("登陆成功！");
                this.Close();
            }
            else
            {
                MessageBox.Show("登陆失败！用户名或密码错误！");
            }
          

        }
    }
}
