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
    public partial class RecordsForm : Form
    {
        Patient patient;
        Record record;
        public Record selectedRecord = null;
        public RecordsForm(Patient p)
        {
            InitializeComponent();
            //避免出现水平滚动条
            tableLayoutPanel9.HorizontalScroll.Enabled = true;
            tableLayoutPanel9.AutoScroll = true;
            this.patient = p;
        }

        private void RecordsForm_Load(object sender, EventArgs e)
        {
            //病历列表
            recordBindingSource1.DataSource = this.patient.records;
            //选中最后一个病历
            dataGridView2.Rows[dataGridView2.RowCount - 1].Selected = true;
            dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.RowCount - 1;
            //病历内容
            Record r = this.patient.records.Last();
            this.record = r;
            recordBindingSource.DataSource = r;
            //处方
            itemBindingSource.DataSource = r.cprescription.items;
            dataGridView4.ClearSelection();
            itemBindingSource1.DataSource = r.wprescription.items;
            dataGridView3.ClearSelection();
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //病历内容
            Record r = this.patient.records.ElementAt(e.RowIndex);
            this.record = r;
            recordBindingSource.DataSource = r;
            //处方
            itemBindingSource.DataSource = r.cprescription.items;
            dataGridView4.ClearSelection();
            itemBindingSource1.DataSource = r.wprescription.items;
            dataGridView3.ClearSelection();
        }

        //选中病历则返回
        private void button10_Click(object sender, EventArgs e)
        {
            this.selectedRecord = this.record;
            this.Close();
        }
    }
}
