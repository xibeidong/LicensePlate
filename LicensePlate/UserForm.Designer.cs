namespace LicensePlate
{
    partial class UserForm
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
            this.textBox_user = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_old_pwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_new_pwd1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_new_pwd2 = new System.Windows.Forms.TextBox();
            this.button_change_pwd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_user
            // 
            this.textBox_user.Location = new System.Drawing.Point(172, 85);
            this.textBox_user.Name = "textBox_user";
            this.textBox_user.Size = new System.Drawing.Size(226, 25);
            this.textBox_user.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "旧密码：";
            // 
            // textBox_old_pwd
            // 
            this.textBox_old_pwd.Location = new System.Drawing.Point(172, 157);
            this.textBox_old_pwd.Name = "textBox_old_pwd";
            this.textBox_old_pwd.PasswordChar = '*';
            this.textBox_old_pwd.Size = new System.Drawing.Size(226, 25);
            this.textBox_old_pwd.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 221);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "新密码：";
            // 
            // textBox_new_pwd1
            // 
            this.textBox_new_pwd1.Location = new System.Drawing.Point(172, 218);
            this.textBox_new_pwd1.Name = "textBox_new_pwd1";
            this.textBox_new_pwd1.PasswordChar = '*';
            this.textBox_new_pwd1.Size = new System.Drawing.Size(226, 25);
            this.textBox_new_pwd1.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 289);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "重复新密码：";
            // 
            // textBox_new_pwd2
            // 
            this.textBox_new_pwd2.Location = new System.Drawing.Point(172, 286);
            this.textBox_new_pwd2.Name = "textBox_new_pwd2";
            this.textBox_new_pwd2.PasswordChar = '*';
            this.textBox_new_pwd2.Size = new System.Drawing.Size(226, 25);
            this.textBox_new_pwd2.TabIndex = 6;
            // 
            // button_change_pwd
            // 
            this.button_change_pwd.Location = new System.Drawing.Point(172, 369);
            this.button_change_pwd.Name = "button_change_pwd";
            this.button_change_pwd.Size = new System.Drawing.Size(226, 35);
            this.button_change_pwd.TabIndex = 8;
            this.button_change_pwd.Text = "修改";
            this.button_change_pwd.UseVisualStyleBackColor = true;
            this.button_change_pwd.Click += new System.EventHandler(this.button_change_pwd_Click);
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 450);
            this.Controls.Add(this.button_change_pwd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_new_pwd2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_new_pwd1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_old_pwd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_user);
            this.Name = "UserForm";
            this.Text = "修改密码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_user;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_old_pwd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_new_pwd1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_new_pwd2;
        private System.Windows.Forms.Button button_change_pwd;
    }
}