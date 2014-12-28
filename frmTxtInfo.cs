using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StormFrequencyCalculator
{
    public partial class frmTxtInfo : Form
    {
        public frmTxtInfo()
        {
            InitializeComponent();
        }

        private void frmTxtInfo_Load(object sender, EventArgs e)
        {
            textBox1.Text = @"Calculations can be ran for the point layer across multiple dates by loading the dates into a text file.  Place each start and end date on a line, delimited by a comma as seen in the following example:" + Environment.NewLine + @"8/1/2009,8/5/2009" + Environment.NewLine + @"9/20/2008,9/25/2008" + Environment.NewLine + @"4/25/2011,4/28/2011";
        }
    }
}
