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
    public partial class TemplateForm : Form
    {
        private DiagnosisContext db;
        private Template template;
        public Template selected_template;

        public TemplateForm(DiagnosisContext db)
        {
            InitializeComponent();
            this.db = db;
        }

        private void refresh()
        {
            IEnumerable<Template> query = from template in db.template_set
                        where template.名称.Contains(textBox5.Text)
                        select template;
            dataGridView1.DataSource = query.ToList();
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.ClearSelection();
            this.template = new Template();
            set_template();
        }

        private void set_items()
        {
            dataGridView2.DataSource = this.template.items.ToList();
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].Visible = false;
        }

        private void set_template()
        {
            textBox1.Text = this.template.名称;
            textBox2.Text = this.template.功用;
            textBox3.Text = this.template.主治;
            textBox4.Text = this.template.备注;
            set_items();
        }

        private void TemplateForm_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //新建模板
        private void button6_Click(object sender, EventArgs e)
        {
            refresh();
        }

        //删除模板
        private void button2_Click(object sender, EventArgs e)
        {
            if (!((ICollection<Template>)dataGridView1.DataSource).Contains(this.template))
            {
                MessageBox.Show("请先选择模板！", "提示信息");
            }
            else if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                db.template_set.Remove(this.template);
                db.SaveChanges();
                refresh();
            }
        }

        //保存模板
        private void button1_Click(object sender, EventArgs e)
        {
            this.template.名称 = textBox1.Text;
            this.template.功用 = textBox2.Text;
            this.template.主治 = textBox3.Text;
            this.template.备注 = textBox4.Text;
            if (!((ICollection<Template>)dataGridView1.DataSource).Contains(this.template))
            {
                db.template_set.Add(this.template);
            }
            db.SaveChanges();
            refresh();
        }

        //选择模板
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                this.template = new Template();
            }
            else
            {
                this.template = (Template)dataGridView1.SelectedRows[0].DataBoundItem;
            }
            set_template();
        }

        //添加药品
        private void button3_Click(object sender, EventArgs e)
        {
            MedicineForm mf = new MedicineForm(db, Category.中药);
            mf.ShowDialog();
            if (mf.result_medicine != null)
            {
                TemplateItem item = new TemplateItem();
                item.药品 = mf.result_medicine;
                item.数量 = mf.result_amount;
                this.template.items.Add(item);
                db.SaveChanges();
                set_items();
            }
        }

        //删除药品
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择要删除的药品！", "提示信息");
            }
            else if(MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.template.items.Remove((TemplateItem)dataGridView2.SelectedRows[0].DataBoundItem);
                db.SaveChanges();
                set_items();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择模板！", "提示信息");
            }
            else
            {
                this.selected_template = (Template)dataGridView1.SelectedRows[0].DataBoundItem;
                this.Close();
            }
        }
    }


}
