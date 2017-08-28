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
    public partial class MainForm : Form
    {
        DiagnosisContext db;
        private List<Patient> l;
        private Record record;
        private Patient patient;

        public MainForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth;
        }

        private void set_patient_editable(bool editable)
        {
            comboBox3.Enabled = editable;
            textBox5.Enabled = editable;
            textBox3.Enabled = editable;
            comboBox1.Enabled = editable;
            numericUpDown1.Enabled = editable;
            numericUpDown2.Enabled = editable;
            comboBox4.Enabled = editable;
            comboBox5.Enabled = editable;
            textBox11.Enabled = editable;
            textBox12.Enabled = editable;
            comboBox6.Enabled = editable;
            textBox13.Enabled = editable;
            textBox14.Enabled = editable;
            textBox4.Enabled = editable;
            textBox15.Enabled = editable;
        }

        private void set_record_editable(bool editable)
        {
            comboBox2.Enabled = editable;
            comboBox7.Enabled = editable;
            dateTimePicker1.Enabled = editable;
            textBox6.Enabled = editable;
            textBox7.Enabled = editable;
            textBox8.Enabled = editable;
            textBox21.Enabled = editable;
            textBox9.Enabled = editable;
            textBox10.Enabled = editable;
            textBox16.Enabled = editable;
            textBox17.Enabled = editable;
            textBox18.Enabled = editable;
            textBox19.Enabled = editable;
            textBox20.Enabled = editable;
        }

        private void set_prescription_editable(bool editable)
        {
            button3.Enabled = editable;
            button4.Enabled = editable;
            button5.Enabled = editable;
            button6.Enabled = editable;
            numericUpDown3.Enabled = editable;
            textBox22.Enabled = editable;
            textBox23.Enabled = editable;
            textBox24.Enabled = editable;
            textBox25.Enabled = editable;
            textBox26.Enabled = editable;
            button7.Enabled = editable;
            button12.Enabled = !editable;
        }

        //刷新用户列表
        private void refresh()
        {
            IEnumerable<Patient> query = from patient in db.patient_set
                                         where patient.编号.Contains(textBox1.Text) && patient.姓名.Contains(textBox2.Text)
                                         select patient;
            l = query.ToList();
            dataGridView1.DataSource = l;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[17].Visible = false;
            dataGridView1.ClearSelection();
            if (this.patient != null)
            {
                
                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    Patient p = (Patient)row.DataBoundItem;
                    if (p == this.patient)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            tabControl1.SelectTab(2);
            tabControl1.SelectTab(3);
            tabControl1.SelectTab(0);
            db = new DiagnosisContext();
            refresh();
            this.reportViewer1.RefreshReport();
        }

        private void set_prescriptions()
        {
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.RefreshReport();
            if (this.record != null)
            {
                //如果处方未保存，则可以编辑，否则不可编辑
                if (this.record.finished)
                {
                    set_prescription_editable(false);
                }
                else
                {
                    set_prescription_editable(true);
                }

                //wprescription
                label46.Text = this.record.wprescription.price.ToString() + "元";
                dataGridView3.DataSource = this.record.wprescription.items.ToList();
                dataGridView3.ClearSelection();
                dataGridView3.Columns[0].Visible = false;
                dataGridView3.Columns[1].Visible = false;

                //cprescription
                numericUpDown3.Value = this.record.cprescription.amount;
                label47.Text = (this.record.cprescription.price * this.record.cprescription.amount).ToString() + "元";
                dataGridView4.DataSource = this.record.cprescription.items.ToList();
                dataGridView4.ClearSelection();
                dataGridView4.Columns[0].Visible = false;
                dataGridView4.Columns[1].Visible = false;

                label48.Text = (this.record.wprescription.price + this.record.cprescription.price * this.record.cprescription.amount).ToString() + "元";
                textBox22.Text = this.record.医师;
                textBox23.Text = this.record.调配;
                textBox24.Text = this.record.审核;
                textBox25.Text = this.record.核对;
                textBox26.Text = this.record.发药;
            }
            else
            {
                dataGridView3.DataSource = null;
                dataGridView4.DataSource = null;
                label46.Text = "0元";
                label47.Text = "0元";
                label48.Text = "0元";
                numericUpDown3.Value = 1;
                textBox22.Text = "";
                textBox23.Text = "";
                textBox24.Text = "";
                textBox25.Text = "";
                textBox26.Text = "";
                set_prescription_editable(false);
                button12.Enabled = false;
            }
        }

        private void set_record(Record r)
        {
            this.record = r;
            if (r == null)
            {
                comboBox2.SelectedIndex = 0;
                comboBox7.SelectedIndex = 0;
                dateTimePicker1.Value = DateTime.Now.Date;
                textBox6.Text = "";
                textBox7.Text = "";
                textBox8.Text = "";
                textBox21.Text = "";
                textBox9.Text = "";
                textBox10.Text = "";
                textBox16.Text = "";
                textBox17.Text = "";
                textBox18.Text = "";
                textBox19.Text = "";
                textBox20.Text = "";
                set_record_editable(true);
                button11.Enabled = false;
                button2.Enabled = true;
            }
            else
            {
                comboBox2.SelectedItem = r.类型;
                comboBox7.SelectedItem = r.科别;
                dateTimePicker1.Value = r.就诊日期;
                textBox6.Text = r.主诉;
                textBox7.Text = r.现病史;
                textBox8.Text = r.既往史;
                textBox21.Text = r.个人史;
                textBox9.Text = r.家族史;
                textBox10.Text = r.月经及婚育史;
                textBox16.Text = r.体格检查;
                textBox17.Text = r.辅助检查;
                textBox18.Text = r.临床诊断;
                textBox19.Text = r.治疗意见;
                textBox20.Text = r.说明;
                set_record_editable(false);
                button11.Enabled = true;
                button2.Enabled = false;
            }
            set_prescriptions();
        }

        private void set_records()
        {
            if (this.patient == null)
            {
                dataGridView2.DataSource = new List<Record>();
                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[1].Visible = false;
                dataGridView2.Columns[2].Visible = false;
                dataGridView2.Columns[5].DefaultCellStyle.Format = "yyyy/MM/dd";
                dataGridView2.Columns[16].Visible = false;
                dataGridView2.Columns[17].Visible = false;
                dataGridView2.Columns[18].Visible = false;
                dataGridView2.Columns[19].Visible = false;
                dataGridView2.Columns[20].Visible = false;
                dataGridView2.Columns[21].Visible = false;
                dataGridView2.Columns[22].Visible = false;
                dataGridView2.Columns[23].Visible = false;
                set_record(null);
            }
            else
            {
                dataGridView2.DataSource = this.patient.records.ToList();
                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[1].Visible = false;
                dataGridView2.Columns[2].Visible = false;
                dataGridView2.Columns[5].DefaultCellStyle.Format = "yyyy/MM/dd";
                dataGridView2.Columns[16].Visible = false;
                dataGridView2.Columns[17].Visible = false;
                dataGridView2.Columns[18].Visible = false;
                dataGridView2.Columns[19].Visible = false;
                dataGridView2.Columns[20].Visible = false;
                dataGridView2.Columns[21].Visible = false;
                dataGridView2.Columns[22].Visible = false;
                dataGridView2.Columns[23].Visible = false;
                if (dataGridView2.Rows.Count > 0)
                {
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Selected = true;
                    set_record(((List<Record>)this.patient.records)[dataGridView2.Rows.Count - 1]);
                }
                else
                {
                    set_record(null);
                }
            }
        }

        //设置当前选中的病人（为null则清空）
        private void set_patient(Patient p)
        {
            this.patient = p;
            if (p == null)
            {
                label29.Text = "";
                comboBox3.SelectedIndex = 0;
                textBox5.Text = "";
                textBox3.Text = "";
                comboBox1.SelectedIndex = 0;
                numericUpDown1.Value = 0;
                numericUpDown2.Value = 0;
                comboBox4.SelectedIndex = 0;
                comboBox5.SelectedIndex = 0;
                textBox11.Text = "";
                textBox12.Text = "";
                comboBox6.SelectedIndex = 0;
                textBox13.Text = "";
                textBox14.Text = "";
                textBox4.Text = "";
                textBox15.Text = "";
            }
            else
            {
                label29.Text = p.编号;
                comboBox3.SelectedItem = p.费别;
                textBox5.Text = p.医保卡号;
                textBox3.Text = p.姓名;
                comboBox1.SelectedItem = p.性别;
                numericUpDown1.Value = p.年龄;
                numericUpDown2.Value = p.体重;
                comboBox4.SelectedItem = p.民族;
                comboBox5.SelectedItem = p.血型;
                textBox11.Text = p.籍贯;
                textBox12.Text = p.职业;
                comboBox6.SelectedItem = p.婚姻;
                textBox13.Text = p.身份证号;
                textBox14.Text = p.住址;
                textBox4.Text = p.电话;
                textBox15.Text = p.过敏史;
            }
            set_records();
        }

        private void medicineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedicineForm mf = new MedicineForm(db, Category.中药);
            mf.ShowDialog();
        }

        private void add_item(Category c)
        {
            if (this.record == null)
            {
                MessageBox.Show("请先选择病人与病历！", "提示信息");
            }
            else
            {
                MedicineForm mf = new MedicineForm(this.db, c);
                mf.ShowDialog();
                if (mf.result_medicine != null)
                {
                    Item item = new Item();
                    item.名称 = mf.result_medicine.名称;
                    item.单价 = mf.result_medicine.价格;
                    item.数量 = mf.result_amount;
                    item.小计 = item.单价 * item.数量;
                    Prescription p;
                    if (mf.result_medicine.category == Category.中成药与西药)
                    {
                        p = this.record.wprescription;
                    }
                    else
                    {
                        p = this.record.cprescription;
                    }
                    item.prescription = p;
                    p.items.Add(item);
                    p.price += item.小计;
                    set_prescriptions();
                }
            }
        }

        private void del_item(DataGridView dgv, Category c)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int index = dgv.CurrentRow.Index;
                Item i = ((List<Item>)dgv.DataSource)[index];
                Prescription p;
                if (c == Category.中成药与西药)
                {
                    p = this.record.wprescription;
                }
                else
                {
                    p = this.record.cprescription;
                }
                p.items.Remove(i);
                p.price -= i.小计;
                db.SaveChanges();
                set_prescriptions();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            add_item(Category.中成药与西药);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            del_item(dataGridView3, Category.中成药与西药);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            add_item(Category.中药);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            del_item(dataGridView4, Category.中药);
        }

        /*-----------病人页面的操作-------------*/
        //搜索条件改变
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //双击选中一个病人
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            set_patient(l[e.RowIndex]);
            set_patient_editable(false);
            button9.Enabled = true;
            button10.Enabled = true;
            button1.Enabled = false;
        }

        //新建病人
        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            set_patient(null);
            set_patient_editable(true);
            button9.Enabled = false;
            button10.Enabled = false;
            button1.Enabled = true;
        }

        //编辑当前选中的病人
        private void button10_Click(object sender, EventArgs e)
        {
            set_patient_editable(true);
            button10.Enabled = false;
            button1.Enabled = true;
        }

        //保存当前病人（新建的或选中的）
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.patient == null)
            {
                Patient patient = new Patient();
                patient.费别 = (string)comboBox3.SelectedItem;
                patient.医保卡号 = textBox5.Text;
                patient.姓名 = textBox3.Text;
                patient.性别 = (string)comboBox1.SelectedItem;
                patient.年龄 = numericUpDown1.Value;
                patient.体重 = numericUpDown2.Value;
                patient.民族 = (string)comboBox4.SelectedItem;
                patient.血型 = (string)comboBox5.SelectedItem;
                patient.籍贯 = textBox11.Text;
                patient.职业 = textBox12.Text;
                patient.婚姻 = (string)comboBox6.SelectedItem;
                patient.身份证号 = textBox13.Text;
                patient.住址 = textBox14.Text;
                patient.电话 = textBox4.Text;
                patient.过敏史 = textBox15.Text;
                db.patient_set.Add(patient);
                db.SaveChanges();
                patient.编号 = patient.id.ToString("00000");
                db.SaveChanges();
                set_patient(patient);
            }
            else
            {
                this.patient.费别 = (string)comboBox3.SelectedItem;
                this.patient.医保卡号 = textBox5.Text;
                this.patient.姓名 = textBox3.Text;
                this.patient.性别 = (string)comboBox1.SelectedItem;
                this.patient.年龄 = numericUpDown1.Value;
                this.patient.体重 = numericUpDown2.Value;
                this.patient.民族 = (string)comboBox4.SelectedItem;
                this.patient.血型 = (string)comboBox5.SelectedItem;
                this.patient.籍贯 = textBox11.Text;
                this.patient.职业 = textBox12.Text;
                this.patient.婚姻 = (string)comboBox6.SelectedItem;
                this.patient.身份证号 = textBox13.Text;
                this.patient.住址 = textBox14.Text;
                this.patient.电话 = textBox4.Text;
                this.patient.过敏史 = textBox15.Text;
                db.SaveChanges();
            }
            refresh();
            set_patient_editable(false);
            button9.Enabled = true;
            button10.Enabled = true;
            button1.Enabled = false;
        }

        /*-----------病历页面的操作-------------*/
        //双击选中一个病历
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            set_record(((List<Record>)this.patient.records)[e.RowIndex]);
        }

        //新建空病历
        private void button11_Click(object sender, EventArgs e)
        {
            set_record(null);
        }

        //保存新病历
        private void button2_Click(object sender, EventArgs e)
        {
            Record r = new Record();
            r.patient = this.patient;
            r.类型 = (string)comboBox2.SelectedItem;
            r.科别 = (string)comboBox7.SelectedItem;
            r.就诊日期 = dateTimePicker1.Value;
            r.主诉 = textBox6.Text;
            r.现病史 = textBox7.Text;
            r.既往史 = textBox8.Text;
            r.个人史 = textBox21.Text;
            r.家族史 = textBox9.Text;
            r.月经及婚育史 = textBox10.Text;
            r.体格检查 = textBox16.Text;
            r.辅助检查 = textBox17.Text;
            r.临床诊断 = textBox18.Text;
            r.治疗意见 = textBox19.Text;
            r.说明 = textBox20.Text;
            r.wprescription = new Prescription();
            r.cprescription = new Prescription();
            this.patient.records.Add(r);
            db.SaveChanges();
            set_records();
        }

        /*-----------处方页面的操作-------------*/

        private void button7_Click(object sender, EventArgs e)
        {
            this.record.医师 = textBox22.Text;
            this.record.调配 = textBox23.Text;
            this.record.审核 = textBox24.Text;
            this.record.核对 = textBox25.Text;
            this.record.发药 = textBox26.Text;
            this.record.finished = true;
            db.SaveChanges();
            set_prescription_editable(false);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (this.record != null)
            {
                this.record.cprescription.amount = (int)numericUpDown3.Value;
                set_prescriptions();
            }
        }


        /*-----------预览页面的操作-------------*/
        //生成打印预览
        private void button12_Click(object sender, EventArgs e)
        {
            if (this.patient == null || this.record == null)
            {
                MessageBox.Show("请先选择病人与病历！", "提示信息");
                return;
            }
            List<Record> r = new List<Record>();
            r.Add(this.record);
            List<Patient> p = new List<Patient>();
            p.Add(this.patient);
            List<Prescription> wp = new List<Prescription>();
            wp.Add(this.record.wprescription);
            List<Prescription> cp = new List<Prescription>();
            cp.Add(this.record.cprescription);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("patient", p));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("record", r));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("wp", wp));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("cp", cp));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("witems", this.record.wprescription.items.ToList()));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("citems", this.record.cprescription.items.ToList()));
            reportViewer1.RefreshReport();
        }

        private void prescriptionTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateForm tf = new TemplateForm(db);
            tf.ShowDialog();
        }
    }
}
