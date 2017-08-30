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
    public partial class MedicineForm : Form
    {
        private DiagnosisContext db;
        private List<Medicine> l;
        private int selected;
        public Medicine result_medicine = null;
        public decimal result_amount = 0;

        public MedicineForm(DiagnosisContext db, Category c)
        {
            InitializeComponent();
            this.db = db;
            comboBox1.DataSource = Enum.GetValues(typeof(Category));
            comboBox1.SelectedItem = c;
        }

        private void refresh()
        {
            IEnumerable<Medicine> query = from medicine in db.medicine_set
                                          where medicine.名称.Contains(textBox1.Text) && medicine.category == (Category)comboBox1.SelectedItem
                                          select medicine;
            l = query.ToList();
            dataGridView1.DataSource = l;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.ClearSelection();
            selected = -1;
            textBox2.Text = "";
            textBox3.Text = "";
            numericUpDown1.Value = 0.10M;
        }

        private void MedicineForm_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selected = e.RowIndex;
            if (selected < 0 || selected > l.Count)
            {
                return;
            }
            textBox2.Text = l[selected].名称;
            textBox3.Text = l[selected].规格;
            numericUpDown1.Value = l[selected].价格;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("品名不能为空！", "提示信息");
            }
            else if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("规格不能为空！", "提示信息");
            }
            else
            {
                Medicine medicine = new Medicine();
                medicine.category = (Category)comboBox1.SelectedItem;
                medicine.名称 = textBox2.Text;
                medicine.规格 = textBox3.Text;
                medicine.价格 = decimal.Round(numericUpDown1.Value + 0.00M, 2);
                db.medicine_set.Add(medicine);
                db.SaveChanges();
                refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selected == -1)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("品名不能为空！", "提示信息");
            }
            else if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("规格不能为空！", "提示信息");
            }
            else
            {
                Medicine medicine = l[selected];
                medicine.名称 = textBox2.Text;
                medicine.规格 = textBox3.Text;
                medicine.价格 = decimal.Round(numericUpDown1.Value + 0.00M, 2);
                db.SaveChanges();
                refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selected == -1)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                db.medicine_set.Remove(l[selected]);
                db.SaveChanges();
                refresh();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selected == -1)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else
            {
                this.result_medicine = l[selected];
                this.result_amount = decimal.Round(numericUpDown2.Value + 0.0M, 1);
                this.Close();
            }
        }
    }
}
