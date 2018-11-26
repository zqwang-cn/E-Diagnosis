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
        //数据库
        private DiagnosisContext db;
        //当前选中的模板
        private Template template;

        //主窗口新建此窗口，传入数据库
        public TemplateForm(DiagnosisContext db)
        {
            InitializeComponent();
            this.db = db;
        }

        //-------------------模板操作-------------------------
        //刷新模板列表
        private void refresh()
        {
            //根据搜索条件查找所有模板并显示
            IEnumerable<Template> query = from template in db.template_set
                                          where template.名称.Contains(textBox5.Text) && template.主治.Contains(textBox7.Text) && template.备注.Contains(textBox8.Text)
                                          select template;
            templateBindingSource.DataSource = query.ToList();
            dataGridView1.ClearSelection();

            //将目前选中的模板选中
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                Template t = (Template)row.DataBoundItem;
                if (t == this.template)
                {
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
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

        //新建模板(添加一个内容为空的模板以供后续修改）
        private void button6_Click(object sender, EventArgs e)
        {
            Template t = new Template();
            db.template_set.Add(t);
            db.SaveChanges();
            set_template(t);
            refresh();
        }

        //显示一个模板
        private void set_template(Template t)
        {
            this.template = t;
            templateBindingSource1.DataSource = t;
            set_items();
        }
        //显示模板内容
        private void set_items()
        {
            templateItemBindingSource.DataSource = null;
            templateItemBindingSource.DataSource = this.template.items;
        }

        //删除模板与显示模板内容
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.RowCount && e.ColumnIndex >= 0)
            {
                //删除模板
                if (dataGridView1.Columns[e.ColumnIndex].Name == "delete" && MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Template t = (Template)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                    db.template_set.Remove(t);
                    db.SaveChanges();
                    refresh();
                }
                //点击一个模板时，显示该模板的内容
                else
                {
                    Template t = (Template)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                    set_template(t);
                }
            }
        }

        //-------------------------药品操作-----------------------
        //删除药品
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView2.RowCount && e.ColumnIndex >= 0 && dataGridView2.Columns[e.ColumnIndex].Name == "titemdelete")
            {
                if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    TemplateItem item = (TemplateItem)dataGridView2.Rows[e.RowIndex].DataBoundItem;
                    templateItemBindingSource.Remove(item);
                    //item.template.items.Remove(item);
                    //db.SaveChanges();
                    //set_items();
                }
            }
        }
        
        //修改简拼，显示候选药品
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (textBox9.Text == "")
            {
                medicineBindingSource.DataSource = null;
            }
            else
            {
                medicineBindingSource.DataSource = db.medicine_set.Where(m => m.category==Category.中药 && (m.简拼.Contains(textBox9.Text)||m.名称.Contains(textBox9.Text))).ToList();
            }
        }
        //添加药品至当前模板
        private void add_item(Medicine m)
        {
            TemplateItem item = new TemplateItem();
            item.药品 = m.名称;
            item.数量 = 1;
            item.medicine = m;
            templateItemBindingSource.Add(item);
            //this.template.items.Add(item);
            //db.SaveChanges();
            //set_items();
        }
        //回车添加药品
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.template != null && dataGridView3.RowCount > 0)
            {
                add_item((Medicine)dataGridView3.Rows[0].DataBoundItem);
                MessageBox.Show("添加成功！", "提示信息");
            }
        }
        //双击添加药品
        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.template != null)
            {
                add_item((Medicine)dataGridView3.Rows[e.RowIndex].DataBoundItem);
                MessageBox.Show("添加成功！", "提示信息");
            }
        }

        //关闭窗口自动保存
        private void TemplateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            textBox1.Focus();
            db.SaveChanges();
        }
    }


}
