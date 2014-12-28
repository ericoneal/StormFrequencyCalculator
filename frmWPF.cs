using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration; 


namespace StormFrequencyCalculator
{
    public partial class frmWPF : Form
    {
        public frmWPF()
        {
            try
            {
                InitializeComponent();

                var ctr = (elementHost1.Child as WPFStormControl);
                if (ctr != null)
                    ctr.FormsWindow = this;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void frmWPF_Load(object sender, EventArgs e)
        {

        }

    }
}
