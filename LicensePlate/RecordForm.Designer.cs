namespace LicensePlate
{
    partial class RecordForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_chepai = new System.Windows.Forms.CheckBox();
            this.button_report = new System.Windows.Forms.Button();
            this.button_find = new System.Windows.Forms.Button();
            this.textBox_chepai = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker_end = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_begin = new System.Windows.Forms.DateTimePicker();
            this.label_first_page = new System.Windows.Forms.Label();
            this.label_last_page = new System.Windows.Forms.Label();
            this.label_next_page = new System.Windows.Forms.Label();
            this.label_end_page = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox_report = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(2, 1);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(842, 728);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_chepai);
            this.groupBox1.Controls.Add(this.button_report);
            this.groupBox1.Controls.Add(this.button_find);
            this.groupBox1.Controls.Add(this.textBox_chepai);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dateTimePicker_end);
            this.groupBox1.Controls.Add(this.dateTimePicker_begin);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 250);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询条件";
            // 
            // checkBox_chepai
            // 
            this.checkBox_chepai.AutoSize = true;
            this.checkBox_chepai.Location = new System.Drawing.Point(21, 108);
            this.checkBox_chepai.Name = "checkBox_chepai";
            this.checkBox_chepai.Size = new System.Drawing.Size(84, 16);
            this.checkBox_chepai.TabIndex = 4;
            this.checkBox_chepai.Text = "指定车牌号";
            this.checkBox_chepai.UseVisualStyleBackColor = true;
            // 
            // button_report
            // 
            this.button_report.Location = new System.Drawing.Point(163, 161);
            this.button_report.Name = "button_report";
            this.button_report.Size = new System.Drawing.Size(92, 26);
            this.button_report.TabIndex = 3;
            this.button_report.Text = "输出报告";
            this.button_report.UseVisualStyleBackColor = true;
            // 
            // button_find
            // 
            this.button_find.Location = new System.Drawing.Point(21, 161);
            this.button_find.Name = "button_find";
            this.button_find.Size = new System.Drawing.Size(97, 26);
            this.button_find.TabIndex = 3;
            this.button_find.Text = "开始查询";
            this.button_find.UseVisualStyleBackColor = true;
            this.button_find.Click += new System.EventHandler(this.button_find_Click);
            // 
            // textBox_chepai
            // 
            this.textBox_chepai.Location = new System.Drawing.Point(111, 106);
            this.textBox_chepai.Name = "textBox_chepai";
            this.textBox_chepai.Size = new System.Drawing.Size(170, 21);
            this.textBox_chepai.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 12);
            this.label3.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "截止时间";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "开始时间";
            // 
            // dateTimePicker_end
            // 
            this.dateTimePicker_end.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePicker_end.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_end.Location = new System.Drawing.Point(81, 58);
            this.dateTimePicker_end.Name = "dateTimePicker_end";
            this.dateTimePicker_end.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker_end.TabIndex = 0;
            // 
            // dateTimePicker_begin
            // 
            this.dateTimePicker_begin.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePicker_begin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_begin.Location = new System.Drawing.Point(81, 20);
            this.dateTimePicker_begin.Name = "dateTimePicker_begin";
            this.dateTimePicker_begin.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker_begin.TabIndex = 0;
            // 
            // label_first_page
            // 
            this.label_first_page.AutoSize = true;
            this.label_first_page.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_first_page.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label_first_page.Location = new System.Drawing.Point(220, 736);
            this.label_first_page.Name = "label_first_page";
            this.label_first_page.Size = new System.Drawing.Size(40, 16);
            this.label_first_page.TabIndex = 2;
            this.label_first_page.Text = "首页";
            // 
            // label_last_page
            // 
            this.label_last_page.AutoSize = true;
            this.label_last_page.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_last_page.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label_last_page.Location = new System.Drawing.Point(293, 736);
            this.label_last_page.Name = "label_last_page";
            this.label_last_page.Size = new System.Drawing.Size(56, 16);
            this.label_last_page.TabIndex = 2;
            this.label_last_page.Text = "上一页";
            // 
            // label_next_page
            // 
            this.label_next_page.AutoSize = true;
            this.label_next_page.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_next_page.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label_next_page.Location = new System.Drawing.Point(377, 736);
            this.label_next_page.Name = "label_next_page";
            this.label_next_page.Size = new System.Drawing.Size(56, 16);
            this.label_next_page.TabIndex = 2;
            this.label_next_page.Text = "下一页";
            // 
            // label_end_page
            // 
            this.label_end_page.AutoSize = true;
            this.label_end_page.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_end_page.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label_end_page.Location = new System.Drawing.Point(462, 736);
            this.label_end_page.Name = "label_end_page";
            this.label_end_page.Size = new System.Drawing.Size(40, 16);
            this.label_end_page.TabIndex = 2;
            this.label_end_page.Text = "尾页";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox_report);
            this.groupBox2.Location = new System.Drawing.Point(6, 309);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 357);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "报告";
            // 
            // richTextBox_report
            // 
            this.richTextBox_report.Location = new System.Drawing.Point(6, 20);
            this.richTextBox_report.Name = "richTextBox_report";
            this.richTextBox_report.Size = new System.Drawing.Size(287, 331);
            this.richTextBox_report.TabIndex = 0;
            this.richTextBox_report.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(317, 331);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "入厂截图";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(6, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(306, 276);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.pictureBox2);
            this.groupBox4.Location = new System.Drawing.Point(6, 343);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(312, 329);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "出厂截图";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox2.Location = new System.Drawing.Point(6, 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(306, 281);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.DoubleClick += new System.EventHandler(this.pictureBox2_DoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(850, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(334, 717);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(326, 691);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "查询";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(326, 691);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "图像";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label_end_page);
            this.Controls.Add(this.label_next_page);
            this.Controls.Add(this.label_last_page);
            this.Controls.Add(this.label_first_page);
            this.Controls.Add(this.listView1);
            this.Name = "RecordForm";
            this.Text = "记录";
            this.Load += new System.EventHandler(this.RecordForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_chepai;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker_end;
        private System.Windows.Forms.DateTimePicker dateTimePicker_begin;
        private System.Windows.Forms.Button button_find;
        private System.Windows.Forms.CheckBox checkBox_chepai;
        private System.Windows.Forms.Button button_report;
        private System.Windows.Forms.Label label_first_page;
        private System.Windows.Forms.Label label_last_page;
        private System.Windows.Forms.Label label_next_page;
        private System.Windows.Forms.Label label_end_page;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBox_report;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}