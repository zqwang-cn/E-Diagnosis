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
        private List<Template> l;
        private Template template;

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
            l = query.ToList();
            dataGridView1.DataSource = l;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.ClearSelection();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void set_template(Template t)
        {
            this.template = t;
            textBox1.Text = t.名称;
            textBox2.Text = t.功用;
            textBox3.Text = t.主治;
            textBox4.Text = t.备注;
            dataGridView2.DataSource = t.items.ToList();
        }

        private void TemplateForm_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Template t = new Template();
            t.名称 = textBox1.Text;
            t.功用 = textBox2.Text;
            t.主治 = textBox3.Text;
            t.备注 = textBox4.Text;
            db.template_set.Add(t);
            db.SaveChanges();
            refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                db.template_set.Remove(l[dataGridView1.SelectedRows[0].Index]);
                db.SaveChanges();
                refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else
            {
                Template t = (Template)dataGridView1.SelectedRows[0].DataBoundItem;
                t.名称 = textBox1.Text;
                t.功用 = textBox2.Text;
                t.主治 = textBox3.Text;
                t.备注 = textBox4.Text;
                db.SaveChanges();
                refresh();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            Template t = (Template)dataGridView1.SelectedRows[0].DataBoundItem;
            textBox1.Text = t.名称;
            textBox2.Text = t.功用;
            textBox3.Text = t.主治;
            textBox4.Text = t.备注;
        }
    }
}
