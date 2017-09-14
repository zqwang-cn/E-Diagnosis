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
        private Item item;
        public MedicineEditForm(Item item)
        {
            InitializeComponent();
            this.item = item;
        }

        private void MedicineEditForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = this.item.名称;
            numericUpDown1.Value = this.item.数量;
            numericUpDown2.Value = this.item.单价;
            label5.Text = this.item.小计.ToString()+"元";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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
