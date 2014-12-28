using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace StormFrequencyCalculator
{
    public class StormFrequencyCalculator : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        public StormFrequencyCalculator()
        {

        }

        protected override void OnStartup()
        {
            btnStormFrequency btn = (btnStormFrequency)ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID<btnStormFrequency>(ThisAddIn.IDs.btnStormFrequency);
            btn.isEnabled = true;
        }

        private void WireDocumentEvents()
        {
            

        }

       

    }

}
