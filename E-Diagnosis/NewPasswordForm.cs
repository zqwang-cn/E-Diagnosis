using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace E_Diagnosis
{
    public partial class NewPasswordForm : Form
    {
        //用于返回的密码
        public string newpassword = null;
        public NewPasswordForm()
        {
            InitializeComponent();
        }

        //点击确定按钮
        private void button1_Click(object sender, EventArgs e)
        {
            //如果再次密码一致，保存并关闭窗口
            if (textBox1.Text == textBox2.Text)
            {
                this.newpassword = textBox1.Text;
                this.Close();
            }
            //不一致则报错
            else
            {
                MessageBox.Show("再次输入不一致！", "提示信息");
            }
        }

        //点击取消直接关闭
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
