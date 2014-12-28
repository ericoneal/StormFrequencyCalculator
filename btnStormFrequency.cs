using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using System.Windows;

namespace StormFrequencyCalculator
{
    class btnStormFrequency : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public bool isEnabled = true;
        protected override void OnClick()
        {

            IMap pmap = ArcMap.Document.FocusMap;
            if (pmap.SelectionCount == 0)
            {
                MessageBox.Show("There are currently no features selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            //frmStormInputs frm = new frmStormInputs();
            frmWPF frm = new frmWPF();
            frm.ShowDialog();

            ArcMap.Application.CurrentTool = null;
        }

        protected override void OnUpdate()
        {

            Enabled = isEnabled;
        }



    }
}
