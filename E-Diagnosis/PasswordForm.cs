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
    public partial class PasswordForm : Form
    {
        //用于返回的密码
        public string password = null;
        public PasswordForm()
        {
            InitializeComponent();
        }

        //点击确定按钮，保存密码并关闭窗口
        private void button1_Click(object sender, EventArgs e)
        {
            this.password = textBox1.Text;
            this.Close();
        }

        //点击取消，直接关闭窗口
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
