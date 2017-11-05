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
    public partial class MedicineEditForm : Form
    {
        //要编辑的药品
        private Item item;

        //新建窗口时，将要修改的处方项传入
        public MedicineEditForm(Item item)
        {
            InitializeComponent();
            this.item = item;
        }

        //窗口初始化时，将当前内容显示
        private void MedicineEditForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = this.item.名称;
            numericUpDown1.Value = this.item.数量;
            numericUpDown2.Value = this.item.单价;
            label5.Text = this.item.小计.ToString()+"元";
        }

        //点击取消直接关闭
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //点击确定将修改写入数据库
        private void button1_Click(object sender, EventArgs e)
        {
            this.item.prescription.price -= this.item.小计;
            this.item.名称 = textBox1.Text;
            this.item.数量 = decimal.Round(numericUpDown1.Value + 0.0M, 1);
            this.item.单价 = decimal.Round(numericUpDown2.Value + 0.00M, 2);
            this.item.小计 = decimal.Round(this.item.数量 * this.item.单价 + 0.00M, 2);
            this.item.prescription.price += this.item.小计;
            this.Close();
        }
    }
}
