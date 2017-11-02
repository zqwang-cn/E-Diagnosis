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
        //当前模板
        private Template template;
        //用于返回的模板
        public Template selected_template;

        //主窗口新建此窗口，传入数据库
        public TemplateForm(DiagnosisContext db)
        {
            InitializeComponent();
            this.db = db;
        }

        //重新载入所有模板
        private void refresh()
        {
            IEnumerable<Template> query = from template in db.template_set
                                          where template.名称.Contains(textBox5.Text) && template.主治.Contains(textBox7.Text) && template.备注.Contains(textBox8.Text)
                                          select template;
            dataGridView1.DataSource = query.ToList();
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.ClearSelection();
            this.template = new Template();
            set_template();
        }

        //显示一个模板的所有药品
        private void set_items()
        {
            dataGridView2.DataSource = this.template.items.ToList();
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].Visible = false;
        }

        //显示一个模板
        private void set_template()
        {
            textBox1.Text = this.template.名称;
            textBox2.Text = this.template.功用;
            textBox3.Text = this.template.主治;
            textBox4.Text = this.template.备注;
            textBox6.Text = this.template.keywords;
            set_items();
        }

        //窗口载入时刷新
        private void TemplateForm_Load(object sender, EventArgs e)
        {
            refresh();
        }

        //筛选条件修改时刷新
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //筛选条件修改时刷新
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //筛选条件修改时刷新
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //新建模板(直接刷新，将所有内容置为空
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
            this.template.keywords = textBox6.Text;
            if (!((ICollection<Template>)dataGridView1.DataSource).Contains(this.template))
            {
                db.template_set.Add(this.template);
            }
            db.SaveChanges();
            refresh();
        }

        //点击一个模板时
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //如果未选中（点击行间空白有时候会出现），显示一个空模板
            if (dataGridView1.SelectedRows.Count == 0)
            {
                this.template = new Template();
            }
            //显示选中的模板内容
            else
            {
                this.template = (Template)dataGridView1.SelectedRows[0].DataBoundItem;
            }
            set_template();
        }

        //添加药品
        private void button3_Click(object sender, EventArgs e)
        {
            //打开药品编辑窗口进行选择
            MedicineForm mf = new MedicineForm(db, Category.中药);
            mf.ShowDialog();
            //如果选择了一个药品则添加到模板中
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
            //确认
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择要删除的药品！", "提示信息");
            }
            //删除并刷新
            else if(MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.template.items.Remove((TemplateItem)dataGridView2.SelectedRows[0].DataBoundItem);
                db.SaveChanges();
                set_items();
            }
        }

        //本窗口用于导入模板时，点击选择按钮
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择模板！", "提示信息");
            }
            //选中则保存并返回
            else
            {
                this.selected_template = (Template)dataGridView1.SelectedRows[0].DataBoundItem;
                this.Close();
            }
        }
    }


}
