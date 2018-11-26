using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace E_Diagnosis
{
    public partial class MedicineForm : Form
    {
        //数据库
        private DiagnosisContext db;
        //药品列表
        private List<Medicine> l;

        //主窗口新建本窗口，并传入数据库与要查看的药品类型（也可以为空）
        public MedicineForm(DiagnosisContext db, Category c)
        {
            InitializeComponent();
            this.db = db;
            var clist = db.medicine_set.Where(m => m.category == Category.中药);
            var wlist = db.medicine_set.Where(m => m.category == Category.中成药);
            comboBox1.DataSource = Enum.GetValues(typeof(Category));
            comboBox1.SelectedItem = c;
        }

        //刷新窗口，重置所有药品
        private void refresh()
        {
            //根据搜索条件查找药品并显示
            IEnumerable<Medicine> query = from medicine in db.medicine_set
                                          where (medicine.名称.Contains(textBox1.Text)||medicine.简拼.Contains(textBox1.Text)) && medicine.category == (Category)comboBox1.SelectedItem
                                          select medicine;
            l = query.ToList();
            if (dataGridView1.Columns["delete"] != null)
            {
                dataGridView1.Columns.Remove("delete");
            }
            dataGridView1.DataSource = l;
            DataGridViewButtonColumn bc = new DataGridViewButtonColumn();
            bc.Text = "删除";
            bc.Name = "delete";
            bc.HeaderText = "";
            bc.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(bc);
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.ClearSelection();
            //textBox2.Text = "";
            //textBox3.Text = "";
            //numericUpDown1.Value = 0.10M;
        }

        //窗口载入时刷新窗口
        private void MedicineForm_Load(object sender, EventArgs e)
        {
            refresh();
        }

        //选择类型时刷新窗口
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //筛选条件改变时刷新窗口
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //添加药品
        private void button1_Click(object sender, EventArgs e)
        {
            //if (textBox2.Text.Trim() == "")
            //{
            //    MessageBox.Show("品名不能为空！", "提示信息");
            //}
            //else if (textBox3.Text.Trim() == "")
            //{
            //    MessageBox.Show("规格不能为空！", "提示信息");
            //}
            //else
            //{
            //    Medicine medicine = new Medicine();
            //    medicine.category = (Category)comboBox1.SelectedItem;
            //    medicine.名称 = textBox2.Text;
            //    medicine.规格 = textBox3.Text;
            //    medicine.价格 = decimal.Round(numericUpDown1.Value + 0.00M, 2);
            //    db.medicine_set.Add(medicine);
            //    db.SaveChanges();
            //    refresh();
            //}
            Medicine medicine = new Medicine();
            medicine.category = (Category)comboBox1.SelectedItem;
            db.medicine_set.Add(medicine);
            db.SaveChanges();
            textBox1.Text = "";
            refresh();
            dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true;
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        //修改内容之后自动保存
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            db.SaveChanges();
        }

        //点击删除按钮
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "delete")
            {
                if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Medicine m = (Medicine)dataGridView1.CurrentRow.DataBoundItem;
                    db.medicine_set.Remove(m);
                    db.SaveChanges();
                    refresh();
                }
            }
        }

        private void MedicineForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            textBox1.Focus();
        }
    }
}
