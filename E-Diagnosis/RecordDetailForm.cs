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
    public partial class RecordDetailForm : Form
    {
        public RecordDetailForm(Record record)
        {
            InitializeComponent();
            recordBindingSource.DataSource = record;
        }

        //关闭窗口时切换焦点以使自动保存
        private void RecordDetailForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            textBox6.Focus();
            textBox7.Focus();
        }
    }
}
