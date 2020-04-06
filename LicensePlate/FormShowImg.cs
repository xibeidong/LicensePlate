using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LicensePlate
{
    public partial class FormShowImg : Form
    {
        public FormShowImg()
        {
            InitializeComponent();
        }

        public void SetImg(string path)
        {
           
            if (File.Exists(path))
            {
                pictureBox1.Image = Image.FromFile(path);
                this.Show();
            }
            else
            {
                MessageBox.Show("无法显示不存在的图像：" + path);
                this.Close();
            }
            
        }
    }
}
