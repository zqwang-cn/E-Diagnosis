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
        //用于返回的模板
        public Template selected_template;
        //所有中药名
        AutoCompleteStringCollection caccs;

        //主窗口新建此窗口，传入数据库
        public TemplateForm(DiagnosisContext db)
        {
            InitializeComponent();
            this.db = db;
            //初始化自动补全内容
            IEnumerable<String> query = from medicine in db.medicine_set
                                        where medicine.category == Category.中药
                                        select medicine.名称;
            this.caccs = new AutoCompleteStringCollection();
            this.caccs.AddRange(query.ToArray());
        }

        //重新载入所有模板
        private void refresh()
        {
            Template lastt = this.template;
            //根据搜索条件查找所有模板并显示
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

            bool found = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                Template t = (Template)row.DataBoundItem;
                if (t == lastt)
                {
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                this.template = new Template();
                set_template();
            }
        }

        //显示一个模板的所有药品
        private void set_items()
        {
            if (dataGridView2.Columns["delete"] != null)
            {
                dataGridView2.Columns.Remove("delete");
            }
            //Medicine m = new Medicine();
            //m.名称= "[新增]";
            TemplateItem i = new TemplateItem();
            i.药品 = "[新增]";
            i.数量 = 1;
            i.medicine = null;
            List<TemplateItem> l = this.template.items.ToList();
            l.Add(i);
            dataGridView2.DataSource = l;
            DataGridViewButtonColumn bc = new DataGridViewButtonColumn();
            bc.Text = "删除";
            bc.Name = "delete";
            bc.HeaderText = "";
            bc.UseColumnTextForButtonValue = true;
            dataGridView2.Columns.Add(bc);
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].Visible = false;
            dataGridView2.Columns[4].Visible = false;
            //dataGridView2.Columns[2].ReadOnly = true;
            dataGridView2.ClearSelection();
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

        //新建模板(添加一个内容为空的模板以供后续修改）
        private void button6_Click(object sender, EventArgs e)
        {
            Template t = new Template();
            t.名称 = "新模板";
            db.template_set.Add(t);
            db.SaveChanges();
            this.template = t;
            refresh();
        }

        //删除模板（删除当前选中的模板）
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择模板！", "提示信息");
            }
            else if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                db.template_set.Remove((Template)dataGridView1.SelectedRows[0].DataBoundItem);
                db.SaveChanges();
                this.template = new Template();
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
            MessageBox.Show("保存成功！", "提示信息");
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

        ////添加药品
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    ////打开药品编辑窗口进行选择
        //    //MedicineForm mf = new MedicineForm(db, Category.中药);
        //    //mf.ShowDialog();
        //    ////如果选择了一个药品则添加到模板中
        //    //if (mf.result_medicine != null)
        //    //{
        //    //    TemplateItem item = new TemplateItem();
        //    //    item.药品 = mf.result_medicine;
        //    //    item.数量 = mf.result_amount;
        //    //    this.template.items.Add(item);
        //    //    db.SaveChanges();
        //    //    set_items();
        //    //}

        //    //根据筛选药品名称查找药品
        //    IEnumerable<Medicine> query = from medicine in db.medicine_set
        //                                  where medicine.名称.Equals(textBox27.Text)
        //                                  select medicine;
        //    Medicine[] medicines = query.ToArray();
        //    if (medicines.Length == 0)
        //    {
        //        MessageBox.Show("药品名称错误！", "提示信息");
        //        return;
        //    }
        //    Medicine m = medicines[0];
        //    //添加至当前模板
        //    TemplateItem item = new TemplateItem();
        //    item.药品 = m.名称;
        //    item.数量 = 1;
        //    item.medicine = m;
        //    this.template.items.Add(item);
        //    //db.SaveChanges();
        //    set_items();
        //}

        ////删除药品
        //private void button4_Click(object sender, EventArgs e)
        //{
        //    //确认
        //    if (dataGridView2.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("请选择要删除的药品！", "提示信息");
        //    }
        //    //删除并刷新
        //    else if(MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //    {
        //        this.template.items.Remove((TemplateItem)dataGridView2.SelectedRows[0].DataBoundItem);
        //        //db.SaveChanges();
        //        set_items();
        //    }
        //}

        //删除药品
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dataGridView2.Columns[e.ColumnIndex].Name == "delete")
            {
                if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    TemplateItem item = (TemplateItem)dataGridView2.Rows[e.RowIndex].DataBoundItem;
                    item.template.items.Remove(item);
                    set_items();
                }

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

        //增加药品时添加自动补全内容
        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell.ColumnIndex == 2)
            {
                if (dgv.CurrentCell.RowIndex == dgv.RowCount - 1)
                {
                    tb.Text = "";
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    tb.AutoCompleteCustomSource = this.caccs;
                }
                else
                {
                    tb.AutoCompleteMode = AutoCompleteMode.None;
                }
            }
            else
            {
                tb.AutoCompleteMode = AutoCompleteMode.None;
            }
        }
        
        //添加药品
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            //如果是新增药品则添加至模板
            if (e.RowIndex == dgv.RowCount - 1)
            {
                if (e.ColumnIndex == 2)
                {
                    String s = (String)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    IEnumerable<Medicine> query = from medicine in db.medicine_set
                                                  where medicine.名称.Equals(s) && medicine.category == Category.中药
                                                  select medicine;
                    Medicine[] medicines = query.ToArray();
                    if (medicines.Length == 0)
                    {
                        MessageBox.Show("药品名称错误！", "提示信息");
                        return;
                    }
                    Medicine m = medicines[0];
                    //添加至当前模板
                    TemplateItem item = new TemplateItem();
                    item.药品 = m.名称;
                    item.数量 = 1;
                    item.medicine = m;
                    item.规格 = m.规格;
                    this.template.items.Add(item);
                    db.SaveChanges();
                    set_items();
                    MessageBox.Show("添加成功！", "提示信息");
                }
            }
        }
    }


}
