using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AC.AvalonControlsLibrary;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using AC.AvalonControlsLibrary.Controls;
using System.Windows.Forms;



namespace StormFrequencyCalculator
{
    /// <summary>
    /// Interaction logic for WPFStormControl.xaml
    /// </summary>
    public partial class WPFStormControl : System.Windows.Controls.UserControl
    {

        Dictionary<string, string> dicDates;
        string _strPointType = "PIXEL";

        public WPFStormControl()
        {
            InitializeComponent();
            FillCombo();
        }

        private void FillCombo()
        {
            try
            {
                IMap pmap = ArcMap.Document.ActiveView.FocusMap;
                int i = 0;
                IFeatureLayer player;

                IStandaloneTableCollection pStTabColl = pmap as IStandaloneTableCollection;
                pStTabColl.RemoveAllStandaloneTables();
                
                while (i <= pmap.LayerCount - 1)
                {
                    player = pmap.get_Layer(i) as IFeatureLayer;
                   
                    if ((player is IFeatureLayer) && (player.Valid))
                    {
                        if (player.Name == "Nearest Rain Pixels")
                        {
                            pmap.DeleteLayer(player);
                            continue;
                        }
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
                System.Windows.MessageBox.Show(ex.StackTrace);
            }
        }

        private void cboPointLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

           

            cboLayerFields.Items.Clear();
            IFeatureLayer pPointLayer = FindLayer(cboPointLayer.SelectedValue.ToString());

            IFeatureSelection pFeatSel = pPointLayer as IFeatureSelection;
            if (pFeatSel.SelectionSet.Count == 0)
            {
                System.Windows.MessageBox.Show("This layer contains no selection.", "Can't Continue..", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            for (int i = 0; i <= pPointLayer.FeatureClass.Fields.FieldCount - 1; i++)
            {
                cboLayerFields.Items.Add(pPointLayer.FeatureClass.Fields.Field[i].Name);
            }
          


            if (ConfirmRGNUMField(pPointLayer))
            {
                cboRainData.SelectedIndex = 2;
            }
            else
            {
                cboRainData.SelectedIndex = 0;
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
            try
            {
                IMap pmap = ArcMap.Document.ActiveView.FocusMap;
                int i = 0;

                while (i <= pmap.LayerCount - 1)
                {
                    if ((pmap.get_Layer(i).Name.ToUpper()) == (aLayerName.ToUpper()))
                    {
                        return pmap.get_Layer(i) as IFeatureLayer;
                    }
                    i++;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
            if (cboPointLayer.SelectedValue == null)
            {
                System.Windows.MessageBox.Show("No Point Layer Selected", "Can't Continue..", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (dicDates == null)
            {
                dicDates = new Dictionary<string, string>();
                dicDates.Add(dtStart.CurrentlySelectedDate.ToShortDateString(), dtEnd.CurrentlySelectedDate.ToShortDateString());
                dtEnd.IsEnabled = true;
                dtStart.IsEnabled = true;
            }


            foreach (KeyValuePair<string, string> k in dicDates)
            {
                int index = k.Key.IndexOf("_");
                string keyvalStripped = (index > 0 ? k.Key.Substring(0, index) : "");
                if (index != -1)
                {
                    if (Convert.ToDateTime(k.Value) < Convert.ToDateTime(keyvalStripped))
                    {
                        System.Windows.MessageBox.Show("Ending date: " + k.Value + " is before the Start date: " + keyvalStripped, "Can't Continue..", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (Convert.ToDateTime(k.Value) == Convert.ToDateTime(keyvalStripped))
                    {
                        System.Windows.MessageBox.Show("Starting date: " + k.Value + " is the same as End date: " + keyvalStripped, "Can't Continue..", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    if (Convert.ToDateTime(k.Value) < Convert.ToDateTime(k.Key))
                    {
                        System.Windows.MessageBox.Show("Starting date: " + k.Value + " is the same as End date: " + k.Key, "Can't Continue..", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            string strCBORAINVal = cboRainData.Text.ToUpper();
            switch (strCBORAINVal)
            {
                case "REAL TIME":
                    _strPointType = "PIXEL";
                    break;

                case "CORRECTED (EOM)":
                    _strPointType = "CORRECTED";
                    break;

                case "RAIN GUAGE":
                    _strPointType = "GUAGE";
                    break;
            }


            clsStormCalc Storm = new clsStormCalc(_strPointType);
            Storm.MakeStorm(_strPointType, cboPointLayer.SelectedValue.ToString(), dicDates, cboLayerFields.SelectedValue.ToString());

                this.FormsWindow.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void dtStart_SelectedDateChanged_1(object sender, DateSelectedChangedRoutedEventArgs e)
        {
            DatePicker dtPicker = sender as DatePicker;
            if (txtStart.IsFocused)
            {
                return;
            }
            txtStart.Text = dtPicker.CurrentlySelectedDate.ToShortDateString();
        }


        private void txtStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DateTime dt = DateTime.Parse(txtStart.Text);
                dtStart.CurrentlySelectedDate = dt;
            }
            catch { }
        }

        private void dtEnd_SelectedDateChanged(object sender, DateSelectedChangedRoutedEventArgs e)
        {
            DatePicker dtPicker = sender as DatePicker;
            if (txtEnd.IsFocused)
            {
                return;
            }
            txtEnd.Text = dtPicker.CurrentlySelectedDate.ToShortDateString();
        }

        private void txtEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DateTime dt = DateTime.Parse(txtEnd.Text);
                dtEnd.CurrentlySelectedDate = dt;
            }
            catch { }
        }

        public Form FormsWindow { get; set; }

        private void chkUseBatch_Checked(object sender, RoutedEventArgs e)
        {
            string line = "";
            try
            {
                if (chkUseBatch.IsChecked == true)
                {
                    dtEnd.IsEnabled = false;
                    dtStart.IsEnabled = false;
                    txtStart.IsEnabled = false;
                    txtEnd.IsEnabled = false;

                    Microsoft.Win32.OpenFileDialog dlgOpenFile = new Microsoft.Win32.OpenFileDialog();
                    dlgOpenFile.Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";


                    if (dlgOpenFile.ShowDialog() == true)
                    {
                        dicDates = new Dictionary<string, string>();
                        int counter = 0;
                       
                        System.IO.StreamReader file = new System.IO.StreamReader(dlgOpenFile.FileName);
                       
                        while ((line = file.ReadLine()) != null)
                        {
                            string[] aryDates = line.Split(',');
                            DateTime dtOUT;
                            if ((DateTime.TryParse(aryDates[0], out dtOUT) && DateTime.TryParse(aryDates[1], out dtOUT)) && (aryDates.Length == 2))
                            {
                                dicDates.Add(aryDates[0] + "_" + counter.ToString(), aryDates[1]);
                                counter++;
                            }
                            else
                            {
                                file.Close();
                                throw new Exception();
                               
                            }
                          
                        }

                        file.Close();
                        System.Windows.MessageBox.Show (counter.ToString() + " dates successfully loaded.");

                    }
                    else
                    {
                        chkUseBatch.IsChecked = false;
                        dicDates = null;
                        dtEnd.IsEnabled = true;
                        dtStart.IsEnabled = true;
                        txtStart.IsEnabled = true;
                        txtEnd.IsEnabled = true;
                    }

                }

                else
                {
                    dicDates = null;
                    dtEnd.IsEnabled = true;
                    dtStart.IsEnabled = true;
                    txtStart.IsEnabled = true;
                    txtEnd.IsEnabled = true;
                }
            }

            catch
            {
                dicDates = null;
                dtEnd.IsEnabled = true;
                dtStart.IsEnabled = true;
                chkUseBatch.IsChecked = false;
                txtStart.IsEnabled = true;
                txtEnd.IsEnabled = true;
                System.Windows.MessageBox.Show("Text file format appears to be invalid. Line: " + line);
            }
        }


        private void chkUseBatch_Unchecked(object sender, RoutedEventArgs e)
        {
            dicDates = null;
            dtEnd.IsEnabled = true;
            dtStart.IsEnabled = true;
            txtStart.IsEnabled = true;
            txtEnd.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frmTxtInfo frm = new frmTxtInfo();
            frm.ShowDialog();
        }

       
    }
}
