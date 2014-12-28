using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.esriSystem;
using System.Data.OracleClient;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geoprocessing;
using Microsoft.Win32;
using System.Diagnostics;
using iTextSharp.text.pdf;
using System.Runtime.InteropServices;

namespace StormFrequencyCalculator
{
    class clsStormCalc
    {

        //StreamWriter sw;
        clsDepthChart cDepthChart;
        IStepProgressor _pStepProgressor;
        IProgressDialog2 _pProgressDialog;
        IRowBuffer _rowBuffer;
        ICursor _cursor;
        OracleConnection _connection;
  
        string _strPointType;
        ITable _ptable;
        string _strStormTableName;
        string _strStormTableDisplayName;
        Dictionary<string, string> dic7DayAntecedent;




        IMapDocument mapdoc;
        /* GDI delegate to GetDeviceCaps function */
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);

        /* User32 delegates to getDC and ReleaseDC */
        [DllImport("User32.dll")]
        public static extern int GetDC(int hWnd);

        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);

        //[DllImport("user32.dll", SetLastError = true)]
        //static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        /* constants used for user32 calls */
        const uint SPI_GETFONTSMOOTHING = 74;
        const uint SPI_SETFONTSMOOTHING = 75;
        const uint SPIF_UPDATEINIFILE = 0x1;

        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern int GetDesktopWindow();



        public clsStormCalc(string PointType)
        {
              cDepthChart = new clsDepthChart();
              _strPointType = PointType;
              
        }

        public void MakeStorm(string strPointType, string strPointLayerName, Dictionary<string, string> dicDates, string strLabelField)
        {

            try{
           

                //Get the point feature layer
            IFeatureLayer pPointLayer = FindLayer(strPointLayerName);
            _connection = new OracleConnection("");

                //Rain Gages are in a different database
            if (_strPointType == "GUAGE")
            {
                _connection = new OracleConnection("");
                if (!ConfirmRGNUMField(pPointLayer))
                {
                    MessageBox.Show("RGNUM Field not present");
                    return;
                }
            }



                //Prepare the location map mxd, open it only once to save memory
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\LocationMap.mxd", StormFrequencyCalculator.Properties.Resources.LocationMap);
            string strMXD = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\LocationMap.mxd";

            mapdoc = new MapDocumentClass();
            mapdoc.Open(strMXD);

       



            IProgressDialogFactory pProgressDlgFact = new ProgressDialogFactoryClass();
            ITrackCancel pTrackCancel = new CancelTrackerClass();
            _pProgressDialog = pProgressDlgFact.Create(pTrackCancel, 0) as IProgressDialog2;

          
                
                _connection.Open();

              
             //Get the raingrid points from SDE
                IFeatureLayer pPixelLayer = GetSDELayer("msddata.raingrid_point");
              





                //Show the progress dialog
                _pStepProgressor = _pProgressDialog as IStepProgressor;
                _pProgressDialog.CancelEnabled = false;
                _pProgressDialog.Title = "Storm Frequency Calculator";
                _pProgressDialog.Animation = esriProgressAnimationTypes.esriProgressGlobe;
                _pStepProgressor.MinRange = 0;
                _pStepProgressor.MaxRange = pPointLayer.FeatureClass.FeatureCount(null);
                _pStepProgressor.StepValue = 1;
                _pProgressDialog.ShowDialog();


                //If using OneRain data, get the closest rain pixels and make a feature layer for them
                IFeatureLayer pOutpoints = null;
                if ((_strPointType == "PIXEL") || (_strPointType == "CORRECTED"))
                {
                    //OUT Layer, holding values
                    pOutpoints = new FeatureLayerClass();
                    pOutpoints.FeatureClass = MakeOUTFeatureClass(pPointLayer);
                    pOutpoints.Name = "Nearest Rain Pixels";
                    ArcMap.Document.ActiveView.FocusMap.AddLayer(pOutpoints);
                    pOutpoints.Visible = false;

                    GetClosestPixel(pPointLayer, pPixelLayer, pOutpoints);
                }


                //Make an ITable to hold the data
                _ptable = MakeCalcTable();

                string strMergedPDF;
                IStandaloneTable pStandAloneTable = null;
                List<string> lstPDFFileNames = new List<string>();

                dic7DayAntecedent = new Dictionary<string, string>();

                string strStart = "";
                string strEnd = "";
                IFeatureLayer pPDFLayer = new FeatureLayerClass();
                pPDFLayer.Name = "OutPoints";

                //Loop thru the date sets
                foreach (KeyValuePair<string, string> k in dicDates)
                {
                    int index = k.Key.IndexOf("_");
                    if (index == -1)
                    {
                        strStart = k.Key;
                        strEnd = k.Value;
                    }
                    else
                    {
                        strStart = (index > 0 ? k.Key.Substring(0, index) : "");
                        strEnd = k.Value;
                      
                    }
                    switch (_strPointType)
                    {
                        case "PIXEL":
                            FillLayerTable(pOutpoints, strStart, strEnd);
                            pPDFLayer.FeatureClass = pOutpoints.FeatureClass;
                            break;

                        case "CORRECTED":
                            FillLayerTable(pOutpoints, strStart, strEnd);
                            pPDFLayer.FeatureClass = pOutpoints.FeatureClass;
                            break;

                        case "GUAGE":
                            FillLayerTable(pPointLayer, strStart, strEnd);
                            pPDFLayer.FeatureClass = pPointLayer.FeatureClass;
                            break;
                    }




                    //Show the standalone table
                    pStandAloneTable = AddTable(_ptable);

                    //using a model to join the results to the original featureclass
                    DoModelJoin(_ptable, pPointLayer);

                    //Make the report page and add it to the collection to be merged into 1 pdf
                    clsReport Report = new clsReport();
                    strMergedPDF = Report.MakeReport("", pPointLayer, pPDFLayer, pStandAloneTable, _strPointType, strStart, strEnd, strLabelField, mapdoc);
                    lstPDFFileNames.Add(strMergedPDF);
              

                }

                //Merge all pdf pages and show the pdf
                string strFinalPDFFilename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf");
                MergePdfFiles(lstPDFFileNames, strFinalPDFFilename);

                Process p = new Process();
                p.StartInfo.FileName = strFinalPDFFilename;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                p.Start();


                _pProgressDialog.HideDialog();

                MessageBox.Show("Raw data stored in standalone table: " + pStandAloneTable.Name + " in this project.  MAXSTORM(s) are selected in this table.", "Storm Calculator", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //ShowTable(pStandAloneTable);

              


            }

            catch(Exception ex) {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                _pProgressDialog.HideDialog();
                _connection.Close();
            }

        }



       

        //private void ExportTable()
        //{
        //    IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;

        //    ITableSelection pTableSel = _ptable as ITableSelection;
        //    ISelectionSet pSelSet = pTableSel.SelectionSet;

        //    IDataset pDataset = _ptable as IDataset;
        //    ITableName pInTableName = pDataset.FullName as ITableName;
        //    IDatasetName pInDsName = pInTableName as IDatasetName;

        //    ITableName pOutTableName = new TableNameClass();
        //    IDatasetName pOutDSName = pOutTableName as IDatasetName;
        //    pOutDSName.Name = "TestExport.dbf";

        //    IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
        //    pWorkspaceName.PathName = Defaultgdb.DefaultDatabaseName.PathName;
        //    pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesGDB.FileGDBWorkspaceFactory";
        //    pOutDSName.WorkspaceName = pWorkspaceName;

        //    IExportOperation pExportOp = new ExportOperationClass();
        //    pExportOp.ExportTable(pInDsName, null, pSelSet, pOutDSName, 0);

              

        //}

        private void DoModelJoin(ITable ptable, IFeatureLayer pPointLayer)
        {

            try
            {
                ESRI.ArcGIS.esriSystem.IPropertySet pPropSet = new PropertySet();

                IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;
                IGeoProcessor gp = new ESRI.ArcGIS.Geoprocessing.GeoProcessor();

                //RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\\LOJICReg\\MSD");
                //string strSupportFilePath = regkey.GetValue("SupportFiles").ToString() + @"\AddinsSupportFiles\StormFrequencyCalculator\";

                //gp.AddToolbox(strSupportFilePath + "StormCalculator.tbx");
          
                //Check to see if Toolbox is already there and add if not
                IGpEnumList gpList =    gp.ListToolboxes("*");
                string fc = gpList.Next();
                bool isToolboxThere = false;
                 while (fc != "")
                 {
                     if (fc.ToUpper() == "STORMCALCULATOR")
                     {
                         isToolboxThere = true;
                         break;
                     }
                     fc = gpList.Next();
                     System.Diagnostics.Debug.WriteLine(fc);
                 }

                 if (isToolboxThere == false)
                 {
                     gp.AddToolbox(@"M:\prod\MSD\AddinsSupportFiles\StormFrequencyCalculator\" + "StormCalculator.tbx");
                 }

                gp.OverwriteOutput = true;
                
                IVariantArray gpParams = new ESRI.ArcGIS.esriSystem.VarArray();

                string str = pPointLayer.FeatureClass.OIDFieldName;

                IField pField = pPointLayer.FeatureClass.Fields.get_Field(pPointLayer.FeatureClass.Fields.FindField(pPointLayer.FeatureClass.OIDFieldName));



                gpParams.Add(_strStormTableDisplayName);
                gpParams.Add(pPointLayer.Name);
                gpParams.Add(_strStormTableName + ".MAXStorm IS NOT null");
                gpParams.Add(pField);


                // Execute the tool.
                IGeoProcessorResult gpResult = new GeoProcessorResult();
                gpResult = gp.Execute("Model", gpParams, null);

            }

            catch (Exception ex)
            {

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
  

        private void FillLayerTable(IFeatureLayer pDataLayer, string strStart, string strEnd)
        {
           

            try
            {

                IFeatureClass pFClass = pDataLayer.FeatureClass;
                IFeatureCursor ppointcurs = pFClass.Search(null, false);
                IFeature ppointFeat = ppointcurs.NextFeature();

                clsExport Export = new clsExport();
               


                int i = 1;
                int iPointID = 0;
                string strPointLocation = "";
                while (ppointFeat != null)
                {
                    _pProgressDialog.Description = "Calculating data for date interval " + strStart + " - " + strEnd;
                    _pStepProgressor.Step();

              
                 
                    switch (_strPointType)
                    {
                        case "PIXEL":
                            strPointLocation = ppointFeat.get_Value(ppointFeat.Fields.FindField("PIXEL")).ToString();
                            iPointID = Convert.ToInt32(ppointFeat.get_Value(ppointFeat.Fields.FindField("PointID")));
                            break;
                        case "CORRECTED":
                            strPointLocation = ppointFeat.get_Value(ppointFeat.Fields.FindField("PIXEL")).ToString();
                            iPointID = Convert.ToInt32(ppointFeat.get_Value(ppointFeat.Fields.FindField("PointID")));
                            break;
                        case "GUAGE":
                            strPointLocation = ppointFeat.get_Value(ppointFeat.Fields.FindField("RGNUM")).ToString();
                            iPointID = Convert.ToInt32(ppointFeat.OID);
                            break;
                    }

                   


                    Dictionary<int, Dictionary<DateTime, double>> dicDateTotals = new Dictionary<int, Dictionary<DateTime, double>>();
                    dicDateTotals = GetRainTotals(strPointLocation, iPointID, strStart, strEnd);

                    //DateTime dtStartPeriod = DateTime.Parse(strStart);
                    //double dblSevenDayAtecedent = Calc_SevenDayAntecedent(strPointLocation, dtStartPeriod);

                
                 
                    List<double> lstStormCalcs = new List<double>();

                    if (dicDateTotals != null)
                    {
                        foreach (KeyValuePair<int, Dictionary<DateTime, double>> rain in dicDateTotals)
                        {
                            int iHour = rain.Key;

                            Dictionary<DateTime, double> dicDateTimeInterval = new Dictionary<DateTime, double>();
                            dicDateTimeInterval = rain.Value;


                            DateTime dtDateTimeInterval = dicDateTimeInterval.Keys.ElementAt(0);
                            double dblRainTotal = dicDateTimeInterval.Values.ElementAt(0);

                            lstStormCalcs.Add(CalculateAndRecord("Atlas14", iHour, strPointLocation, iPointID, dtDateTimeInterval, dblRainTotal));
                            lstStormCalcs.Add(CalculateAndRecord("CloudBurst", iHour, strPointLocation, iPointID, dtDateTimeInterval, dblRainTotal));
                        }

                        //double dblMax = lstStormCalcs.Max();
                        RecordMaxStorm(strPointLocation, lstStormCalcs.Max());
                    }
                 
                    ppointFeat = ppointcurs.NextFeature();
                    //_connection.Close();
                    i++;
                }
                //sw.Close();
                _cursor.Flush();
                return;
            }



            catch(Exception ex) {
                MessageBox.Show(ex.Message);
               
            }
            finally
            {
               
            }
        }




        public double Calc_SevenDayAntecedent(string strPixel, DateTime dtStart)
        {

            try
            {
                DateTime dt7DayStart = dtStart.AddDays(-7);
                string strQuery = "";

                switch (_strPointType)
                {
                    case "PIXEL":
                         strQuery = @"SELECT     SUM(RAIN) AS RAINSUM
                                        FROM         NEXRAIN.PIXEL
                                        WHERE     (PIXEL = " + strPixel + @") AND (TIMESTAMP >= TO_DATE('" + dt7DayStart + @"', 'MM/DD/YYYY hh:MI:SS AM')) 
                                        AND (TIMESTAMP <= TO_DATE('" + dtStart + @"',  'MM/DD/YYYY hh:MI:SS AM'))";
                                if (_connection == null)
                                {
                                    _connection = new OracleConnection("");
                                    _connection.Open();
                                }
                        break;

                    case "CORRECTED":
                         strQuery = @"SELECT     SUM(RAIN) AS RAINSUM
                                        FROM         ONERAIN_EOM.PIXEL
                                        WHERE     (PIXEL = " + strPixel + @") AND (TIMESTAMP >= TO_DATE('" + dt7DayStart + @"', 'MM/DD/YYYY hh:MI:SS AM')) 
                                        AND (TIMESTAMP <= TO_DATE('" + dtStart + @"',  'MM/DD/YYYY hh:MI:SS AM'))";
                                if (_connection == null)
                                {
                                    _connection = new OracleConnection("");
                                    _connection.Open();
                                }
                        break;

                    case "GUAGE":
                       strQuery = @"SELECT        SUM(INTVALUE) AS RAINSUM
                                FROM            RAINFALL.RAINFALL
                                WHERE        (RGSITE = '" + strPixel + @"') AND (TO_DATE(EVENT_DATE || '  ' || EVENT_TIME, 'MM/DD/YYYY HH24:MI') >= TO_DATE('" + dt7DayStart + @"', 'MM/DD/YYYY hh:MI:SS AM')) 
                               AND (TO_DATE(EVENT_DATE || '  ' || EVENT_TIME, 'MM/DD/YYYY HH24:MI') <= TO_DATE('" + dtStart + @"', 'MM/DD/YYYY hh:MI:SS AM'))";
                            if (_connection == null)
                            {
                                _connection = new OracleConnection("");
                                _connection.Open();
                            }
                        break;
                }





              //  System.Diagnostics.Debug.Write(Environment.NewLine + strQuery + Environment.NewLine);



                using (OracleCommand command = new OracleCommand(strQuery, _connection))
                {
                   
                    using (OracleDataReader reader = command.ExecuteReader())
                    {


                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                string g = reader.GetValue(0).ToString();
                                if (reader.GetValue(0).ToString().Length == 0)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return Convert.ToDouble(reader.GetValue(0));
                                }
                            }

                        }


                    }
                }

                return 0;

            }

            catch (Exception ex)
            {
                return 0;
            }
           
        }




        public double Calc_PeriodTotal(string strPixel, DateTime dtStart, DateTime dtEnd)
        {

            try
            {
                DateTime dt7DayStart = dtStart.AddDays(-7);
                string strQuery = "";


                string DateFormat = "MM/dd/yyyy HH:mm";
                string strStartDate = dtStart.ToString(DateFormat);
                string strEndDate = dtEnd.ToString(DateFormat);

                switch (_strPointType)
                {
                    case "PIXEL":
                        strQuery = @"SELECT SUM(RAIN) as RAIN FROM NEXRAIN.PIXEL where timestamp >= to_date('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI') and timestamp <= to_date('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI') and (PIXEL = " + strPixel + @") GROUP BY PIXEL"; 
                        if (_connection == null)
                        {
                            _connection = new OracleConnection("");
                            _connection.Open();
                        }
                        break;

                    case "CORRECTED":
                        strQuery = @"SELECT SUM(RAIN) as RAIN FROM ONERAIN_EOM.PIXEL where timestamp >= to_date('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI') and timestamp <= to_date('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI') and (PIXEL = " + strPixel + @") GROUP BY PIXEL";
                        if (_connection == null)
                        {
                            _connection = new OracleConnection("");
                            _connection.Open();
                        }
                        break;

                    case "GUAGE":
                         strQuery = @"SELECT        SUM(INTVALUE) AS RAINSUM
                                FROM            RAINFALL.RAINFALL
                                WHERE        (RGSITE = '" + strPixel + @"') AND (TO_DATE(EVENT_DATE || '  ' || EVENT_TIME, 'MM/DD/YYYY HH24:MI') >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) 
                               AND (TO_DATE(EVENT_DATE || '  ' || EVENT_TIME, 'MM/DD/YYYY HH24:MI') <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI'))";
                            if (_connection == null)
                            {
                                _connection = new OracleConnection("");
                                _connection.Open();
                            }
                        break;
                }
               

                //System.Diagnostics.Debug.Write(Environment.NewLine + strQuery + Environment.NewLine);



                using (OracleCommand command = new OracleCommand(strQuery, _connection))
                {

                    using (OracleDataReader reader = command.ExecuteReader())
                    {


                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                string g = reader.GetValue(0).ToString();
                                if (reader.GetValue(0).ToString().Length == 0)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return Convert.ToDouble(reader.GetValue(0));
                                }
                            }

                        }


                    }
                }

                return 0;

            }

            catch (Exception ex)
            {
                return 0;
            }

        }


        private void RecordMaxStorm(string strPointLocation, double dblMax)
        {

            try
            {
                string strQuery = "";
                switch (_strPointType)
                {
                    case "PIXEL":
                        strQuery = @"PIXEL = '" + strPointLocation + "' AND CalcStorm = " + dblMax.ToString();
                        break;
                    case "CORRECTED":
                        strQuery = @"PIXEL = '" + strPointLocation + "' AND CalcStorm = " + dblMax.ToString();
                        break;
                    case "GUAGE":
                        strQuery = @"GUAGE = '" + strPointLocation + "' AND CalcStorm = " + dblMax.ToString();
                        break;
                }


               
                IQueryFilter pq = new QueryFilterClass();
                pq.WhereClause = strQuery;

                ICursor pcurs = _ptable.Update(pq, false);
                IRow prow = pcurs.NextRow();

                while (prow != null)
                {
                    prow.set_Value(prow.Fields.FindField("MAXStorm"), dblMax);
                    pcurs.UpdateRow(prow);
                    prow = pcurs.NextRow();
                }
             
                pcurs.Flush();
            }
            catch (Exception ex)
            {

            }

        }




        private Dictionary<int, Dictionary<DateTime, double>> GetRainTotals(string strPointLocation, int iPointID, string strStartDate, string strEndDate)
        {

            string strOracleQuery = "";

            Dictionary<int, Dictionary<DateTime, double>> dicMaster = new Dictionary<int, Dictionary<DateTime, double>>();
            Dictionary<DateTime, double> dicDateTotal;


            List<int> lstHours = new List<int>();
            lstHours.Add(1);
            lstHours.Add(3);
            lstHours.Add(6);
            lstHours.Add(12);
            lstHours.Add(24);
            lstHours.Add(48);

         

            foreach (int iHour in lstHours)
            {

                switch (_strPointType)
                {
                    case "PIXEL":
                        #region PIXEL Query

                        strOracleQuery = @"
SELECT        MIN(TIMESTAMP) AS TS, MAXOFSUMOFRAIN AS TOTAL
FROM            (SELECT        STEP2.TIMESTAMP, STEP6.MAXOFSUMOFRAIN
                          FROM            (SELECT        TO_CHAR(""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS TIMESTAMP, AVG(RAIN) AS RAIN
                                                    FROM            NEXRAIN.PIXEL
                                                    WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY hh24:MI')) AND (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY hh24:MI')) 
                                                                              AND (PIXEL = " + strPointLocation + @")
                                                    GROUP BY ""TIMESTAMP"") STEP2 INNER JOIN
                                                        (SELECT        STEP5.MAXOFSUMOFRAIN, STEP4_1.HR0, STEP4_1.HR" + iHour.ToString() + @"
                                                          FROM            (SELECT        MAX(SUMOFRAIN) AS MAXOFSUMOFRAIN
                                                                                    FROM            (SELECT        SUM(STEP2_2.RAIN) AS SUMOFRAIN, STEP3.HR0, STEP3.HR" + iHour.ToString() + @"
                                                                                                              FROM            (SELECT        TO_CHAR(""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS TIMESTAMP, AVG(RAIN) AS RAIN
                                                                                                                                        FROM            NEXRAIN.PIXEL RAINFALL_3
                                                                                                                                        WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                                  (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                                        GROUP BY ""TIMESTAMP"") STEP2_2 INNER JOIN
                                                                                                                                            (SELECT        TO_CHAR(0 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR0, TO_CHAR(1 / 24 + ""TIMESTAMP"", 
                                                                                                                                                                        'MM/DD/YYYY HH24:MI') AS HR1, TO_CHAR(3 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') 
                                                                                                                                                                        AS HR3, TO_CHAR(6 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR6, 
                                                                                                                                                                        TO_CHAR(12 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR12, 
                                                                                                                                                                        TO_CHAR(24 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR24, TO_CHAR(2 + ""TIMESTAMP"",
                                                                                                                                                                         'MM/DD/YYYY HH24:MI') AS HR48, TO_CHAR(7 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') 
                                                                                                                                                                        AS HR168
                                                                                                                                              FROM            (SELECT        ""TIMESTAMP"", AVG(RAIN) AS RAIN
                                                                                                                                                                        FROM            NEXRAIN.PIXEL RAINFALL_1
                                                                                                                                                                        WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                                                                  (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                                                                        GROUP BY ""TIMESTAMP"") DERIVEDTBL_1) STEP3 ON STEP2_2.TIMESTAMP >= STEP3.HR0 AND 
                                                                                                                                        STEP2_2.TIMESTAMP < STEP3.HR" + iHour.ToString() + @"
                                                                                                              GROUP BY STEP3.HR0, STEP3.HR" + iHour.ToString() + @") STEP4) STEP5 INNER JOIN
                                                                                        (SELECT        SUM(STEP2_1.RAIN) AS SUMOFRAIN, STEP3_1.HR0, STEP3_1.HR" + iHour.ToString() + @"
                                                                                          FROM            (SELECT        TO_CHAR(""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS TIMESTAMP, AVG(RAIN) AS RAIN
                                                                                                                    FROM            NEXRAIN.PIXEL RAINFALL_2
                                                                                                                    WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                              (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                    GROUP BY ""TIMESTAMP"") STEP2_1 INNER JOIN
                                                                                                                        (SELECT        TO_CHAR(0 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR0, TO_CHAR(1 / 24 + ""TIMESTAMP"", 
                                                                                                                                                    'MM/DD/YYYY HH24:MI') AS HR1, TO_CHAR(3 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR3, 
                                                                                                                                                    TO_CHAR(6 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR6, TO_CHAR(12 / 24 + ""TIMESTAMP"", 
                                                                                                                                                    'MM/DD/YYYY HH24:MI') AS HR12, TO_CHAR(24 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR24, 
                                                                                                                                                    TO_CHAR(2 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR48, TO_CHAR(7 + ""TIMESTAMP"", 
                                                                                                                                                    'MM/DD/YYYY HH24:MI') AS HR168
                                                                                                                          FROM            (SELECT        ""TIMESTAMP"", AVG(RAIN) AS RAIN
                                                                                                                                                    FROM            NEXRAIN.PIXEL RAINFALL_1
                                                                                                                                                    WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                                              (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                                                    GROUP BY ""TIMESTAMP"") DERIVEDTBL_1_1) STEP3_1 ON STEP2_1.TIMESTAMP >= STEP3_1.HR0 AND 
                                                                                                                    STEP2_1.TIMESTAMP < STEP3_1.HR" + iHour.ToString() + @"
                                                                                          GROUP BY STEP3_1.HR0, STEP3_1.HR" + iHour.ToString() + @") STEP4_1 ON STEP5.MAXOFSUMOFRAIN = STEP4_1.SUMOFRAIN) STEP6 ON 
                                                    STEP2.TIMESTAMP >= STEP6.HR0 AND STEP2.TIMESTAMP < STEP6.HR" + iHour.ToString() + @") STEP7
                                                    GROUP BY MAXOFSUMOFRAIN";


                        #endregion
                        break;


                    case "CORRECTED":
                        #region CORRECYED PIXEL Query

                        strOracleQuery = @"
SELECT        MIN(TIMESTAMP) AS TS, MAXOFSUMOFRAIN AS TOTAL
FROM            (SELECT        STEP2.TIMESTAMP, STEP6.MAXOFSUMOFRAIN
                          FROM            (SELECT        TO_CHAR(""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS TIMESTAMP, AVG(RAIN) AS RAIN
                                                    FROM            ONERAIN_EOM.PIXEL
                                                    WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY hh24:MI')) AND (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY hh24:MI')) 
                                                                              AND (PIXEL = " + strPointLocation + @")
                                                    GROUP BY ""TIMESTAMP"") STEP2 INNER JOIN
                                                        (SELECT        STEP5.MAXOFSUMOFRAIN, STEP4_1.HR0, STEP4_1.HR" + iHour.ToString() + @"
                                                          FROM            (SELECT        MAX(SUMOFRAIN) AS MAXOFSUMOFRAIN
                                                                                    FROM            (SELECT        SUM(STEP2_2.RAIN) AS SUMOFRAIN, STEP3.HR0, STEP3.HR" + iHour.ToString() + @"
                                                                                                              FROM            (SELECT        TO_CHAR(""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS TIMESTAMP, AVG(RAIN) AS RAIN
                                                                                                                                        FROM            ONERAIN_EOM.PIXEL RAINFALL_3
                                                                                                                                        WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                                  (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                                        GROUP BY ""TIMESTAMP"") STEP2_2 INNER JOIN
                                                                                                                                            (SELECT        TO_CHAR(0 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR0, TO_CHAR(1 / 24 + ""TIMESTAMP"", 
                                                                                                                                                                        'MM/DD/YYYY HH24:MI') AS HR1, TO_CHAR(3 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') 
                                                                                                                                                                        AS HR3, TO_CHAR(6 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR6, 
                                                                                                                                                                        TO_CHAR(12 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR12, 
                                                                                                                                                                        TO_CHAR(24 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR24, TO_CHAR(2 + ""TIMESTAMP"",
                                                                                                                                                                         'MM/DD/YYYY HH24:MI') AS HR48, TO_CHAR(7 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') 
                                                                                                                                                                        AS HR168
                                                                                                                                              FROM            (SELECT        ""TIMESTAMP"", AVG(RAIN) AS RAIN
                                                                                                                                                                        FROM            ONERAIN_EOM.PIXEL RAINFALL_1
                                                                                                                                                                        WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                                                                  (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                                                                        GROUP BY ""TIMESTAMP"") DERIVEDTBL_1) STEP3 ON STEP2_2.TIMESTAMP >= STEP3.HR0 AND 
                                                                                                                                        STEP2_2.TIMESTAMP < STEP3.HR" + iHour.ToString() + @"
                                                                                                              GROUP BY STEP3.HR0, STEP3.HR" + iHour.ToString() + @") STEP4) STEP5 INNER JOIN
                                                                                        (SELECT        SUM(STEP2_1.RAIN) AS SUMOFRAIN, STEP3_1.HR0, STEP3_1.HR" + iHour.ToString() + @"
                                                                                          FROM            (SELECT        TO_CHAR(""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS TIMESTAMP, AVG(RAIN) AS RAIN
                                                                                                                    FROM            ONERAIN_EOM.PIXEL RAINFALL_2
                                                                                                                    WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                              (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                    GROUP BY ""TIMESTAMP"") STEP2_1 INNER JOIN
                                                                                                                        (SELECT        TO_CHAR(0 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR0, TO_CHAR(1 / 24 + ""TIMESTAMP"", 
                                                                                                                                                    'MM/DD/YYYY HH24:MI') AS HR1, TO_CHAR(3 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR3, 
                                                                                                                                                    TO_CHAR(6 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR6, TO_CHAR(12 / 24 + ""TIMESTAMP"", 
                                                                                                                                                    'MM/DD/YYYY HH24:MI') AS HR12, TO_CHAR(24 / 24 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR24, 
                                                                                                                                                    TO_CHAR(2 + ""TIMESTAMP"", 'MM/DD/YYYY HH24:MI') AS HR48, TO_CHAR(7 + ""TIMESTAMP"", 
                                                                                                                                                    'MM/DD/YYYY HH24:MI') AS HR168
                                                                                                                          FROM            (SELECT        ""TIMESTAMP"", AVG(RAIN) AS RAIN
                                                                                                                                                    FROM            ONERAIN_EOM.PIXEL RAINFALL_1
                                                                                                                                                    WHERE        (""TIMESTAMP"" >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                                              (""TIMESTAMP"" <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (PIXEL = " + strPointLocation + @")
                                                                                                                                                    GROUP BY ""TIMESTAMP"") DERIVEDTBL_1_1) STEP3_1 ON STEP2_1.TIMESTAMP >= STEP3_1.HR0 AND 
                                                                                                                    STEP2_1.TIMESTAMP < STEP3_1.HR" + iHour.ToString() + @"
                                                                                          GROUP BY STEP3_1.HR0, STEP3_1.HR" + iHour.ToString() + @") STEP4_1 ON STEP5.MAXOFSUMOFRAIN = STEP4_1.SUMOFRAIN) STEP6 ON 
                                                    STEP2.TIMESTAMP >= STEP6.HR0 AND STEP2.TIMESTAMP < STEP6.HR" + iHour.ToString() + @") STEP7
                                                    GROUP BY MAXOFSUMOFRAIN";


                        #endregion
                        break;



                    case "GUAGE":
                        #region RAINGUAGE Query

                        strOracleQuery = @"
SELECT        MIN(EVENT_DATETIME) AS TS, MAXOFSUMOFRAIN AS TOTAL
FROM            (SELECT        STEP2.EVENT_DATETIME, STEP6.MAXOFSUMOFRAIN
                    FROM            (SELECT        TO_CHAR(EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS EVENT_DATETIME, AVG(INTVALUE / 100) AS RAIN
                                    FROM            RAINFALL.RAINFALL
                                    WHERE        (EVENT_DATETIME >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH12:MI')) AND (EVENT_DATETIME <= TO_DATE('" + strEndDate + @"', 
                                                'MM/DD/YYYY HH12:MI')) AND (RGSITE = '" + strPointLocation + @"')
                            GROUP BY EVENT_DATETIME) STEP2 INNER JOIN
                (SELECT        STEP5.MAXOFSUMOFRAIN, STEP4_1.HR0, STEP4_1.HR" + iHour.ToString() + @"
                    FROM            (SELECT        MAX(SUMOFRAIN) AS MAXOFSUMOFRAIN
                                            FROM            (SELECT        SUM(STEP2_2.RAIN) AS SUMOFRAIN, STEP3.HR0, STEP3.HR" + iHour.ToString() + @"
                                                                        FROM            (SELECT        TO_CHAR(EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS EVENT_DATETIME, AVG(INTVALUE / 100) 
                                                                                                                            AS RAIN
                                                                                                FROM            RAINFALL.RAINFALL RAINFALL_3
                                                                                                WHERE        (EVENT_DATETIME >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                            (EVENT_DATETIME <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY  HH24:MI')) AND (RGSITE = '" + strPointLocation + @"')
                                                                                                GROUP BY EVENT_DATETIME) STEP2_2 INNER JOIN
                                                                                                    (SELECT        TO_CHAR(0 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR0, 
                                                                                                                    TO_CHAR(1 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR1,
                                                                                                                    TO_CHAR(3 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR3, 
                                                                                                                    TO_CHAR(6 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR6, 
                                                                                                                    TO_CHAR(12 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR12, 
                                                                                                                    TO_CHAR(24 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR24, 
                                                                                                                    TO_CHAR(2 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR48, 
                                                                                                                    TO_CHAR(7 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR168
                                                                                                        FROM            (SELECT        EVENT_DATETIME, AVG(INTVALUE / 100) AS RAIN
                                                                                                                                FROM            RAINFALL.RAINFALL RAINFALL_1
                                                                                                                                WHERE        (EVENT_DATETIME >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                            (EVENT_DATETIME <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                                                                            (RGSITE = '" + strPointLocation + @"')
                                                                                                                                GROUP BY EVENT_DATETIME) DERIVEDTBL_1) STEP3 ON 
                                                                                                STEP2_2.EVENT_DATETIME >= STEP3.HR0 AND STEP2_2.EVENT_DATETIME < STEP3.HR" + iHour.ToString() + @"
                                                                        GROUP BY STEP3.HR0, STEP3.HR" + iHour.ToString() + @") STEP4) STEP5 INNER JOIN
                (SELECT        SUM(STEP2_1.RAIN) AS SUMOFRAIN, STEP3_1.HR0, STEP3_1.HR" + iHour.ToString() + @"
                    FROM            (SELECT        TO_CHAR(EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS EVENT_DATETIME, AVG(INTVALUE / 100) 
                                                                        AS RAIN
                                            FROM            RAINFALL.RAINFALL RAINFALL_2
                                            WHERE        (EVENT_DATETIME >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                        (EVENT_DATETIME <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (RGSITE = '" + strPointLocation + @"')
                                            GROUP BY EVENT_DATETIME) STEP2_1 INNER JOIN
                                                (SELECT         TO_CHAR(0 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR0, 
                                                                TO_CHAR(1 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR1,
                                                                TO_CHAR(3 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR3, 
                                                                TO_CHAR(6 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR6, 
                                                                TO_CHAR(12 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR12, 
                                                                TO_CHAR(24 / 24 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR24, 
                                                                TO_CHAR(2 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR48, 
                                                                TO_CHAR(7 + EVENT_DATETIME, 'MM/DD/YYYY HH24:MI') AS HR168
                                                    FROM            (SELECT        EVENT_DATETIME, AVG(INTVALUE / 100) AS RAIN
                                                                            FROM            RAINFALL.RAINFALL RAINFALL_1
                                                                            WHERE        (EVENT_DATETIME >= TO_DATE('" + strStartDate + @"', 'MM/DD/YYYY HH24:MI')) AND 
                                                                                                        (EVENT_DATETIME <= TO_DATE('" + strEndDate + @"', 'MM/DD/YYYY HH24:MI')) AND (RGSITE = '" + strPointLocation + @"')
                                                                            GROUP BY EVENT_DATETIME) DERIVEDTBL_1_1) STEP3_1 ON 
                                            STEP2_1.EVENT_DATETIME >= STEP3_1.HR0 AND STEP2_1.EVENT_DATETIME < STEP3_1.HR" + iHour.ToString() + @"
                    GROUP BY STEP3_1.HR0, STEP3_1.HR" + iHour.ToString() + @") STEP4_1 ON STEP5.MAXOFSUMOFRAIN = STEP4_1.SUMOFRAIN) STEP6 ON 
                    STEP2.EVENT_DATETIME >= STEP6.HR0 AND STEP2.EVENT_DATETIME < STEP6.HR" + iHour.ToString() + @") STEP7
                    GROUP BY MAXOFSUMOFRAIN";


                        #endregion
                        break;
                }




                double dblRainTotal = 0;
                DateTime dtStartPeriod = DateTime.Now;
                dicDateTotal = new Dictionary<DateTime, double>();



                try
                {
                    using (OracleCommand command = new OracleCommand(strOracleQuery, _connection))
                    {

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            System.Diagnostics.Debug.Write(strOracleQuery);

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dtStartPeriod = Convert.ToDateTime(reader.GetValue(0));
                                    dblRainTotal = Convert.ToDouble(reader.GetValue(1));
                                    dicDateTotal.Add(dtStartPeriod, dblRainTotal);
                                }

                            }
                            else
                            {
                                return null;
                            }


                        }

                    }

                }
                catch (Exception ex)
                {

                }
                    finally
                    {
                      
                    }
                dicMaster.Add(iHour,dicDateTotal);


            }

            //_connection.Close();
            return dicMaster;
        }





        private double CalculateAndRecord(string strModel, int iHour, string strPointLocation, int iPointID, DateTime dtStartPeriod, double dblRainTotal)
        {
            double dblStormCalc = 0;
            double dblSevenDayAtecedent = Calc_SevenDayAntecedent(strPointLocation, dtStartPeriod);

           
             
           
            try
            {


                switch (strModel)
                {

                    case "Atlas14":

                        ////calc Atlas14 correlated storm result
                        double Atlas14CalcStorm = 0;
                        switch (iHour.ToString())
                        {
                            case ("1"):

                                foreach (DepthChart a in cDepthChart.lst1HourAtlas14)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        Atlas14CalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, Atlas14CalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);

                                        dblStormCalc = Atlas14CalcStorm;

                                        _cursor.InsertRow(_rowBuffer);

                                        
                                        break;
                                    }
                                }
                                break;

                            case ("3"):

                                foreach (DepthChart a in cDepthChart.lst3HourAtlas14)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        Atlas14CalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, Atlas14CalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = Atlas14CalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;

                            case ("6"):

                                foreach (DepthChart a in cDepthChart.lst6HourAtlas14)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        Atlas14CalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        string aa = dtStartPeriod.ToString();
                                        string b = dtStartPeriod.AddHours(iHour).ToString();
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, Atlas14CalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = Atlas14CalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;

                            case ("12"):

                                foreach (DepthChart a in cDepthChart.lst12HourAtlas14)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        Atlas14CalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        string aa = dtStartPeriod.ToString();
                                        string b = dtStartPeriod.AddHours(iHour).ToString();
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, Atlas14CalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = Atlas14CalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;


                            case ("24"):

                                foreach (DepthChart a in cDepthChart.lst24HourAtlas14)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        Atlas14CalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, Atlas14CalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = Atlas14CalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;



                            case ("48"):

                                foreach (DepthChart a in cDepthChart.lst48HourAtlas14)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        Atlas14CalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, Atlas14CalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = Atlas14CalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;

                        }

                        break;

                    case "CloudBurst":

                        ////calc Atlas14 correlated storm result
                        double CloudBurstCalcStorm = 0;
                        switch (iHour.ToString())
                        {
                            case ("1"):

                                foreach (DepthChart a in cDepthChart.lst1HourCloudBurst)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        CloudBurstCalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, CloudBurstCalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = CloudBurstCalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;



                            case ("3"):

                                foreach (DepthChart a in cDepthChart.lst3HourCloudBurst)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        CloudBurstCalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, CloudBurstCalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = CloudBurstCalcStorm;

                                        _cursor.InsertRow(_rowBuffer);

                                        break;
                                    }
                                }
                                break;

                            case ("6"):

                                foreach (DepthChart a in cDepthChart.lst6HourCloudBurst)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        CloudBurstCalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, CloudBurstCalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = CloudBurstCalcStorm;

                                        _cursor.InsertRow(_rowBuffer);


                                        break;
                                    }
                                }
                                break;

                            case ("12"):

                                foreach (DepthChart a in cDepthChart.lst12HourCloudBurst)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        CloudBurstCalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, CloudBurstCalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = CloudBurstCalcStorm;

                                        _cursor.InsertRow(_rowBuffer);

                                        break;
                                    }
                                }
                                break;


                            case ("24"):

                                foreach (DepthChart a in cDepthChart.lst24HourCloudBurst)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        CloudBurstCalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, CloudBurstCalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = CloudBurstCalcStorm;

                                        _cursor.InsertRow(_rowBuffer);

                                        break;
                                    }
                                }
                                break;


                            case ("48"):

                                foreach (DepthChart a in cDepthChart.lst48HourCloudBurst)
                                {
                                    if ((dblRainTotal >= a.LowVal) && (dblRainTotal <= a.HighVal))
                                    {
                                        CloudBurstCalcStorm = CalcCorrelatedStorm(dblRainTotal, a.lowYear, a.highYear, a.LowVal, a.HighVal);

                                        _rowBuffer.set_Value(1, iPointID);
                                        _rowBuffer.set_Value(2, strPointLocation);
                                        _rowBuffer.set_Value(3, iHour.ToString() + " hr");
                                        _rowBuffer.set_Value(4, dtStartPeriod.ToString());
                                        _rowBuffer.set_Value(5, dtStartPeriod.AddHours(iHour).ToString());
                                        _rowBuffer.set_Value(6, dblRainTotal);
                                        _rowBuffer.set_Value(7, strModel);
                                        _rowBuffer.set_Value(8, a.lowYear);
                                        _rowBuffer.set_Value(9, a.LowVal);
                                        _rowBuffer.set_Value(10, a.highYear);
                                        _rowBuffer.set_Value(11, a.HighVal);
                                        _rowBuffer.set_Value(12, CloudBurstCalcStorm);

                                        _rowBuffer.set_Value(14, dblSevenDayAtecedent);
                                        dblStormCalc = CloudBurstCalcStorm;

                                        _cursor.InsertRow(_rowBuffer);

                                        break;
                                    }
                                }
                                break;


                        }
                        break;
                      
                }


                return dblStormCalc;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
          
        }









        private double CalcCorrelatedStorm(double dblRainTotal, int lowYear, int highYear, double LowValue, double HighValue)
        {

            return Math.Round(lowYear + ((dblRainTotal - LowValue) / (HighValue - LowValue)) * (highYear - lowYear), 2);
            //[Lo]+(([Value]-[Low Value])/([High Value]-[Low Value]))*([Hi]-[Lo])
        }
  
        //Calculates the distances from datalayer to reflayer.
        public void GetClosestPixel(IFeatureLayer pPointLayer, IFeatureLayer pReflayer, IFeatureLayer pOUTPoints)
        {


            IFeatureClass pFClass = pPointLayer.FeatureClass;
            IFeatureClass pRefClass = pReflayer.FeatureClass;

            IFeatureSelection pFSel = pPointLayer as IFeatureSelection;
            ISelectionSet pSelSet = pFSel.SelectionSet;
            ICursor pCurs;
            pSelSet.Search(null, false, out pCurs);
            IFeatureCursor ppointcurs = pCurs as IFeatureCursor;



            IGeoDataset pgeo = pFClass as IGeoDataset;
            pgeo = pRefClass as IGeoDataset;

            //IFeatureCursor ppointcurs = pFClass.Search(null, false);
            IFeature ppointFeat = ppointcurs.NextFeature();

            IPoint ppoint = ppointFeat.Shape as IPoint;

            IFeatureBuffer pFBuffer = pOUTPoints.FeatureClass.CreateFeatureBuffer();
            IFeatureCursor pFCurOutPoints = pOUTPoints.Search(null, false);
            pFCurOutPoints = pOUTPoints.FeatureClass.Insert(true);

            IFeatureCursor plinecurs;
            IFeatureIndex2 pFtrInd;
            IIndexQuery2 pIndQry;
            int FtdID = 0;
            double dDist2Ftr = 0;
            int c = 1;
            IEnvelope pCombinedEnvelope = CombineExtents(pFClass, pRefClass);
            IFeature pPixelFeature;
            if (pCombinedEnvelope.IsEmpty)
            {
                MessageBox.Show("Spatial Envelope is empty, aborting process...");
                return;
            }

            while (ppointFeat != null)
            {
                //Application.DoEvents();
                try
                {
                    
                    plinecurs = pRefClass.Search(null, false);
                    pFtrInd = new FeatureIndexClass();
                    pFtrInd.FeatureClass = pRefClass;
                    pFtrInd.FeatureCursor = plinecurs;
                    //pFtrInd.set_OutputSpatialReference(pFClass.ShapeFieldName, pgeo.SpatialReference);
                    pFtrInd.Index(null, pCombinedEnvelope);

                    pIndQry = pFtrInd as IIndexQuery2;
                    pIndQry.NearestFeature(ppointFeat.Shape, out FtdID, out dDist2Ftr);

                    pPixelFeature = pRefClass.GetFeature(FtdID);
                    string pixel = pPixelFeature.get_Value(pPixelFeature.Fields.FindField("PIXEL")).ToString();
                    pFBuffer.set_Value(pFBuffer.Fields.FindField("PointID"), ppointFeat.OID);
                    pFBuffer.set_Value(pFBuffer.Fields.FindField("Pixel"), pixel);
                    pFBuffer.Shape = pPixelFeature.ShapeCopy;
                    pFCurOutPoints.InsertFeature(pFBuffer);

                    //ppointFeat.set_Value(ppointFeat.Fields.FindField(DestField), dDist2Ftr);
                    //ppointFeat.Store();
                   // MessageBox.Show("Closest pixel is " + FtdID);
                   
                    FtdID = 0;
                    dDist2Ftr = 0;

                    ppointFeat = ppointcurs.NextFeature();
                    c++;
                }
                catch
                {

                    FtdID = 0;
                    dDist2Ftr = 0;
                    ppointFeat = ppointcurs.NextFeature();
                }


            }


         

    

        }

        private IFeatureClass MakeOUTFeatureClass(IFeatureLayer pFeatureLayer)
        {

            try
            {

                IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                IWorkspace pWorkspace = workspaceFactory.OpenFromFile(Defaultgdb.DefaultDatabaseName.PathName, 0);




                IFeatureWorkspace workspace = pWorkspace as IFeatureWorkspace;
                UID CLSID = new UID();
                CLSID.Value = "esriGeodatabase.Feature";

                IFields pFields = new FieldsClass();
                IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
                //pFieldsEdit.FieldCount_2 = 17;
                pFieldsEdit.FieldCount_2 = 3;


                IGeoDataset geoDataset = pFeatureLayer as IGeoDataset;


                IGeometryDef pGeomDef = new GeometryDef();
                IGeometryDefEdit pGeomDefEdit = pGeomDef as IGeometryDefEdit;
                pGeomDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                pGeomDefEdit.SpatialReference_2 = geoDataset.SpatialReference;



                IField pField;
                IFieldEdit pFieldEdit;

          

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "SHAPE";
                pFieldEdit.Name_2 = "SHAPE";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                pFieldEdit.GeometryDef_2 = pGeomDef;
                pFieldsEdit.set_Field(0, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "PointID";
                pFieldEdit.Name_2 = "PointID";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                pFieldsEdit.set_Field(1, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "PIXEL"; 
                pFieldEdit.Name_2 = "PIXEL";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                pFieldsEdit.set_Field(2, pFieldEdit);





                string strFCName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName());
                char[] chars = strFCName.ToCharArray();
                if (Char.IsDigit(chars[0]))
                {
                    strFCName = strFCName.Remove(0, 1);
                }
                KillExistingFeatureclass(strFCName);



                IFeatureClass pFeatureClass = workspace.CreateFeatureClass("rain_" + strFCName, pFieldsEdit, CLSID, null, esriFeatureType.esriFTSimple, "SHAPE", "");
                return pFeatureClass;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                return null;
            }
        }


        private IStandaloneTable AddTable(ITable ptable)
        {
            try
            {
              

                //IWorkspaceFactory pFact = new TextFileWorkspaceFactory();
                //IWorkspace pWorkspace = pFact.OpenFromFile(strPath, 0);
                //IFeatureWorkspace pFeatws = pWorkspace as IFeatureWorkspace;
                //ITable ptable = pFeatws.OpenTable(strFileName);

                IMxDocument pmxdoc = ArcMap.Document as IMxDocument;
                IMap pmap = pmxdoc.FocusMap;

                IStandaloneTable pStTab = new StandaloneTableClass();
                pStTab.Table = ptable;

                switch (_strPointType)
                {
                    case "PIXEL":
                        pStTab.Name = "Storm(PIXEL)";
                        break;
                    case "CORRECTED":
                        pStTab.Name = "Storm(EOM-PIXEL)";
                        break;
                    case "GUAGE":
                        pStTab.Name = "Storm(GUAGE)";
                        break;
                }


                _strStormTableDisplayName = pStTab.Name;


                IStandaloneTableCollection pStTabColl = pmap as IStandaloneTableCollection;
                pStTabColl.AddStandaloneTable(pStTab);
                pmxdoc.UpdateContents();

                //ITableWindow2 ptableWindow = new TableWindowClass();
                //ptableWindow.Application = ArcMap.Application;
                //ptableWindow.StandaloneTable = pStTab;
                //ptableWindow.TableSelectionAction = esriTableSelectionActions.esriSelectFeatures;
                ////ptableWindow.Show(true);

                return pStTab;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }


        private void ShowTable(IStandaloneTable pStTab)
        {
            ITableWindow2 ptableWindow = new TableWindowClass();
            ptableWindow.Application = ArcMap.Application;
            ptableWindow.StandaloneTable = pStTab;
            //ptableWindow.TableSelectionAction = esriTableSelectionActions.esriSelectFeatures;
            //ptableWindow.ShowSelected = true;
            ptableWindow.Show(true);
            MessageBox.Show("Table showing highlighted records of MAXSTORM(s)", "Storm Calculator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private IEnvelope CombineExtents(IFeatureClass pfclass1, IFeatureClass pfeatclass2)
        {

            try
            {

                IEnvelope pEnv = new EnvelopeClass();
                IGeoDataset pGeoDS = pfclass1 as IGeoDataset;
                pGeoDS.Extent.QueryEnvelope(pEnv);

                pGeoDS = pfeatclass2 as IGeoDataset;
                pEnv.Union(pGeoDS.Extent);

                return pEnv;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Be sure that both layers have matching Spatial References.");
                IEnvelope pEnv = new EnvelopeClass();
                pEnv.SetEmpty();
                return pEnv;
            }



        }


        public IFeatureLayer GetSDELayer(string FClassName)
        {

            IWorkspaceFactory2 pWorkFact = new SdeWorkspaceFactoryClass();
            IFeatureWorkspace pFWorkspace = pWorkFact.OpenFromString(", 0) as IFeatureWorkspace;
            IFeatureClass pFClass = pFWorkspace.OpenFeatureClass(FClassName);
            IFeatureLayer pFLayer = new FeatureLayerClass();
            pFLayer.FeatureClass = pFClass;
            return pFLayer;


        }


        private void KillExistingFeatureclass(string strFilename)
        {
            try
            {

                IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                IWorkspace pWorkspace = workspaceFactory.OpenFromFile(Defaultgdb.DefaultDatabaseName.PathName, 0);

                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(strFilename);
                IDataset pDataset = pFeatureLayer.FeatureClass as IDataset;
                if (pDataset.CanDelete())
                {
                    pDataset.Delete();
                }
            }

            catch { }
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


        private ITable MakeCalcTable()
        {

            try
            {

                IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;
                IWorkspaceFactory workspaceFactory = Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")) as IWorkspaceFactory;


                IWorkspace workspace = workspaceFactory.OpenFromFile(Defaultgdb.DefaultDatabaseName.PathName, 0);
                IFeatureWorkspace pFeatWorkSpace = workspace as IFeatureWorkspace;

                ITable ptable;

                //UID CLSID = new UID();
                //CLSID.Value = "esriGeoDatabase.Object";

                IFields pFields = new FieldsClass();
                IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
                pFieldsEdit.FieldCount_2 = 15;



                IField pField;
                IFieldEdit pFieldEdit;

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "ObjectID";
                pFieldEdit.Name_2 = "ObjectID";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
                pFieldsEdit.set_Field(0, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "PointID";
                pFieldEdit.Name_2 = "PointID";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                pFieldsEdit.set_Field(1, pFieldEdit);


                switch (_strPointType)
                {
                    case "PIXEL":
                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.AliasName_2 = "PIXEL";
                    pFieldEdit.Name_2 = "PIXEL";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldsEdit.set_Field(2, pFieldEdit);
                    break;

                    case "CORRECTED":
                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.AliasName_2 = "PIXEL";
                    pFieldEdit.Name_2 = "PIXEL";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldsEdit.set_Field(2, pFieldEdit);
                    break;

                    case "GUAGE":
                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.AliasName_2 = "GUAGE";
                    pFieldEdit.Name_2 = "GUAGE";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldsEdit.set_Field(2, pFieldEdit);
                    break;
                }

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "Period";
                pFieldEdit.Name_2 = "Period";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                pFieldsEdit.set_Field(3, pFieldEdit);


                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "Start";
                pFieldEdit.Name_2 = "Start";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                pFieldsEdit.set_Field(4, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "End";
                pFieldEdit.Name_2 = "End";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                pFieldsEdit.set_Field(5, pFieldEdit);


                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "Value";
                pFieldEdit.Name_2 = "Value";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                pFieldsEdit.set_Field(6, pFieldEdit);


                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "Standard";
                pFieldEdit.Name_2 = "Standard";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                pFieldsEdit.set_Field(7, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "LowFreq";
                pFieldEdit.Name_2 = "LowFreq";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                pFieldsEdit.set_Field(8, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "LowVal";
                pFieldEdit.Name_2 = "LowVal";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                pFieldsEdit.set_Field(9, pFieldEdit);
                //sw.WriteLine("Pixel,PointID,Period,Start,End,Value,Standard,LowFreq,LowVal,HighFreq,HighVal,CalcStorm");

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "HighFreq";
                pFieldEdit.Name_2 = "HighFreq";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                pFieldsEdit.set_Field(10, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "HighVal";
                pFieldEdit.Name_2 = "HighVal";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                pFieldsEdit.set_Field(11, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "CalcStorm";
                pFieldEdit.Name_2 = "CalcStorm";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                pFieldsEdit.set_Field(12, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "MAXStorm";
                pFieldEdit.Name_2 = "MAXStorm";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                pFieldsEdit.set_Field(13, pFieldEdit);

                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.AliasName_2 = "Prior7SUM";
                pFieldEdit.Name_2 = "Prior7SUM";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                pFieldsEdit.set_Field(14, pFieldEdit);


                string strRandomFileName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName());
           

                //Be sure the first character isnt a number resulting in an invalid filename
                char[] chars = strRandomFileName.ToCharArray();
                while (Char.IsDigit(chars[0]))
                {
                    strRandomFileName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName());
                    chars = strRandomFileName.ToCharArray();
                }

                _strStormTableName = strRandomFileName;
                IFeatureClassDescription fcDesc = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDesc = (IObjectClassDescription)fcDesc;
                IFeatureWorkspace fw = (IFeatureWorkspace)workspace;
                ptable = fw.CreateTable(strRandomFileName, pFields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, "");
    

                //ptable = pFeatWorkSpace.CreateTable(strRandomFileName, pFieldsEdit, CLSID, null, "");

                _rowBuffer = ptable.CreateRowBuffer();
                _cursor = ptable.Insert(true);
             


                return ptable;

            }

            catch (Exception ex)
            {
                return null;
            }
        }




        public bool MergePdfFiles(List<string> pdfFiles, string outputPath)
        {
            pdfFiles.Sort();

            IList<string> lstPDFFiles2 = new List<string>();

            //Reshuffle to make sure the cover page is first
            for (int i = 0; i <= pdfFiles.Count - 1; i++)
            {
                if (pdfFiles[i].ToString().ToUpper().Contains("COVER"))
                {
                    lstPDFFiles2.Add(pdfFiles[i]);
                    break;
                }
            }

            for (int i = 0; i <= pdfFiles.Count - 1; i++)
            {
                if (!pdfFiles[i].ToString().ToUpper().Contains("COVER"))
                {
                    lstPDFFiles2.Add(pdfFiles[i]);

                }
            }



            //Merge now

            bool result = false;
            int pdfCount = 0;
            //total input pdf file count
            int f = 0;
            //pointer to current input pdf file
            string fileName = string.Empty;
            //current input pdf filename
            iTextSharp.text.pdf.PdfReader reader = null;
            int pageCount = 0;
            //cureent input pdf page count
            iTextSharp.text.Document pdfDoc = null;
            //the output pdf document
            PdfWriter writer = null;
            PdfContentByte cb = null;
            //Declare a variable to hold the imported pages
            PdfImportedPage page = null;
            int rotation = 0;
            //Declare a font to used for the bookmarks
            iTextSharp.text.Font bookmarkFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, iTextSharp.text.Font.BOLD, iTextSharp.text.Color.BLUE);
            try
            {
                pdfCount = lstPDFFiles2.Count;
                if (pdfCount > 0)
                {
                    //Open the 1st pad using PdfReader object
                    fileName = lstPDFFiles2[f].ToString();
                    reader = new iTextSharp.text.pdf.PdfReader(fileName);
                    //Get page count
                    pageCount = reader.NumberOfPages;
                    //Instantiate an new instance of pdf document and set its margins. This will be the output pdf.
                    //NOTE: bookmarks will be added at the 1st page of very original pdf file using its filename. The location
                    //of this bookmark will be placed at the upper left hand corner of the document. So you'll need to adjust
                    //the margin left and margin top values such that the bookmark won't overlay on the merged pdf page. The
                    //unit used is "points" (72 points = 1 inch), thus in this example, the bookmarks' location is at 1/4 inch from
                    //left and 1/4 inch from top of the page.
                    pdfDoc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1), 18, 18, 18, 18);
                    //Instantiate a PdfWriter that listens to the pdf document
                    writer = PdfWriter.GetInstance(pdfDoc, new System.IO.FileStream(outputPath, FileMode.Create));
                    //Set metadata and open the document
                    {
                        //pdfDoc.AddAuthor("Your name here");
                        //pdfDoc.AddCreationDate();
                        //pdfDoc.AddCreator("Your program name here");
                        //pdfDoc.AddSubject("Whatever subject you want to give it");
                        ////Use the filename as the title... You can give it any title of course.
                        //pdfDoc.AddTitle(System.IO.Path.GetFileNameWithoutExtension(outputPath));
                        ////Add keywords, whatever keywords you want to attach to it
                        //pdfDoc.AddKeywords("Report, Merged PDF, " + System.IO.Path.GetFileName(outputPath));
                        pdfDoc.Open();
                    }
                    //Instantiate a PdfContentByte object
                    cb = writer.DirectContent;
                    //Now loop thru the input pdfs
                    while (f < pdfCount)
                    {
                        //Declare a page counter variable
                        int i = 0;
                        //Loop thru the current input pdf's pages starting at page 1
                        while (i < pageCount)
                        {
                            i += 1;
                            //Get the input page size
                            pdfDoc.SetPageSize(reader.GetPageSizeWithRotation(i));
                            //Create a new page on the output document
                            pdfDoc.NewPage();
                            //If it is the 1st page, we add bookmarks to the page
                            if (i == 1)
                            {
                                //First create a paragraph using the filename as the heading
                                iTextSharp.text.Paragraph para = new iTextSharp.text.Paragraph(System.IO.Path.GetFileName(fileName).ToUpper(), bookmarkFont);
                                //Then create a chapter from the above paragraph
                                iTextSharp.text.Chapter chpter = new iTextSharp.text.Chapter(para, f + 1);
                                //Finally add the chapter to the document
                                //pdfDoc.Add(chpter);
                            }
                            //Now we get the imported page
                            page = writer.GetImportedPage(reader, i);
                            //Read the imported page's rotation
                            rotation = reader.GetPageRotation(i);
                            //Then add the imported page to the PdfContentByte object as a template based on the page's rotation
                            if (rotation == 90)
                            {
                                cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                            }
                            else if (rotation == 270)
                            {
                                cb.AddTemplate(page, 0, 1f, -1f, 0, reader.GetPageSizeWithRotation(i).Width + 60, -30);
                            }
                            else
                            {
                                cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            }
                        }
                        //Increment f and read the next input pdf file
                        f += 1;
                        if (f < pdfCount)
                        {
                            fileName = lstPDFFiles2[f].ToString();
                            reader = new iTextSharp.text.pdf.PdfReader(fileName);
                            pageCount = reader.NumberOfPages;
                        }
                    }
                    //When all done, we close the documwent so that the pdfwriter object can write it to the output file
                    pdfDoc.Close();
                    pdfDoc = null;
                    cb = null;
                    reader = null;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }


       


    }






  

}



//////private void RelateData(ITable ptable, IFeatureLayer pPointLayer)
//////{
//////    try
//////    {

//////        ////// Get the origin and destination classes from the relationship class.
//////        ITable originClass = pPointLayer.FeatureClass as ITable;
//////        ITable destinationClass = ptable;

//////        // Get the origin and destination objects.
//////        IObjectClass originObject = originClass as IObjectClass;
//////        IObjectClass destinationObject = destinationClass as IObjectClass;

//////        IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;
//////        Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
//////        IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
//////        IWorkspace pWorkspace = workspaceFactory.OpenFromFile(Defaultgdb.DefaultDatabaseName.PathName, 0);

//////        IFeatureWorkspace featureWorkspace = pWorkspace as IFeatureWorkspace;

//////        IRelationshipClassContainer relClassContainer = (IRelationshipClassContainer)featureWorkspace.OpenFeatureDataset(nameOfFeatureDataset);

//////        IObjectClass originClass = (IObjectClass)featureWorkspace.OpenTable(nameOfOriginClass);
//////        IObjectClass destinationClass = (IObjectClass)featureWorkspace.OpenFeatureClass(nameOfDestClass);

//////        IRelationshipClass relClass = relClassContainer.CreateRelationshipClass(nameOfRelClass,
//////            originClass, destinationClass, "owns", "is owned by",
//////            esriRelCardinality.esriRelCardinalityOneToMany, esriRelNotification.esriRelNotificationNone,
//////            false, false, null, "PROPERTY_ID", "", "PROPERTY_ID", "");


//////        ////// Get the origin and destination classes from the relationship class.
//////        ////ITable originClass = pPointLayer.FeatureClass as ITable;
//////        ////ITable destinationClass = ptable;

//////        ////// Get the origin and destination objects.
//////        ////IObjectClass originObject = originClass as IObjectClass;
//////        ////IObjectClass destinationObject = destinationClass as IObjectClass;

//////        ////IGxCatalogDefaultDatabase Defaultgdb = ArcMap.Application as IGxCatalogDefaultDatabase;
//////        ////Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
//////        ////IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
//////        ////IWorkspace pWorkspace = workspaceFactory.OpenFromFile(Defaultgdb.DefaultDatabaseName.PathName, 0);

//////        ////IFeatureWorkspace featureWorkspace = pWorkspace as IFeatureWorkspace;

//////        ////// Creating a relationship class without an intermediate table.
//////        ////IRelationshipClass relClass = featureWorkspace.CreateRelationshipClass
//////        ////  ("TabletoLayer", originClass  as IObjectClass, ptable as IObjectClass, "owns", "is owned by",
//////        ////  esriRelCardinality.esriRelCardinalityOneToMany,
//////        ////  esriRelNotification.esriRelNotificationNone, true, false, null, "FID",
//////        ////  "", "PointID", "");


//////        //IDisplayTable pDispTable = pPointLayer as IDisplayTable;
//////        //IFeatureClass pFCLayer = pDispTable.DisplayTable as IFeatureClass;
//////        //ITable pTLayer = pFCLayer as ITable;


//////        //IMemoryRelationshipClassFactory pMemRelFact = new MemoryRelationshipClassFactoryClass();
//////        //IRelationshipClass pRelClass = pMemRelFact.Open("TabletoLayer", ptable as IObjectClass, "PIXEL", pTLayer as IObjectClass, "PIXEL", "forward", "backward", esriRelCardinality.esriRelCardinalityOneToMany);


//////        //IDisplayRelationshipClass pDispRC = pPointLayer as IDisplayRelationshipClass;

//////        //pDispRC.DisplayRelationshipClass(pRelClass, esriJoinType.esriLeftInnerJoin);
//////    }

//////    catch (Exception ex)
//////    {

//////    }
//////}


