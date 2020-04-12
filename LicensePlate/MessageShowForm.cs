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
    public partial class MessageShowForm : Form
    {
        public MessageShowForm(string str)
        {
            InitializeComponent();
            this.ControlBox = false;   // 设置不出现关闭按钮
            label1.Text = str;
        }
        public void SetText(string message)
        {
            label1.Text = message;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
