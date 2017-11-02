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
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        //窗口初始化时
        private void SettingForm_Load(object sender, EventArgs e)
        {
            //从设置中读取当前抬头
            textBox1.Text = Properties.Settings.Default.title;
        }

        //点击确定按钮
        private void button1_Click(object sender, EventArgs e)
        {
            //将新抬头写入设置中
            Properties.Settings.Default.title = textBox1.Text;
            Properties.Settings.Default.Save();
            //关闭窗口
            this.Close();
        }

        //点击取消按钮
        private void button2_Click(object sender, EventArgs e)
        {
            //关闭窗口
            this.Close();
        }
    }
}
