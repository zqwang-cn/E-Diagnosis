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
        static char[] separator = { ' ', ',', '.', ';', '，', '。', '；' };
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

        //刷新用户列表
        private void refresh()
        {
            IEnumerable<Patient> query = from patient in db.patient_set
                                         where (textBox1.Text=="" || patient.编号.Contains(textBox1.Text)) && (textBox2.Text=="" || patient.姓名.Contains(textBox2.Text))
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
                //wprescription
                label46.Text = this.record.wprescription.price.ToString() + "元";
                dataGridView3.DataSource = this.record.wprescription.items.ToList();
                dataGridView3.ClearSelection();
                dataGridView3.Columns[0].Visible = false;
                dataGridView3.Columns[1].Visible = false;
                dataGridView3.Columns[2].Visible = false;

                //cprescription
                numericUpDown3.Value = this.record.cprescription.amount;
                label47.Text = (this.record.cprescription.price * this.record.cprescription.amount).ToString() + "元";
                dataGridView4.DataSource = this.record.cprescription.items.ToList();
                dataGridView4.ClearSelection();
                dataGridView4.Columns[0].Visible = false;
                dataGridView4.Columns[1].Visible = false;
                dataGridView4.Columns[2].Visible = false;

                label48.Text = (this.record.wprescription.price + this.record.cprescription.price * this.record.cprescription.amount).ToString() + "元";
            }
            else
            {
                dataGridView3.DataSource = null;
                dataGridView4.DataSource = null;
                label46.Text = "0元";
                label47.Text = "0元";
                label48.Text = "0元";
                numericUpDown3.Value = 1;
            }
        }

        private void set_recommendations()
        {
            if (this.record == null)
            {
                return;
            }
            string[] keywords = this.record.主诉.Split(separator);
            var query = from template in db.template_set
                        where keywords.Any(keyword => keyword != "" && template.keywords.Contains(keyword))
                        select template;
            List<Template> recommendations = query.ToList<Template>();
            dataGridView5.DataSource = recommendations;
            dataGridView5.Columns[0].Visible = false;
            dataGridView5.Columns[1].Visible = false;
            dataGridView5.Columns[3].Visible = false;
            dataGridView5.Columns[4].Visible = false;
            dataGridView5.Columns[5].Visible = false;
            dataGridView5.Columns[6].Visible = false;
            dataGridView5.ClearSelection();
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
                textBox22.Text = "";
                textBox23.Text = "";
                textBox24.Text = "";
                textBox25.Text = "";
                textBox26.Text = "";
                dataGridView5.DataSource = null;
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
                textBox22.Text = r.医师;
                textBox23.Text = r.调配;
                textBox24.Text = r.审核;
                textBox25.Text = r.核对;
                textBox26.Text = r.发药;

                set_recommendations();
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
                dataGridView2.Columns[4].DefaultCellStyle.Format = "yyyy/MM/dd";
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
                dataGridView2.Columns[17].Visible = false;
                dataGridView2.Columns[18].Visible = false;
                dataGridView2.Columns[19].Visible = false;
                dataGridView2.Columns[20].Visible = false;
                dataGridView2.Columns[21].Visible = false;
                dataGridView2.Columns[22].Visible = false;
                set_record(null);
            }
            else
            {
                dataGridView2.DataSource = this.patient.records.ToList();
                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[1].Visible = false;
                dataGridView2.Columns[4].DefaultCellStyle.Format = "yyyy/MM/dd";
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
                dataGridView2.Columns[17].Visible = false;
                dataGridView2.Columns[18].Visible = false;
                dataGridView2.Columns[19].Visible = false;
                dataGridView2.Columns[20].Visible = false;
                dataGridView2.Columns[21].Visible = false;
                dataGridView2.Columns[22].Visible = false;
                if (this.record != null && this.record.patient == this.patient)
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (this.record == (Record)row.DataBoundItem)
                        {
                            row.Selected = true;
                            set_record(this.record);
                            break;
                        }
                    }
                }
                else
                {
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

        private void prescriptionTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateForm tf = new TemplateForm(db);
            tf.ShowDialog();
            set_recommendations();
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForm sf = new SettingForm();
            sf.ShowDialog();
        }

        private void add_item(Prescription pre, Medicine medicine, decimal amount)
        {
            bool found = false;
            foreach(Item item in pre.items)
            {
                if (item.medicine == medicine && item.名称==medicine.名称 && item.单价 == medicine.价格)
                {
                    item.数量 += amount;
                    decimal extra= decimal.Round(medicine.价格 * amount + 0.00M, 2);
                    item.小计 += extra;
                    pre.price += extra;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Item item = new Item();
                item.medicine = medicine;
                item.名称 = medicine.名称;
                item.单价 = medicine.价格;
                item.数量 = amount;
                item.小计 = decimal.Round(item.单价 * item.数量 + 0.00M, 2);
                pre.items.Add(item);
                pre.price += item.小计;
            }
            db.SaveChanges();
            set_prescriptions();
        }

        public void import_template(Template t)
        {
            foreach (TemplateItem titem in t.items)
            {
                add_item(this.record.cprescription, titem.药品, titem.数量);
            }
            db.SaveChanges();
            set_prescriptions();
        }

        private void start_add_item(Category c)
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
                    Prescription p;
                    if (mf.result_medicine.category == Category.中成药与西药)
                    {
                        p = this.record.wprescription;
                    }
                    else
                    {
                        p = this.record.cprescription;
                    }
                    add_item(p, mf.result_medicine, mf.result_amount);

                    //Item item = new Item();
                    //item.medicine = mf.result_medicine;
                    //item.名称 = mf.result_medicine.名称;
                    //item.单价 = mf.result_medicine.价格;
                    //item.数量 = mf.result_amount;
                    //item.小计 = decimal.Round(item.单价 * item.数量 + 0.00M, 2);
                    //p.items.Add(item);
                    //p.price += item.小计;
                    //db.SaveChanges();
                    //set_prescriptions();
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
            start_add_item(Category.中成药与西药);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            del_item(dataGridView3, Category.中成药与西药);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            start_add_item(Category.中药);
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
        }

        //新建病人
        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            set_patient(null);
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
            MessageBox.Show("保存成功！", "提示信息");
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
            dataGridView2.ClearSelection();
            set_record(null);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (this.patient == null)
            {
                MessageBox.Show("请先选择病人！", "提示信息");
                return;
            }
            if (this.patient.records.Count == 0)
            {
                button11_Click(sender, e);
                return;
            }
            Record lr = ((List<Record>)this.patient.records)[this.patient.records.Count - 1];
            Record r = new Record();
            this.patient.records.Add(r);
            r.patient = lr.patient;
            r.类型 = "复诊";
            r.科别 = lr.科别;
            r.就诊日期 = DateTime.Now;
            r.主诉 = lr.主诉;
            r.现病史 = lr.现病史;
            r.既往史 = lr.既往史;
            r.个人史 = lr.个人史;
            r.家族史 = lr.家族史;
            r.月经及婚育史 = lr.月经及婚育史;
            r.体格检查 = lr.体格检查;
            r.辅助检查 = lr.辅助检查;
            r.临床诊断 = lr.临床诊断;
            r.治疗意见 = lr.治疗意见;
            r.说明 = lr.说明;
            r.医师 = lr.医师;
            r.调配 = lr.调配;
            r.审核 = lr.审核;
            r.核对 = lr.核对;
            r.发药 = lr.发药;
            r.wprescription = new Prescription();
            r.wprescription.record = lr.wprescription.record;
            r.wprescription.amount = lr.wprescription.amount;
            r.wprescription.price = lr.wprescription.price;
            foreach(Item litem in lr.wprescription.items)
            {
                Item item = new Item();
                item.prescription = r.wprescription;
                item.medicine = litem.medicine;
                item.名称 = litem.名称;
                item.数量 = litem.数量;
                item.单价 = litem.单价;
                item.小计 = litem.小计;
                r.wprescription.items.Add(item);
            }
            r.cprescription = new Prescription();
            r.cprescription.record = lr.cprescription.record;
            r.cprescription.amount = lr.cprescription.amount;
            r.cprescription.price = lr.cprescription.price;
            foreach (Item litem in lr.cprescription.items)
            {
                Item item = new Item();
                item.prescription = r.cprescription;
                item.medicine = litem.medicine;
                item.名称 = litem.名称;
                item.数量 = litem.数量;
                item.单价 = litem.单价;
                item.小计 = litem.小计;
                r.cprescription.items.Add(item);
            }
            db.SaveChanges();
            this.record= ((List<Record>)this.patient.records)[this.patient.records.Count - 1];
            set_records();
        }

        //保存病历
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.patient == null)
            {
                MessageBox.Show("请先选择病人！", "提示信息");
                return;
            }
            if (this.record == null)
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
                r.医师 = textBox22.Text;
                r.调配 = textBox23.Text;
                r.审核 = textBox24.Text;
                r.核对 = textBox25.Text;
                r.发药 = textBox26.Text;
                r.wprescription = new Prescription();
                r.cprescription = new Prescription();
                if (this.patient.records.Count != 0)
                {
                    Record pr = this.patient.records.Last();
                    foreach(Item pi in pr.wprescription.items)
                    {
                        Item i = new Item();
                        i.名称 = pi.名称;
                        i.单价 = pi.单价;
                        i.数量 = pi.数量;
                        i.小计 = pi.小计;
                        i.medicine = pi.medicine;
                        r.wprescription.items.Add(i);
                    }
                    r.wprescription.price = pr.wprescription.price;
                    foreach (Item pi in pr.cprescription.items)
                    {
                        Item i = new Item();
                        i.名称 = pi.名称;
                        i.单价 = pi.单价;
                        i.数量 = pi.数量;
                        i.小计 = pi.小计;
                        i.medicine = pi.medicine;
                        r.cprescription.items.Add(i);
                    }
                    r.cprescription.amount = pr.cprescription.amount;
                    r.cprescription.price = pr.cprescription.price;
                }
                this.patient.records.Add(r);
            }
            else
            {
                Record r = this.record;
                //r.patient = this.patient;
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
                r.医师 = textBox22.Text;
                r.调配 = textBox23.Text;
                r.审核 = textBox24.Text;
                r.核对 = textBox25.Text;
                r.发药 = textBox26.Text;
            }
            db.SaveChanges();
            set_records();
            MessageBox.Show("保存成功！", "提示信息");

        }

        /*-----------处方页面的操作-------------*/
        //修改剂数
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (this.record != null)
            {
                this.record.cprescription.amount = (int)numericUpDown3.Value;
                db.SaveChanges();
                set_prescriptions();
            }
        }

        //导入模板
        private void button8_Click(object sender, EventArgs e)
        {
            if (this.record == null)
            {
                MessageBox.Show("请先选择病人与病历！", "提示信息");
                return;
            }
            TemplateForm tf = new TemplateForm(db);
            tf.ShowDialog();
            set_recommendations();
            Template t = tf.selected_template;
            if (t != null)
            {
                import_template(t);
                MessageBox.Show("导入成功！", "提示信息");
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
            List<Properties.Settings> settings = new List<Properties.Settings>();
            settings.Add(Properties.Settings.Default);
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("settings", settings));
            reportViewer1.RefreshReport();
        }

        //双击修改已添加的西药中成药
        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            Item item = (Item)dataGridView3.Rows[e.RowIndex].DataBoundItem;
            new MedicineEditForm(item).ShowDialog();
            db.SaveChanges();
            set_prescriptions();
        }

        private void dataGridView4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            Item item = (Item)dataGridView4.Rows[e.RowIndex].DataBoundItem;
            new MedicineEditForm(item).ShowDialog();
            db.SaveChanges();
            set_prescriptions();
        }

        //导入选中的推荐模板
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView5.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择模板", "提示信息");
            }
            else
            {
                Template t = (Template)dataGridView5.SelectedRows[0].DataBoundItem;
                import_template(t);
                MessageBox.Show("导入成功！", "提示信息");
            }
        }
    }
}
