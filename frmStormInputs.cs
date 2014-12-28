using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace StormFrequencyCalculator
{
    public partial class frmStormInputs : Form
    {

        Dictionary<string, string> dicDates;
        string _strPointType = "PIXEL";

        public frmStormInputs()
        {
            InitializeComponent();
          
        }


        private void FillCombo()
        {
            try
            {
                IMap pmap = ArcMap.Document.ActiveView.FocusMap;
                int i = 0;
                IFeatureLayer player;

                while (i <= pmap.LayerCount - 1)
                {
                    player = pmap.get_Layer(i) as IFeatureLayer;
                    if ((player is IFeatureLayer) && (player.Valid))
                    {
                        if (player is IGroupLayer)
                        {
                            ICompositeLayer pCompLayer = player as ICompositeLayer;
                            for (int k = 0; k <= (pCompLayer.Count - 1); k++)
                            {
                                if (pCompLayer is IFeatureLayer)
                                {
                                    IFeatureLayer p = pCompLayer as IFeatureLayer;
                                    if (p.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                                    {
                                        cboPointLayer.Items.Add(p.Name);
                                    }
                                }
                            }
                        }
                        else if ((player is IFeatureLayer) && (player.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint) && (player.Valid))
                        {
                            cboPointLayer.Items.Add(player.Name);
                        }
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }


       

        private void frmStormInputs_Load(object sender, EventArgs e)
        {
            //dtEnd.MaxDate = DateTime.Now;
            //dtStart.MaxDate = DateTime.Now;
            FillCombo();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {

            if (cboPointLayer.Text == "")
            {
                MessageBox.Show("No Point Layer Selected", "Can't Continue..", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dicDates == null)
            {
                dicDates = new Dictionary<string, string>();
                dicDates.Add(dtStart.Value.ToShortDateString(), dtEnd.Value.ToShortDateString());
                dtEnd.Enabled = true;
                dtStart.Enabled = true;
            }


            foreach (KeyValuePair<string, string> k in dicDates)
            {
                if (Convert.ToDateTime(k.Value) < Convert.ToDateTime(k.Key))
                {
                    MessageBox.Show("Ending date: " + k.Value + " is before the Start date: " + k.Key, "Can't Continue..", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            clsStormCalc Storm = new clsStormCalc(_strPointType);
            Storm.MakeStorm(_strPointType, cboPointLayer.Text, dicDates, cboLayerFields.Text);
            this.Close();

        }


        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                // System.Drawing.Drawing2D.LinearGradientBrush baseBackground = (New (Point(0, 0)) New (Point(ClientSize.Width, 0)), Color.Gray, Color.RoyalBlue);
                using (Brush b = new
                           System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, 0), new
                           System.Drawing.Point(this.ClientSize.Width, this.ClientSize.Height), 
                           Color.RoyalBlue, Color.Silver))
                    e.Graphics.FillRectangle(b, ClientRectangle);
            }
            catch { }

        }

        private void chkUseBatch_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkUseBatch.Checked)
                {
                    dtEnd.Enabled = false;
                    dtStart.Enabled = false;



                    if (dlgOpenFile.ShowDialog() == DialogResult.OK)
                    {
                        dicDates = new Dictionary<string, string>();
                        int counter = 0;
                        string line;
                        System.IO.StreamReader file = new System.IO.StreamReader(dlgOpenFile.FileName);
                        while ((line = file.ReadLine()) != null)
                        {
                            string[] aryDates = line.Split(',');
                            dicDates.Add(aryDates[0], aryDates[1]);
                            counter++;
                        }

                        file.Close();


                    }
                    else
                    {
                        chkUseBatch.Checked = false;
                    }

                }

                else
                {
                    dicDates = new Dictionary<string, string>();
                    dicDates.Add(dtStart.Value.ToShortDateString(), dtEnd.Value.ToShortDateString());
                    dtEnd.Enabled = true;
                    dtStart.Enabled = true;
                   
                }
            }

            catch
            {
                dicDates = new Dictionary<string, string>();
                dicDates.Add(dtStart.Value.ToShortDateString(), dtEnd.Value.ToShortDateString());
                dtEnd.Enabled = true;
                dtStart.Enabled = true;
                chkUseBatch.Checked = false;
                MessageBox.Show("Text file format appears to be invalid.");
            }
        }

        private void btnTxtFileInfo_Click(object sender, EventArgs e)
        {
            frmTxtInfo frm = new frmTxtInfo();
            frm.ShowDialog();
        }

        private void radRealTime_CheckedChanged(object sender, EventArgs e)
        {
            if (radRealTime.Checked)
            {
                lblPointLayer.Text = "Input (Point) Layer:";
                _strPointType = "PIXEL";
            }
        }

        private void radMonthEnd_CheckedChanged(object sender, EventArgs e)
        {
            if (radMonthEnd.Checked)
            {
                lblPointLayer.Text = "Input (Point) Layer:";
                _strPointType = "CORRECTED";
            }
        }

        private void radGauge_CheckedChanged(object sender, EventArgs e)
        {
            if (radGauge.Checked)
            {
                lblPointLayer.Text = "Rain Gauge Layer:";
                _strPointType = "GUAGE";
            }
        }

        private void cboPointLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboLayerFields.Items.Clear();
            IFeatureLayer pPointLayer = FindLayer(cboPointLayer.Text);
            for (int i = 0; i <= pPointLayer.FeatureClass.Fields.FieldCount - 1; i++)
            {
                cboLayerFields.Items.Add(pPointLayer.FeatureClass.Fields.Field[i].Name);
            }

            if (ConfirmRGNUMField(pPointLayer))
            {
                radGauge.Enabled = true;
            }
            else
            {
                radGauge.Enabled = false;
            }

        }
        private bool ConfirmRGNUMField(IFeatureLayer pRGLayer)
        {
            IFeatureCursor ppointcurs = pRGLayer.FeatureClass.Search(null, false);
            IFeature ppointFeat = ppointcurs.NextFeature();
            if (ppointFeat.Fields.FindField("RGNUM") != -1)
            {
                return true;
            }
            return false;
        }

        public IFeatureLayer FindLayer(string aLayerName)
        {
            IMap pmap = ArcMap.Document.ActiveView.FocusMap;
            int i = 0;

            while (i <= pmap.LayerCount - 1)
            {
                if ((pmap.get_Layer(i).Name.ToUpper()) == (aLayerName.ToUpper()))

                    break;
                i++;
            }
            return pmap.get_Layer(i) as IFeatureLayer;
        }

        private void dtStart_ValueChanged(object sender, EventArgs e)
        {
            
            if ((radMonthEnd.Checked) && (dtStart.Value < DateTime.Parse("7/3/2008")))
            {
                MessageBox.Show("Corrected radar data ends at 7/3/2008");
            }
        }

    }
}
