using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using System.Data.Linq.SqlClient;
using System.ComponentModel;

namespace E_Diagnosis
{
    public partial class MainForm : Form
    {
        //分隔符，用于分隔主诉中的关键字
        static char[] separator = { ' ', ',', '.', ';', '，', '。', '；' };
        //数据库
        DiagnosisContext db;
        //当前病人列表
        private List<Patient> l;
        //当前病历
        private Record record;
        //当前病人
        private Patient patient;
        //中药自动补全内容
        AutoCompleteStringCollection caccs;
        //西药自动补全内容
        AutoCompleteStringCollection waccs;

        //初始化
        public MainForm()
        {
            InitializeComponent();
            //初始化combobox
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            //初始化预览窗口
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth;
        }

        //刷新用户列表
        private void refresh()
        {
            //根据搜索条件查找所有病人（默认为所有）
            IEnumerable<Patient> query = from patient in db.patient_set
                                         where patient.编号.Contains(textBox1.Text) && patient.姓名.Contains(textBox2.Text)
                                         select patient;
            l = query.ToList();
            //显示为列表，并隐藏不需要的项
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
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            dataGridView1.Columns[8].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[11].Visible = false;
            dataGridView1.Columns[12].Visible = false;
            dataGridView1.Columns[13].Visible = false;
            dataGridView1.Columns[14].Visible = false;
            dataGridView1.Columns[15].Visible = false;
            dataGridView1.Columns[16].Visible = false;
            dataGridView1.Columns[17].Visible = false;
            dataGridView1.ClearSelection();
            //如果当前选中了一个病人，在改变搜索条件后，如果仍在列表中，则将其选中
            if (this.patient != null)
            {
                
                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    Patient p = (Patient)row.DataBoundItem;
                    if (p == this.patient)
                    {
                        row.Selected = true;
                        dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
        }

        //进入主窗口
        private void MainForm_Load(object sender, EventArgs e)
        {
            //不加这几行显示上会有个小bug，不影响功能正常使用
            tabControl1.SelectTab(1);
            tabControl1.SelectTab(2);
            tabControl1.SelectTab(0);

            this.Show();
            //打开密码输入窗口获取密码
            PasswordForm pf = new PasswordForm();
            pf.ShowDialog();
            //如果点击退出则直接退出
            if (pf.password == null)
            {
                Application.Exit();
            }
            //使用密码打开数据库，此时无法判断密码是否正确
            string cstring = String.Format("Data Source=E-Diagnosis.db;Password={0};", pf.password);
            SQLiteConnection connection= new SQLiteConnection(cstring);
            connection.Open();
            db = new DiagnosisContext(connection);
            //刷新所有病人，进行第一次数据库操作时才能判断密码是否正确
            try
            {
                refresh();
            }
            //如果密码出错，则提示并退出
            catch
            {
                MessageBox.Show("密码错误，程序即将退出！", "提示信息");
                Application.Exit();
            }
            this.reportViewer1.RefreshReport();
            //初始化自动补全内容
            IEnumerable<String> query = from medicine in db.medicine_set
                                        where medicine.category==Category.中药
                                        select medicine.名称;
            this.caccs = new AutoCompleteStringCollection();
            this.caccs.AddRange(query.ToArray());
            query = from medicine in db.medicine_set
                    where medicine.category == Category.中成药
                    select medicine.名称;
            this.waccs = new AutoCompleteStringCollection();
            this.waccs.AddRange(query.ToArray());
        }

        //显示当前选中病历的处方
        private void set_prescriptions()
        {
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.RefreshReport();
            //如果已经选中一条病历，则显示该病历的处方
            if (this.record != null)
            {
                //西药处方
                label46.Text = this.record.wprescription.price.ToString() + "元";
                if (dataGridView3.Columns["delete"] != null)
                {
                    dataGridView3.Columns.Remove("delete");
                }
                List<Item> l = this.record.wprescription.items.ToList();
                Item i = new Item();
                i.名称 = "[新增]";
                i.数量 = 1;
                l.Add(i);
                dataGridView3.DataSource = l;
                DataGridViewButtonColumn bc = new DataGridViewButtonColumn();
                bc.Text = "删除";
                bc.Name = "delete";
                bc.HeaderText = "";
                bc.UseColumnTextForButtonValue = true;
                dataGridView3.Columns.Add(bc);
                dataGridView3.ClearSelection();
                dataGridView3.Columns[0].Visible = false;
                dataGridView3.Columns[1].Visible = false;
                dataGridView3.Columns[2].Visible = false;
                dataGridView3.Columns[5].Visible = false;
                dataGridView3.Columns[6].Visible = false;
                ////dataGridView3.Columns[3].ReadOnly = true;
                dataGridView3.Columns[7].ReadOnly = true;

                //中药处方
                numericUpDown3.Value = this.record.cprescription.amount;
                label47.Text = String.Format("每剂{0}元\n共{1}元", this.record.cprescription.price, this.record.cprescription.price * this.record.cprescription.amount);
                if (dataGridView4.Columns["delete"] != null)
                {
                    dataGridView4.Columns.Remove("delete");
                }
                l = this.record.cprescription.items.ToList();
                i = new Item();
                i.名称 = "[新增]";
                i.数量 = 1;
                l.Add(i);
                dataGridView4.DataSource = l;
                bc = new DataGridViewButtonColumn();
                bc.Text = "删除";
                bc.Name = "delete";
                bc.HeaderText = "";
                bc.UseColumnTextForButtonValue = true;
                dataGridView4.Columns.Add(bc);
                dataGridView4.ClearSelection();
                dataGridView4.Columns[0].Visible = false;
                dataGridView4.Columns[1].Visible = false;
                dataGridView4.Columns[2].Visible = false;
                dataGridView4.Columns[5].Visible = false;
                dataGridView4.Columns[6].Visible = false;
                ////dataGridView4.Columns[3].ReadOnly = true;
                dataGridView4.Columns[7].ReadOnly = true;

                //总价
                label48.Text = "总金额：" + (this.record.wprescription.price + this.record.cprescription.price * this.record.cprescription.amount).ToString() + "元";
            }
            //否则清空处方
            else
            {
                dataGridView3.DataSource = null;
                dataGridView4.DataSource = null;
                label46.Text = "0元";
                label47.Text = "0元";
                label48.Text = "总金额：0元";
                numericUpDown3.Value = 1;
            }
        }

        //显示推荐药方
        private void set_recommendations()
        {
            //如果当前未选中病历，则返回
            if (this.record == null)
            {
                return;
            }
            //将主诉拆分为关键词用于搜索模板
            //string[] keywords = this.record.主诉.Split(separator);
            string[] keywords = textBox6.Text.Split(separator);
            var query = from template in db.template_set
                        where keywords.Any(keyword => keyword != "" && template.keywords.Contains(keyword))
                        select template;
            List<Template> recommendations = query.ToList<Template>();
            //显示所有搜索到的模板
            dataGridView5.DataSource = recommendations;
            dataGridView5.Columns[0].Visible = false;
            dataGridView5.Columns[1].Visible = false;
            dataGridView5.Columns[3].Visible = false;
            dataGridView5.Columns[4].Visible = false;
            dataGridView5.Columns[5].Visible = false;
            dataGridView5.Columns[6].Visible = false;
            dataGridView5.ClearSelection();
        }

        //显示一条病历内容
        private void set_record(Record r)
        {
            this.record = r;
            //如果为空则清空
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
                comboBox8.Text = "";
                textBox27.Text = "";
                dataGridView5.DataSource = null;
                dataGridView3.Visible = false;
            }
            //不为空则显示内容
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
                comboBox8.Text = r.服用方法;
                textBox27.Text = "";
                if (record.wprescription.items.Count == 0)
                {
                    dataGridView3.Visible = false;
                }
                else
                {
                    dataGridView3.Visible = true;
                }

                //并设置推荐模板
                set_recommendations();
            }
            //并显示该病历的处方
            set_prescriptions();
        }

        //显示当前选中病人的所有病历
        private void set_records()
        {
            //如果当前未选中病人则清空病历
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
                //设置历史处方列表
                dataGridView6.DataSource = new List<Record>();
                for (int i = 0; i < 24; i++)
                {
                    if (i == 2 || i == 4)
                    {
                        continue;
                    }
                    dataGridView6.Columns[i].Visible = false;
                }
                dataGridView6.Columns[4].DefaultCellStyle.Format = "yyyy/MM/dd";
            }
            //如果已选中病人则显示该病人的所有病历
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
                //如果之前已经选中其中一条病历，则仍然设置为选中状态
                if (this.record != null && this.record.patient == this.patient)
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (this.record == (Record)row.DataBoundItem)
                        {
                            row.Selected = true;
                            dataGridView2.FirstDisplayedScrollingRowIndex = row.Index;
                            set_record(this.record);
                            break;
                        }
                    }
                }
                //如果之前没有选中任何病历，则默认选中最后一条
                else
                {
                    if (dataGridView2.Rows.Count > 0)
                    {
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Selected = true;
                        dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.RowCount - 1;
                        set_record(((List<Record>)this.patient.records)[dataGridView2.Rows.Count - 1]);
                    }
                    else
                    {
                        set_record(null);
                    }
                }
                //设置历史处方列表
                dataGridView6.DataSource = this.patient.records.ToList();
                for(int i = 0; i < 24; i++)
                {
                    if (i == 2 || i == 4)
                    {
                        continue;
                    }
                    dataGridView6.Columns[i].Visible = false;
                }
                dataGridView6.Columns[4].DefaultCellStyle.Format = "yyyy/MM/dd";
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
            //之后显示该病人所有病历
            set_records();
        }

        //打开药品编辑窗口
        private void medicineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedicineForm mf = new MedicineForm(db, Category.中药);
            mf.ShowDialog();
            //编辑药品后重新初始化自动补全内容
            IEnumerable<String> query = from medicine in db.medicine_set
                                        where medicine.category == Category.中药
                                        select medicine.名称;
            this.caccs = new AutoCompleteStringCollection();
            this.caccs.AddRange(query.ToArray());
            query = from medicine in db.medicine_set
                    where medicine.category == Category.中成药
                    select medicine.名称;
            this.waccs = new AutoCompleteStringCollection();
            this.waccs.AddRange(query.ToArray());
        }

        //打开模板编辑窗口
        private void prescriptionTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateForm tf = new TemplateForm(db);
            tf.ShowDialog();
            set_recommendations();
        }

        //打开设置窗口
        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForm sf = new SettingForm();
            sf.ShowDialog();
        }

        //打开修改密码窗口
        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPasswordForm npf = new NewPasswordForm();
            npf.ShowDialog();
            //如果确定修改，则对数据库文件进行修改并显示成功信息
            if (npf.newpassword != null)
            {
                ((SQLiteConnection)db.Database.Connection).ChangePassword(npf.newpassword);
                MessageBox.Show("修改密码成功！", "提示信息");
            }
        }

        /*-----------病人页面的操作-------------*/

        //搜索条件改变时刷新
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            refresh();
        }

        //搜索条件改变时刷新
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

        //新建病人（直接添加一个空病人，之后再进行修改）
        private void button9_Click(object sender, EventArgs e)
        {
            //dataGridView1.ClearSelection();
            //set_patient(null);
            Patient patient = new Patient();
            patient.费别 = "自费";
            patient.医保卡号 = "";
            patient.姓名 = "新病人";
            patient.性别 = "男";
            patient.年龄 = 0;
            patient.体重 = 0;
            patient.民族 = "汉族";
            patient.血型 = "A型";
            patient.籍贯 = "";
            patient.职业 = "";
            patient.婚姻 = "已婚";
            patient.身份证号 = "";
            patient.住址 = "";
            patient.电话 = "";
            patient.过敏史 = "";
            db.patient_set.Add(patient);
            Record r = new Record();
            r.类型 = "初诊";
            r.就诊日期 = DateTime.Now;
            r.wprescription = new Prescription();
            r.cprescription = new Prescription();
            patient.records.Add(r);
            db.SaveChanges();
            patient.编号 = patient.id.ToString("00000");
            db.SaveChanges();
            set_patient(patient);
            textBox1.Text = "";
            textBox2.Text = "";
            refresh();
        }
        
        //保存当前病人
        private void button1_Click(object sender, EventArgs e)
        {
            //当前未选中病人则报错
            if (this.patient == null)
            {
                MessageBox.Show("请先选择一个病人！", "提示信息");
            }
            //否则修改当前病人并保存
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
                refresh();
                MessageBox.Show("保存成功！", "提示信息");
            }
        }

        /*-----------病历页面的操作-------------*/

        //双击选中一个病历
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            set_record(((List<Record>)this.patient.records)[e.RowIndex]);
        }

        ////新建病历
        //private void button11_Click(object sender, EventArgs e)
        //{
        //    dataGridView2.ClearSelection();
        //    set_record(null);
        //}

        //点击复制新建按钮
        private void button10_Click(object sender, EventArgs e)
        {
            if (this.patient == null)
            {
                MessageBox.Show("请先选择病人！", "提示信息");
                return;
            }
            //if (this.patient.records.Count == 0)
            //{
            //    button11_Click(sender, e);
            //    return;
            //}
            //复制当前病历（深拷贝）
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
            r.服用方法 = lr.服用方法;
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
                item.规格 = litem.规格;
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
                item.规格 = litem.规格;
                r.cprescription.items.Add(item);
            }
            db.SaveChanges();
            this.record= ((List<Record>)this.patient.records)[this.patient.records.Count - 1];
            set_records();
            MessageBox.Show("新建成功！", "提示信息");
        }

        //保存病历
        private void button2_Click(object sender, EventArgs e)
        {
            //当前病人不能为空
            if (this.patient == null)
            {
                MessageBox.Show("请先选择病人！", "提示信息");
                return;
            }
            //如果当前病历为空，新建并保存
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
                r.服用方法 = comboBox8.Text;
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
            //否则修改并保存
            else
            {
                Record r = this.record;
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
                r.服用方法 = comboBox8.Text;
            }
            db.SaveChanges();
            //刷新病历列表
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

        ////双击修改已添加的西药
        //private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0)
        //        return;
        //    Item item = (Item)dataGridView3.Rows[e.RowIndex].DataBoundItem;
        //    new MedicineEditForm(item).ShowDialog();
        //    db.SaveChanges();
        //    set_prescriptions();
        //}

        //新增药品时，添加自动补全
        private void dataGridView4_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell.ColumnIndex == 3)
            {
                if (dgv.CurrentCell.RowIndex == dgv.RowCount - 1)
                {
                    tb.Text = "";
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    if (dgv == dataGridView4)
                    {
                        tb.AutoCompleteCustomSource = this.caccs;
                    }
                    else
                    {
                        tb.AutoCompleteCustomSource = this.waccs;
                    }
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

        private void dataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            //新增药品后添加至处方
            if (e.RowIndex == dgv.RowCount - 1)
            {
                if (e.ColumnIndex == 3)
                {
                    String s = (String)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    IEnumerable<Medicine> query = from medicine in db.medicine_set
                                                  where medicine.名称.Equals(s)
                                                  select medicine;
                    Medicine[] medicines = query.ToArray();
                    if (medicines.Length == 0)
                    {
                        MessageBox.Show("药品名称错误！", "提示信息");
                        return;
                    }
                    Medicine m = medicines[0];
                    //根据选择的药品类型确定要添加的处方
                    Prescription p;
                    if (dgv == dataGridView4)
                    {
                        p = this.record.cprescription;
                    }
                    else
                    {
                        p = this.record.wprescription;
                    }
                    //向该处方中添加选择的药品
                    add_item(p, m, 1);
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        Item item = (Item)row.DataBoundItem;
                        if (item.medicine == m && item.名称 == m.名称 && item.单价 == m.价格)
                        {
                            row.Selected = true;
                            dgv.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }
                    MessageBox.Show("添加成功！", "提示信息");
                }
            }
            //中药处方修改单价或数量后，自动修改所有价格
            else
            {
                Item item = (Item)dgv.Rows[e.RowIndex].DataBoundItem;
                item.prescription.price -= item.小计;
                item.小计 = decimal.Round(item.数量 * item.单价 + 0.00M, 2);
                item.prescription.price += item.小计;
                dgv.Refresh();
                set_prescriptions();
                dgv.Rows[e.RowIndex].Selected = true;
                dgv.FirstDisplayedScrollingRowIndex = e.RowIndex;
            }
        }

        ////中成药处方修改单价或数量后，自动修改其它内容
        //private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    Item item = (Item)dataGridView3.Rows[e.RowIndex].DataBoundItem;
        //    item.prescription.price -= item.小计;
        //    item.小计 = decimal.Round(item.数量 * item.单价 + 0.00M, 2);
        //    item.prescription.price += item.小计;
        //    dataGridView3.Refresh();
        //    set_prescriptions();
        //}

        //向处方中添加一味药品
        private void add_item(Prescription pre, Medicine medicine, decimal amount)
        {
            bool found = false;
            foreach (Item item in pre.items)
            {
                //如果该药品已经存在于处方中，则把数量增加
                if (item.medicine == medicine && item.名称 == medicine.名称 && item.单价 == medicine.价格)
                {
                    item.数量 += amount;
                    decimal extra = decimal.Round(medicine.价格 * amount + 0.00M, 2);
                    item.小计 += extra;
                    pre.price += extra;
                    found = true;
                    break;
                }
            }
            //如果不存在，则新建并添加
            if (!found)
            {
                Item item = new Item();
                item.medicine = medicine;
                item.名称 = medicine.名称;
                item.单价 = medicine.价格;
                item.数量 = amount;
                item.小计 = decimal.Round(item.单价 * item.数量 + 0.00M, 2);
                item.规格 = medicine.规格;
                item.prescription = pre;
                pre.items.Add(item);
                pre.price += item.小计;
            }
            //db.SaveChanges();
            set_prescriptions();
        }

        //点击按钮，打开药品窗口，让用户选择一味药品进行添加
        //private void start_add_item(Category c)
        //{
        //    //当前病历不能为空
        //    if (this.record == null)
        //    {
        //        MessageBox.Show("请先选择病人与病历！", "提示信息");
        //    }
        //    else
        //    {
        //        //打开药品选择窗口
        //        MedicineForm mf = new MedicineForm(this.db, c);
        //        mf.ShowDialog();
        //        if (mf.result_medicine != null)
        //        {
        //            //根据选择的药品类型确定要添加的处方
        //            Prescription p;
        //            if (mf.result_medicine.category == Category.中成药)
        //            {
        //                p = this.record.wprescription;
        //            }
        //            else
        //            {
        //                p = this.record.cprescription;
        //            }
        //            //向该处方中添加选择的药品
        //            add_item(p, mf.result_medicine, mf.result_amount);
        //        }
        //    }
        //}

        //从处方中删除一味药品
        private void del_item(DataGridView dgv, Category c)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("请先选择一项！", "提示信息");
            }
            else if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //int index = dgv.CurrentRow.Index;
                //Item i = ((List<Item>)dgv.DataSource)[index];
                Item i = (Item)dgv.CurrentRow.DataBoundItem;
                //确定要删除的药品所属处方
                Prescription p;
                if (c == Category.中成药)
                {
                    p = this.record.wprescription;
                }
                else
                {
                    p = this.record.cprescription;
                }
                //删除该药品并从总价中扣除
                p.items.Remove(i);
                p.price -= i.小计;
                //db.SaveChanges();
                set_prescriptions();
            }

        }

        //添加药品至处方
        private void button4_Click(object sender, EventArgs e)
        {
            //当前病历不能为空
            if (this.record == null)
            {
                MessageBox.Show("请先选择病人与病历！", "提示信息");
            }
            else
            {
                IEnumerable<Medicine> query = from medicine in db.medicine_set
                                              where medicine.名称.Equals(textBox27.Text)
                                              select medicine;
                Medicine[] medicines = query.ToArray();
                if (medicines.Length == 0)
                {
                    MessageBox.Show("药品名称错误！", "提示信息");
                    return;
                }
                Medicine m = medicines[0];
                //根据选择的药品类型确定要添加的处方
                Prescription p;
                DataGridView dgv;
                if (m.category == Category.中成药)
                {
                    p = this.record.wprescription;
                    dgv = dataGridView3;
                }
                else
                {
                    p = this.record.cprescription;
                    dgv = dataGridView4;
                }
                //向该处方中添加选择的药品
                add_item(p, m, 1);
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    Item item = (Item)row.DataBoundItem;
                    if (item.medicine == m && item.名称 == m.名称 && item.单价 == m.价格)
                    {
                        row.Selected = true;
                        dgv.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
                MessageBox.Show("添加成功！", "提示信息");
            }
        }

        ////点击西药添加按钮
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    start_add_item(Category.中成药);
        //}

        //点击西药删除按钮
        //private void button4_Click(object sender, EventArgs e)
        //{
        //    del_item(dataGridView3, Category.中成药);
        //}

        //点击西药删除按钮
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex<dataGridView3.RowCount-1 && e.ColumnIndex >= 0 && dataGridView3.Columns[e.ColumnIndex].Name == "delete")
            {
                del_item(dataGridView3, Category.中成药);
            }
        }

        ////点击中药添加按钮
        //private void button5_Click(object sender, EventArgs e)
        //{
        //    start_add_item(Category.中药);
        //}

        //点击中药删除按钮
        //private void button6_Click(object sender, EventArgs e)
        //{
        //    del_item(dataGridView4, Category.中药);
        //}

        //点击中药删除按钮
        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex>=0 && e.RowIndex < dataGridView4.RowCount - 1 && e.ColumnIndex >= 0 && dataGridView4.Columns[e.ColumnIndex].Name == "delete")
            {
                del_item(dataGridView4, Category.中药);
            }
        }

        //修改主诉自动更新推荐模板
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_recommendations();
        }

        //导入模板
        public void import_template(Template t)
        {
            //将模板中的每一味药添加到中药处方即可
            foreach (TemplateItem titem in t.items)
            {
                add_item(this.record.cprescription, titem.medicine, titem.数量);
            }
            //db.SaveChanges();
            set_prescriptions();
        }

        //双击模板导入
        private void dataGridView5_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            Template t = (Template)dataGridView5.Rows[e.RowIndex].DataBoundItem;
            import_template(t);
            MessageBox.Show("导入成功！", "提示信息");
        }

        //点击更多模板按钮，打开模板窗口进行选择
        private void button8_Click(object sender, EventArgs e)
        {
            if (this.record == null)
            {
                MessageBox.Show("请先选择病人与病历！", "提示信息");
                return;
            }
            //打开模板窗口进行选择
            TemplateForm tf = new TemplateForm(db);
            tf.ShowDialog();
            set_recommendations();
            Template t = tf.selected_template;
            //如果选择了模板则导入
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
            //初始化
            List<Record> r = new List<Record>();
            r.Add(this.record);
            List<Patient> p = new List<Patient>();
            p.Add(this.patient);
            List<Prescription> wp = new List<Prescription>();
            wp.Add(this.record.wprescription);
            List<Prescription> cp = new List<Prescription>();
            cp.Add(this.record.cprescription);
            reportViewer1.LocalReport.DataSources.Clear();
            //将需要的信息传入模板
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("patient", p));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("record", r));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("wp", wp));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("cp", cp));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("witems", this.record.wprescription.items.ToList()));
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("citems", this.record.cprescription.items.ToList()));
            List<Properties.Settings> settings = new List<Properties.Settings>();
            settings.Add(Properties.Settings.Default);
            reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("settings", settings));
            //刷新预览窗口
            reportViewer1.RefreshReport();
        }

        //显示中成药处方
        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView3.Visible = true;
        }

        //删除病人
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.RowCount - 1 && e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "delete")
            {
                if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Patient p = (Patient)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                    db.patient_set.Remove(p);
                    db.SaveChanges();
                    refresh();
                }
            }
        }
    }
}
