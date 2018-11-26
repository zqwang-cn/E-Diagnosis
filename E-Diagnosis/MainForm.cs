using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace E_Diagnosis
{

    public partial class MainForm : Form
    {
        //分隔符，用于分隔主诉中的关键字
        static char[] separator = { ' ', ',', '.', ';', '，', '。', '；' };
        //数据库
        DiagnosisContext db;
        //当前病历
        private Record record;
        //当前病人
        private Patient patient;

        //查找药材对应的控件
        public static Control GetAnyControlAt(TableLayoutPanel pp, int col, int row)
        {
            bool fnd = false;
            Control sendCC = null;
            foreach (Control cc in pp.Controls)
            {
                if (pp.GetCellPosition(cc).Column == col)
                {
                    if (pp.GetCellPosition(cc).Row == row)
                    {
                        sendCC = cc;
                        fnd = true;
                        break;
                    }
                }
            }

            if (fnd == true)
            {
                return sendCC;
            }
            else
            {
                return null;
            }
        }

        //初始化
        public MainForm()
        {
            InitializeComponent();
            //初始化预览窗口
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth;
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
                return;
            }
            //使用密码打开数据库，此时无法判断密码是否正确
            string cstring = String.Format("Data Source=E-Diagnosis.db;Password={0};", pf.password);
            SQLiteConnection connection = new SQLiteConnection(cstring);
            connection.Open();
            db = new DiagnosisContext(connection);
            try
            {
                //载入数据
                db.patient_set.Load();
            }
            //如果密码出错，则提示并退出
            catch
            {
                MessageBox.Show("密码错误，程序即将退出！", "提示信息");
                Application.Exit();
                return;
            }
            //刷新病人列表
            refresh();
            this.reportViewer1.RefreshReport();
        }

        //刷新用户列表
        private void refresh()
        {
            patientBindingSource1.DataSource = db.patient_set.Where(p => p.编号.Contains(textBox1.Text) && p.姓名.Contains(textBox2.Text)).ToList();
            dataGridView1.ClearSelection();
            //如果当前选中了一个病人，在改变搜索条件后，如果仍在列表中，则将其选中
            if (this.patient != null)
            {

                foreach (DataGridViewRow row in dataGridView1.Rows)
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

        //双击选中一个病人
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            set_patient((Patient)dataGridView1.Rows[e.RowIndex].DataBoundItem);
        }

        //新建病人（直接添加一个空病人，之后再进行修改）
        private void button9_Click(object sender, EventArgs e)
        {
            Patient patient = new Patient();
            db.patient_set.Add(patient);
            Record r = new Record();
            r.cprescription = new Prescription();
            r.wprescription = new Prescription();
            patient.records.Add(r);
            db.SaveChanges();
            patient.编号 = patient.id.ToString("0000000");
            db.SaveChanges();
            set_patient(patient);
            textBox1.Text = "";
            textBox2.Text = "";
            refresh();
        }

        //删除病人
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.RowCount && e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "delete")
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

        //显示当前选中病历的处方
        private void set_prescriptions()
        {
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.RefreshReport();
            //设置处方内容
            itemBindingSource.DataSource = null;
            itemBindingSource.DataSource = this.record.cprescription.items;
            dataGridView3.ClearSelection();
            dataGridView3.Columns[0].ReadOnly = true;
            dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[0].ReadOnly = false;
            dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[1].ReadOnly = true;
            dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[3].ReadOnly = true;
            itemBindingSource1.DataSource = null;
            itemBindingSource1.DataSource = this.record.wprescription.items;
            dataGridView4.ClearSelection();
            dataGridView4.Columns[0].ReadOnly = true;
            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[0].ReadOnly = false;
            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[1].ReadOnly = true;
            //如果已经选中一条病历，则显示该病历的处方
            //if (this.record != null)
            //{
            //    int row, col;
            //    TableLayoutPanel tlp;
            //    //中药处方
            //    int i;

            //    //显示中药处方
            //    itemBindingSource.DataSource = null;
            //    itemBindingSource.DataSource = this.record.cprescription.items;
            //    itemBindingSource.MoveFirst();
            //    i = 0;
            //    while (i < itemBindingSource.Count)
            //    {
            //        row = i / 5;
            //        col = i % 5;
            //        tlp = (TableLayoutPanel)GetAnyControlAt(tableLayoutPanel2, col, row);
            //        tlp.Visible = true;
            //        tlp.GetControlFromPosition(0, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(0, 0).DataBindings.Add("Text", itemBindingSource.Current, "名称");
            //        tlp.GetControlFromPosition(1, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(1, 0).DataBindings.Add("Text", itemBindingSource.Current, "煎法");
            //        tlp.GetControlFromPosition(1, 1).DataBindings.Clear();
            //        tlp.GetControlFromPosition(1, 1).DataBindings.Add("Text", itemBindingSource.Current, "数量");
            //        tlp.GetControlFromPosition(2, 1).DataBindings.Clear();
            //        tlp.GetControlFromPosition(2, 1).DataBindings.Add("Text", itemBindingSource.Current, "规格");
            //        itemBindingSource.MoveNext();
            //        i++;
            //    }
            //    //清空并隐藏其它药品格
            //    for (int j = i; j < 25; j++)
            //    {
            //        row = j / 5;
            //        col = j % 5;
            //        tlp = (TableLayoutPanel)GetAnyControlAt(tableLayoutPanel2, col, row);
            //        tlp.Visible = false;
            //        tlp.GetControlFromPosition(0, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(1, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(1, 1).DataBindings.Clear();
            //        tlp.GetControlFromPosition(2, 1).DataBindings.Clear();
            //    }

            //    //显示中成药处方
            //    itemBindingSource1.DataSource = null;
            //    itemBindingSource1.DataSource = this.record.wprescription.items;
            //    itemBindingSource1.MoveFirst();
            //    i = 0;
            //    while (i < itemBindingSource1.Count)
            //    {
            //        row = i / 5;
            //        col = i % 5;
            //        tlp = (TableLayoutPanel)GetAnyControlAt(tableLayoutPanel10, col, row);
            //        tlp.Visible = true;
            //        tlp.GetControlFromPosition(0, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(0, 0).DataBindings.Add("Text", itemBindingSource1.Current, "名称");
            //        tlp.GetControlFromPosition(1, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(1, 0).DataBindings.Add("Text", itemBindingSource1.Current, "数量");
            //        tlp.GetControlFromPosition(2, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(2, 0).DataBindings.Add("Text", itemBindingSource1.Current, "规格");
            //        itemBindingSource1.MoveNext();
            //        i++;
            //    }
            //    //清空并隐藏其它药品格
            //    for (int j = i; j < 10; j++)
            //    {
            //        row = j / 5;
            //        col = j % 5;
            //        tlp = (TableLayoutPanel)GetAnyControlAt(tableLayoutPanel10, col, row);
            //        tlp.Visible = false;
            //        tlp.GetControlFromPosition(1, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(0, 0).DataBindings.Clear();
            //        tlp.GetControlFromPosition(1, 1).DataBindings.Clear();
            //        tlp.GetControlFromPosition(2, 1).DataBindings.Clear();
            //    }
            //}
            ////否则清空处方
            //else
            //{
            //    //dataGridView3.DataSource = null;
            //    dataGridView4.DataSource = null;
            //    //label46.Text = "0元";
            //    //label47.Text = "0元";
            //    //label48.Text = "总金额：0元";
            //    numericUpDown3.Value = 1;
            //}
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
            string[] keywords = textBox6.Text.Split(separator);
            templateBindingSource.DataSource = db.template_set.Where(t=> t.keywords!=null && keywords.Any(keyword => keyword != "" && t.keywords.Contains(keyword))).ToList();
            dataGridView5.ClearSelection();
        }

        //显示一条病历内容
        private void set_record(Record r)
        {
            this.record = r;
            //显示内容
            recordBindingSource.DataSource = r;
            //并设置推荐模板
            set_recommendations();
            //并显示该病历的处方
            set_prescriptions();
        }

        //设置当前选中的病人（为null则清空）
        private void set_patient(Patient p)
        {
            this.patient = p;
            patientBindingSource.DataSource = p;
            set_record(p.records.Last());
        }

        //打开药品编辑窗口
        private void medicineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedicineForm mf = new MedicineForm(db, Category.中药);
            mf.ShowDialog();
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

        /*-----------病历页面的操作-------------*/

        //双击选中一个病历
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            set_record(((List<Record>)this.patient.records)[e.RowIndex]);
        }

        //向处方中添加一味药品
        private void add_item(Medicine medicine, decimal amount)
        {
            Prescription pre;
            BindingSource bs;
            if (medicine.category==Category.中药)
            {
                pre = this.record.cprescription;
                bs = itemBindingSource;
            }
            else
            {
                pre = this.record.wprescription;
                bs = itemBindingSource1;
            }
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
                    bs.ResetBindings(false);
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
                //pre.items.Add(item);
                bs.Add(item);
                //pre.price += item.小计;
            }
            db.SaveChanges();
            //set_prescriptions();
        }

        //修改主诉自动更新推荐模板
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_recommendations();
        }

        //双击模板导入
        private void dataGridView5_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            Template t = (Template)dataGridView5.Rows[e.RowIndex].DataBoundItem;
            //将模板中的每一味药添加到中药处方即可
            foreach (TemplateItem titem in t.items)
            {
                add_item(titem.medicine, titem.数量);
            }
            //db.SaveChanges();
            set_prescriptions();
            MessageBox.Show("导入成功！", "提示信息");
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
            //计算处方总金额
            db.SaveChanges();
            decimal wtotal = 0, ctotal = 0;
            foreach(Item item in this.record.cprescription.items)
            {
                ctotal += item.数量 * item.单价;
            }
            this.record.cprescription.price = ctotal;
            foreach (Item item in this.record.wprescription.items)
            {
                wtotal += item.数量 * item.单价;
            }
            this.record.wprescription.price = wtotal;
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

        //打开新窗口查看历史病历
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.patient == null)
            {
                MessageBox.Show("请先选择病人！", "提示信息");
            }
            else
            {
                RecordsForm rf = new RecordsForm(this.patient);
                rf.ShowDialog();
                if (rf.selectedRecord != null)
                {
                    //复制选中的病历并新建（深拷贝）
                    Record r = new Record();
                    rf.selectedRecord.patient.records.Add(r);
                    r.patient = rf.selectedRecord.patient;
                    r.类型 = "复诊";
                    r.科别 = rf.selectedRecord.科别;
                    r.就诊日期 = DateTime.Now;
                    r.主诉 = rf.selectedRecord.主诉;
                    r.现病史 = rf.selectedRecord.现病史;
                    r.既往史 = rf.selectedRecord.既往史;
                    r.个人史 = rf.selectedRecord.个人史;
                    r.家族史 = rf.selectedRecord.家族史;
                    r.月经及婚育史 = rf.selectedRecord.月经及婚育史;
                    r.体格检查 = rf.selectedRecord.体格检查;
                    r.辅助检查 = rf.selectedRecord.辅助检查;
                    r.诊断 = rf.selectedRecord.诊断;
                    r.治疗意见 = rf.selectedRecord.治疗意见;
                    r.医嘱 = rf.selectedRecord.医嘱;
                    r.服用方法 = rf.selectedRecord.服用方法;
                    r.amount = rf.selectedRecord.amount;
                    //复制中成药处方
                    r.wprescription = new Prescription();
                    //r.wprescription.record = r;
                    r.wprescription.price = rf.selectedRecord.wprescription.price;
                    foreach (Item litem in rf.selectedRecord.wprescription.items)
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
                    //复制中药处方
                    r.cprescription = new Prescription();
                    //r.cprescription.record = r;
                    r.cprescription.price = rf.selectedRecord.cprescription.price;
                    foreach (Item litem in rf.selectedRecord.cprescription.items)
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
                    set_record(r);
                    MessageBox.Show("复制成功！", "提示信息");
                }
            }
        }

        //打开编辑全部病历信息窗口
        private void button5_Click(object sender, EventArgs e)
        {
            RecordDetailForm rdf = new RecordDetailForm(this.record);
            rdf.ShowDialog();
            db.SaveChanges();
        }

        ////修改简拼，更新候选药品列表
        //private void textBox54_TextChanged(object sender, EventArgs e)
        //{
        //    if (textBox54.Text == "")
        //    {
        //        medicineBindingSource.DataSource = null;
        //    }
        //    else
        //    {
        //        medicineBindingSource.DataSource = db.medicine_set.Where(m => m.简拼.StartsWith(textBox54.Text)).ToList();
        //    }
        //}

        ////双击添加药品
        //private void dataGridView2_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (this.record != null)
        //    {
        //        add_item((Medicine)dataGridView2.Rows[e.RowIndex].DataBoundItem, 1);
        //        MessageBox.Show("添加成功！", "提示信息");
        //    }
        //}

        ////回车添加药品
        //private void textBox54_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if(e.KeyCode==Keys.Enter && this.record != null && dataGridView2.RowCount>0)
        //    {
        //        add_item((Medicine)dataGridView2.Rows[0].DataBoundItem, 1);
        //        MessageBox.Show("添加成功！", "提示信息");
        //    }
        //}

        //关闭窗口自动保存
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (db != null)
            {
                db.SaveChanges();
            }
        }

        //bug，切换tab页时手动保存所有numericUpDown的值
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.record != null)
            {
                this.patient.年龄 = numericUpDown1.Value;
                this.patient.体重 = numericUpDown2.Value;
                this.record.amount = (int)numericUpDown3.Value;
            }
        }

        //--------------------------添加中药-----------------------------
        //显示添加中药弹出窗口
        private void dataGridView3_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell.ColumnIndex == 0)
            {
                System.Drawing.Rectangle r = dgv.GetCellDisplayRectangle(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex, false);
                int x = tableLayoutPanel3.Location.X + groupBox2.Location.X + dataGridView3.Location.X + r.Left;
                int y = tableLayoutPanel3.Location.Y + groupBox2.Location.Y + dataGridView3.Location.Y + r.Top;
                panel1.Location = new System.Drawing.Point(x, y);
                panel1.Size = new System.Drawing.Size(r.Right - r.Left, panel1.Size.Height);
                panel1.Visible = true;
                textBox7.Focus();                
            }
        }
        //文字改变，筛选药品
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                listBox1.DataSource = null;
            }
            else
            {
                listBox1.DataSource = db.medicine_set.Where(m => m.category == Category.中药 && (m.简拼.Contains(textBox7.Text) || m.名称.Contains(textBox7.Text))).ToList();
            }
        }
        //上下键选择药品，回车添加，ESC退出
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (listBox1.SelectedIndex != -1 && listBox1.SelectedIndex > 0)
                        listBox1.SelectedIndex -= 1;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    if (listBox1.SelectedIndex != -1 && listBox1.SelectedIndex < listBox1.Items.Count - 1)
                        listBox1.SelectedIndex += 1;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                    if (listBox1.SelectedIndex != -1)
                    {
                        Medicine m = (Medicine)listBox1.SelectedItem;
                        add_item(m, 1);
                        //set_prescriptions();
                        panel1.Visible = false;
                        textBox7.Text = "";
                    }
                    break;
                case Keys.Escape:
                    panel1.Visible = false;
                    textBox7.Text = "";
                    break;
            }
        }
        //点击药品添加
        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != -1)
            {
                Medicine m = (Medicine)listBox1.SelectedItem;
                add_item(m, 1);
                //set_prescriptions();
                panel1.Visible = false;
                textBox7.Text = "";
            }
        }
        //焦点离开，隐藏弹出窗口
        private void panel1_Leave(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        //--------------------------添加中成药-----------------------------
        //显示添加中成药弹出窗口
        private void dataGridView4_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell.ColumnIndex == 0)
            {
                System.Drawing.Rectangle r = dgv.GetCellDisplayRectangle(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex, false);
                int x = tableLayoutPanel3.Location.X + groupBox5.Location.X + dataGridView4.Location.X + r.Left;
                int y = tableLayoutPanel3.Location.Y + groupBox5.Location.Y + dataGridView4.Location.Y + r.Bottom - panel3.Size.Height;
                panel3.Location = new System.Drawing.Point(x, y);
                panel3.Size = new System.Drawing.Size(r.Right - r.Left, panel3.Size.Height);
                panel3.Visible = true;
                textBox8.Focus();
            }
        }
        //文字改变，筛选药品
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (textBox8.Text == "")
            {
                listBox2.DataSource = null;
            }
            else
            {
                listBox2.DataSource = db.medicine_set.Where(m => m.category == Category.中成药 && (m.简拼.Contains(textBox8.Text) || m.名称.Contains(textBox8.Text))).ToList();
            }
        }
        //上下键选择药品，回车添加，ESC退出
        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (listBox2.SelectedIndex != -1 && listBox2.SelectedIndex > 0)
                        listBox2.SelectedIndex -= 1;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    if (listBox2.SelectedIndex != -1 && listBox2.SelectedIndex < listBox2.Items.Count - 1)
                        listBox2.SelectedIndex += 1;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                    if (listBox2.SelectedIndex != -1)
                    {
                        Medicine m = (Medicine)listBox2.SelectedItem;
                        add_item(m, 1);
                        //set_prescriptions();
                        panel3.Visible = false;
                        textBox8.Text = "";
                    }
                    break;
                case Keys.Escape:
                    panel3.Visible = false;
                    textBox8.Text = "";
                    break;
            }
        }
        //点击药品添加
        private void listBox2_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listBox2.IndexFromPoint(e.Location);
            if (index != -1)
            {
                Medicine m = (Medicine)listBox2.SelectedItem;
                add_item(m, 1);
                //set_prescriptions();
                panel3.Visible = false;
                textBox8.Text = "";
            }
        }
        //焦点离开，隐藏弹出窗口
        private void panel3_Leave(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }

        //删除中药
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0 && e.RowIndex < dataGridView3.RowCount-1)
            {
                if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    itemBindingSource.RemoveAt(e.RowIndex);
                }
            }
        }

        //删除西药
        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0 && e.RowIndex < dataGridView4.RowCount - 1)
            {
                if (MessageBox.Show("是否确认删除？", "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    itemBindingSource1.RemoveAt(e.RowIndex);
                }
            }
        }
    }
}
