using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BK_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region 三分类接口
        private void button1_Click(object sender, EventArgs e)
        {
            SFLReportMain f = new SFLReportMain();

            f.ShowDialog();
        }
        #endregion
        #region 特定蛋白接口
        private void button2_Click(object sender, EventArgs e)
        {

        }
        #endregion

    }
}
