using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using iTextSharp.text.pdf;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Drawing;

namespace StormFrequencyCalculator
{
    class clsReport
    {
        private ITable _ptable;
     

        public string MakeReport(string strOutputFile, IFeatureLayer pSrclayer, IFeatureLayer pdfLayer, IStandaloneTable pStandaloneTable, string strPointType, string strStart, string strEnd, string strLabelField, IMapDocument mapdoc)
        {
            try
            {
                _ptable = pStandaloneTable.Table;
                 string StrRepName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

                clsExport Export = new clsExport();

                //RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\\LOJICReg\\MSD");
                //string strPDFTemplate = regkey.GetValue("SupportFiles").ToString() + @"\AddinsSupportFiles\StormFrequencyCalculator\template.pdf";
                string strPDFTemplate = @"M:\prod\MSD\AddinsSupportFiles\StormFrequencyCalculator\template.pdf";

                File.Copy(strPDFTemplate, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\" + StrRepName, true);
                string strTemplate = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\" + StrRepName;

                IFeatureCursor pFeatureCursor;
                pFeatureCursor = pdfLayer.FeatureClass.Search(null, false);
                //ICursor pCursor = pStandaloneTable.Table.Search(null, false);
                IFeature pFeature = pFeatureCursor.NextFeature();

                string strPointID;
                List<string> lstPDFPages = new List<string>();
                IFeature pSrcFeature = null;
                while (pFeature != null)
                {
                    try
                    {
                        strPointID = pFeature.get_Value(pFeature.Fields.FindField("PointID")).ToString();
                        pSrcFeature = pSrclayer.FeatureClass.GetFeature(Convert.ToInt32(strPointID));
                    }
                    catch
                    {
                        strPointID = pFeature.OID.ToString();
                        pSrcFeature = pFeature;
                    }



                    string strPointLocation = "";
                    switch (strPointType)
                    {
                        case "PIXEL":
                            strPointLocation = pFeature.get_Value(pFeature.Fields.FindField("PIXEL")).ToString();
                            break;

                        case "CORRECTED":
                            strPointLocation = pFeature.get_Value(pFeature.Fields.FindField("PIXEL")).ToString();
                            break;

                        case "GUAGE":
                            strPointLocation = pFeature.get_Value(pFeature.Fields.FindField("RGNUM")).ToString();
                            break;
                    }

                   // Make jpg
                    string strJPGFile = Export.MakeLocationMap(pFeature, mapdoc);

                   
                    string strArea = pSrcFeature.get_Value(pSrcFeature.Fields.FindField(strLabelField)).ToString();
                    FillclsReportValues(Convert.ToInt32(strPointID), strStart, strEnd, strPointType, strArea, strPointLocation);



                    string strPDFFile = MakePage(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf"), strJPGFile, strTemplate);
                    lstPDFPages.Add(strPDFFile);
                    //File.Delete(strJPGFile);

                    pFeature = pFeatureCursor.NextFeature();
                }

                string strMergedPDF = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf");
                MergePdfFiles(lstPDFPages, strMergedPDF);

                Cleanup(lstPDFPages);

                return strMergedPDF;

            }

            catch (Exception ex)
            {
                return null;
            }


        }

     


        private string MakePage(string strOutputFile, string strLocationMapImage, string strTemplate)
        {

            using (Stream inputImageStream = new FileStream(strLocationMapImage, FileMode.Open, FileAccess.Read, FileShare.Read))

            using (var existingFileStream = new System.IO.FileStream(strTemplate, FileMode.Open))
            using (var newFileStream = new System.IO.FileStream(strOutputFile, FileMode.Create))
            {
                // Open existing PDF  
                var pdfReader = new PdfReader(existingFileStream);

                // PdfStamper, which will create  
                var stamper = new PdfStamper(pdfReader, newFileStream);

                var form = stamper.AcroFields;
                var fieldKeys = form.Fields.Keys;

                foreach (string fieldKey in fieldKeys)
                {


                    switch (fieldKey)
                    {

                        case "txtRunDate":
                            //form.SetField(fieldKey, DateTime.Now.ToString("MMMM dd, yyyy"));
                            form.SetField(fieldKey, clsReportValues.RunDate);
                            break;

                        case "txtRunTime":
                            form.SetField(fieldKey, clsReportValues.RunTime);
                            break;

                        case "txtArea":
                            form.SetField(fieldKey, clsReportValues.Area);
                            break;

                        case "txtStartDate":
                            form.SetField(fieldKey, clsReportValues.StartDate);
                            break;

                        case "txtEndDate":
                            form.SetField(fieldKey, clsReportValues.EndDate);
                            break;

                        //7 day antecedent 
                        case "SevenDayAntecedent":
                            form.SetField(fieldKey, clsReportValues.SevenDayAntecedent);
                            break;

                        case "PeriodTotal":
                            form.SetField(fieldKey, clsReportValues.PeriodTotal);
                            break;


                        #region 1Hour
                        case "txtHour1Start":
                            form.SetField(fieldKey, clsReportValues.Hour1Start);
                            break;

                        case "txtHour1End":
                            form.SetField(fieldKey, clsReportValues.Hour1End);
                            break;

                        case "txtHour1RainTotal":
                            form.SetField(fieldKey, clsReportValues.Hour1RainTotal);
                            break;

                        case "txtHour1AtlasLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour1AtlasLowFrequency);
                            break;

                        case "txtHour1AtlasLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour1AtlasLowValue);
                            break;

                        case "txtHour1AtlasHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour1AtlasHighFrequency);
                            break;

                        case "txtHour1AtlasHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour1AtlasHighValue);
                            break;

                        case "txtHour1CloudBurstLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour1CloudBurstLowFrequency);
                            break;

                        case "txtHour1CloudBurstLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour1CloudBurstLowValue);
                            break;

                        case "txtHour1CloudBurstHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour1CloudBurstHighFrequency);
                            break;

                        case "txtHour1CloudBurstHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour1CloudBurstHighValue);
                            break;
                     
#endregion

                        #region 3 Hour
                        case "txtHour3Start":
                            form.SetField(fieldKey, clsReportValues.Hour3Start);
                            break;

                        case "txtHour3End":
                            form.SetField(fieldKey, clsReportValues.Hour3End);
                            break;

                        case "txtHour3RainTotal":
                            form.SetField(fieldKey, clsReportValues.Hour3RainTotal);
                            break;

                        case "txtHour3AtlasLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour3AtlasLowFrequency);
                            break;

                        case "txtHour3AtlasLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour3AtlasLowValue);
                            break;

                        case "txtHour3AtlasHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour3AtlasHighFrequency);
                            break;

                        case "txtHour3AtlasHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour3AtlasHighValue);
                            break;

                        case "txtHour3CloudBurstLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour3CloudBurstLowFrequency);
                            break;

                        case "txtHour3CloudBurstLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour3CloudBurstLowValue);
                            break;

                        case "txtHour3CloudBurstHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour3CloudBurstHighFrequency);
                            break;

                        case "txtHour3CloudBurstHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour3CloudBurstHighValue);
                            break;
#endregion


                        #region 6 Hour
                        case "txtHour6Start":
                            form.SetField(fieldKey, clsReportValues.Hour6Start);
                            break;

                        case "txtHour6End":
                            form.SetField(fieldKey, clsReportValues.Hour6End);
                            break;

                        case "txtHour6RainTotal":
                            form.SetField(fieldKey, clsReportValues.Hour6RainTotal);
                            break;

                        case "txtHour6AtlasLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour6AtlasLowFrequency);
                            break;

                        case "txtHour6AtlasLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour6AtlasLowValue);
                            break;

                        case "txtHour6AtlasHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour6AtlasHighFrequency);
                            break;

                        case "txtHour6AtlasHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour6AtlasHighValue);
                            break;

                        case "txtHour6CloudBurstLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour6CloudBurstLowFrequency);
                            break;

                        case "txtHour6CloudBurstLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour6CloudBurstLowValue);
                            break;

                        case "txtHour6CloudBurstHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour6CloudBurstHighFrequency);
                            break;

                        case "txtHour6CloudBurstHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour6CloudBurstHighValue);
                            break;
                        #endregion

                        #region 12 Hour
                        case "txtHour12Start":
                            form.SetField(fieldKey, clsReportValues.Hour12Start);
                            break;

                        case "txtHour12End":
                            form.SetField(fieldKey, clsReportValues.Hour12End);
                            break;

                        case "txtHour12RainTotal":
                            form.SetField(fieldKey, clsReportValues.Hour12RainTotal);
                            break;

                        case "txtHour12AtlasLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour12AtlasLowFrequency);
                            break;

                        case "txtHour12AtlasLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour12AtlasLowValue);
                            break;

                        case "txtHour12AtlasHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour12AtlasHighFrequency);
                            break;

                        case "txtHour12AtlasHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour12AtlasHighValue);
                            break;

                        case "txtHour12CloudBurstLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour12CloudBurstLowFrequency);
                            break;

                        case "txtHour12CloudBurstLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour12CloudBurstLowValue);
                            break;

                        case "txtHour12CloudBurstHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour12CloudBurstHighFrequency);
                            break;

                        case "txtHour12CloudBurstHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour12CloudBurstHighValue);
                            break;
                        #endregion

                        #region 24 Hour
                        case "txtHour24Start":
                            form.SetField(fieldKey, clsReportValues.Hour24Start);
                            break;

                        case "txtHour24End":
                            form.SetField(fieldKey, clsReportValues.Hour24End);
                            break;

                        case "txtHour24RainTotal":
                            form.SetField(fieldKey, clsReportValues.Hour24RainTotal);
                            break;

                        case "txtHour24AtlasLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour24AtlasLowFrequency);
                            break;

                        case "txtHour24AtlasLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour24AtlasLowValue);
                            break;

                        case "txtHour24AtlasHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour24AtlasHighFrequency);
                            break;

                        case "txtHour24AtlasHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour24AtlasHighValue);
                            break;

                        case "txtHour24CloudBurstLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour24CloudBurstLowFrequency);
                            break;

                        case "txtHour24CloudBurstLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour24CloudBurstLowValue);
                            break;

                        case "txtHour24CloudBurstHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour24CloudBurstHighFrequency);
                            break;

                        case "txtHour24CloudBurstHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour24CloudBurstHighValue);
                            break;
                        #endregion

                        #region 48 Hour
                        case "txtHour48Start":
                            form.SetField(fieldKey, clsReportValues.Hour48Start);
                            break;

                        case "txtHour48End":
                            form.SetField(fieldKey, clsReportValues.Hour48End);
                            break;

                        case "txtHour48RainTotal":
                            form.SetField(fieldKey, clsReportValues.Hour48RainTotal);
                            break;

                        case "txtHour48AtlasLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour48AtlasLowFrequency);
                            break;

                        case "txtHour48AtlasLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour48AtlasLowValue);
                            break;

                        case "txtHour48AtlasHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour48AtlasHighFrequency);
                            break;

                        case "txtHour48AtlasHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour48AtlasHighValue);
                            break;

                        case "txtHour48CloudBurstLowFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour48CloudBurstLowFrequency);
                            break;

                        case "txtHour48CloudBurstLowValue":
                            form.SetField(fieldKey, clsReportValues.Hour48CloudBurstLowValue);
                            break;

                        case "txtHour48CloudBurstHighFrequency":
                            form.SetField(fieldKey, clsReportValues.Hour48CloudBurstHighFrequency);
                            break;

                        case "txtHour48CloudBurstHighValue":
                            form.SetField(fieldKey, clsReportValues.Hour48CloudBurstHighValue);
                            break;
                        #endregion
                            

                    }


                }

                // "Flatten" the form so it wont be editable/usable anymore  
                stamper.FormFlattening = true;

                // You can also specify fields to be flattened, which  
                // leaves the rest of the form still be editable/usable  
                stamper.PartialFormFlattening("field1");


                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
                image.SetAbsolutePosition(290, 505);
                image.ScalePercent(11);
                //image.Border = iTextSharp.text.Rectangle.BOX;
                //image.BorderColor = iTextSharp.text.Color.BLACK;
                //image.BorderWidth = 5f; 

                var pdfContentByte = stamper.GetUnderContent(1);
                pdfContentByte.AddImage(image);

                stamper.Close();
                pdfReader.Close();


                return strOutputFile;
            }
            



        }


        private void FillclsReportValues(int iPointID, string strStart, string strEnd, string strPointType, string strArea, string strPointLocation)
        {

            try
            {
               
             
                
                DateTime dtStartPeriod = DateTime.Parse(strStart);
                clsStormCalc storm = new clsStormCalc(strPointType);
                double dblSevenDayAtecedent = 0;
                //clsReportValues.SevenDayAntecedent = dblSevenDayAtecedent.ToString();

                //DateTime dtEndPeriod = DateTime.Parse(strEnd);
                //double dblPeriodTotal = storm.Calc_PeriodTotal(strPointLocation, dtStartPeriod, dtEndPeriod);
                //clsReportValues.PeriodTotal = dblPeriodTotal.ToString();

                double dblPeriodTotal = 0;
                DateTime dtEndPeriod = DateTime.Parse(strEnd);
                switch (strPointType)
                {
                    case "GUAGE":
                        dblSevenDayAtecedent = storm.Calc_SevenDayAntecedent(strPointLocation, dtStartPeriod) / 100;
                        clsReportValues.SevenDayAntecedent = dblSevenDayAtecedent.ToString();
                     
                        dblPeriodTotal = storm.Calc_PeriodTotal(strPointLocation, dtStartPeriod, dtEndPeriod) / 100;
                        clsReportValues.PeriodTotal = dblPeriodTotal.ToString();
                        break;

                    default:
                        dblSevenDayAtecedent = storm.Calc_SevenDayAntecedent(strPointLocation, dtStartPeriod);
                        clsReportValues.SevenDayAntecedent = dblSevenDayAtecedent.ToString();
                      
                        dblPeriodTotal = storm.Calc_PeriodTotal(strPointLocation, dtStartPeriod, dtEndPeriod);
                        clsReportValues.PeriodTotal = dblPeriodTotal.ToString();
                        break;
                }

                //Report Headers
                clsReportValues.RunDate = DateTime.Now.ToShortDateString();
                clsReportValues.RunTime = DateTime.Now.ToShortTimeString();
                clsReportValues.StartDate = strStart;
                clsReportValues.EndDate = strEnd;

                List<string> lstHours = new List<string> { "1", "3", "6", "12", "24", "48" };

                int k;

                foreach (string strHour in lstHours)
                {
                    k = 0;
                    switch (strHour)
                    {
                        case "1":
                            //Set Area and rain total and times for both
                            //clsReportValues.Area = GetTableValue(iPointID, strHour, "Atlas14", strPointType);
                            clsReportValues.Area = strArea;
                            clsReportValues.Hour1RainTotal = GetTableValue(iPointID, strHour, "Atlas14", "Value");
                            clsReportValues.Hour1Start = GetTableValue(iPointID, strHour, "Atlas14", "Start");
                            clsReportValues.Hour1End = GetTableValue(iPointID, strHour, "Atlas14", "End");

                            //Atlas14
                            clsReportValues.Hour1AtlasLowFrequency = GetTableValue(iPointID, strHour, "Atlas14", "LowFreq") + " Year";
                            clsReportValues.Hour1AtlasLowValue = GetTableValue(iPointID, strHour, "Atlas14", "LowVal");
                            clsReportValues.Hour1AtlasHighFrequency = GetTableValue(iPointID, strHour, "Atlas14", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour1AtlasHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour1AtlasHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour1AtlasHighFrequency = "> 500 Year";
                            }
                            if (k < 500)
                            {
                                clsReportValues.Hour1AtlasHighFrequency = clsReportValues.Hour1AtlasHighFrequency + " Year";
                            }

                            clsReportValues.Hour1AtlasHighValue = GetTableValue(iPointID, strHour, "Atlas14", "HighVal");

                            //Cloudburst
                            clsReportValues.Hour1CloudBurstLowFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "LowFreq") + " Year";
                            clsReportValues.Hour1CloudBurstLowValue = GetTableValue(iPointID, strHour, "CloudBurst", "LowVal");
                            clsReportValues.Hour1CloudBurstHighFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour1CloudBurstHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour1CloudBurstHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour1CloudBurstHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour1CloudBurstHighFrequency = clsReportValues.Hour1CloudBurstHighFrequency + " Year";
                            }
                            clsReportValues.Hour1CloudBurstHighValue = GetTableValue(iPointID, strHour, "CloudBurst", "HighVal");
                            break;



                        case "3":
                            clsReportValues.Hour3RainTotal = GetTableValue(iPointID, strHour, "Atlas14", "Value");
                            clsReportValues.Hour3Start = GetTableValue(iPointID, strHour, "Atlas14", "Start");
                            clsReportValues.Hour3End = GetTableValue(iPointID, strHour, "Atlas14", "End");

                            //Atlas14
                            clsReportValues.Hour3AtlasLowFrequency = GetTableValue(iPointID, strHour, "Atlas14", "LowFreq") + " Year";
                            clsReportValues.Hour3AtlasLowValue = GetTableValue(iPointID, strHour, "Atlas14", "LowVal");
                            clsReportValues.Hour3AtlasHighFrequency = GetTableValue(iPointID, strHour, "Atlas14", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour3AtlasHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour3AtlasHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour3AtlasHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour3AtlasHighFrequency = clsReportValues.Hour3AtlasHighFrequency + " Year";
                            }
                            clsReportValues.Hour3AtlasHighValue = GetTableValue(iPointID, strHour, "Atlas14", "HighVal");

                            //Cloudburst
                            clsReportValues.Hour3CloudBurstLowFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "LowFreq") + " Year";
                            clsReportValues.Hour3CloudBurstLowValue = GetTableValue(iPointID, strHour, "CloudBurst", "LowVal");
                            clsReportValues.Hour3CloudBurstHighFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour3CloudBurstHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour3CloudBurstHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour3CloudBurstHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour3CloudBurstHighFrequency = clsReportValues.Hour3CloudBurstHighFrequency + " Year";
                            }
                            clsReportValues.Hour3CloudBurstHighValue = GetTableValue(iPointID, strHour, "CloudBurst", "HighVal");
                            break;


                        case "6":
                            clsReportValues.Hour6RainTotal = GetTableValue(iPointID, strHour, "Atlas14", "Value");
                            clsReportValues.Hour6Start = GetTableValue(iPointID, strHour, "Atlas14", "Start");
                            clsReportValues.Hour6End = GetTableValue(iPointID, strHour, "Atlas14", "End");

                            //Atlas14
                            clsReportValues.Hour6AtlasLowFrequency = GetTableValue(iPointID, strHour, "Atlas14", "LowFreq") + " Year";
                            clsReportValues.Hour6AtlasLowValue = GetTableValue(iPointID, strHour, "Atlas14", "LowVal");
                            clsReportValues.Hour6AtlasHighFrequency = GetTableValue(iPointID, strHour, "Atlas14", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour6AtlasHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour6AtlasHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour6AtlasHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour6AtlasHighFrequency = clsReportValues.Hour6AtlasHighFrequency + " Year";
                            }
                            clsReportValues.Hour6AtlasHighValue = GetTableValue(iPointID, strHour, "Atlas14", "HighVal");

                            //Cloudburst
                            clsReportValues.Hour6CloudBurstLowFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "LowFreq") + " Year";
                            clsReportValues.Hour6CloudBurstLowValue = GetTableValue(iPointID, strHour, "CloudBurst", "LowVal");
                            clsReportValues.Hour6CloudBurstHighFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour6CloudBurstHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour6CloudBurstHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour6CloudBurstHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour6CloudBurstHighFrequency = clsReportValues.Hour6CloudBurstHighFrequency + " Year";
                            }
                            clsReportValues.Hour6CloudBurstHighValue = GetTableValue(iPointID, strHour, "CloudBurst", "HighVal");
                            break;


                        case "12":
                            clsReportValues.Hour12RainTotal = GetTableValue(iPointID, strHour, "Atlas14", "Value");
                            clsReportValues.Hour12Start = GetTableValue(iPointID, strHour, "Atlas14", "Start");
                            clsReportValues.Hour12End = GetTableValue(iPointID, strHour, "Atlas14", "End");

                            //Atlas14
                            clsReportValues.Hour12AtlasLowFrequency = GetTableValue(iPointID, strHour, "Atlas14", "LowFreq") + " Year";
                            clsReportValues.Hour12AtlasLowValue = GetTableValue(iPointID, strHour, "Atlas14", "LowVal");
                            clsReportValues.Hour12AtlasHighFrequency = GetTableValue(iPointID, strHour, "Atlas14", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour12AtlasHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour12AtlasHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour12AtlasHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour12AtlasHighFrequency = clsReportValues.Hour12AtlasHighFrequency + " Year";
                            }
                            clsReportValues.Hour12AtlasHighValue = GetTableValue(iPointID, strHour, "Atlas14", "HighVal");

                            //Cloudburst
                            clsReportValues.Hour12CloudBurstLowFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "LowFreq") + " Year";
                            clsReportValues.Hour12CloudBurstLowValue = GetTableValue(iPointID, strHour, "CloudBurst", "LowVal");
                            clsReportValues.Hour12CloudBurstHighFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour12CloudBurstHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour12CloudBurstHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour12CloudBurstHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour12CloudBurstHighFrequency = clsReportValues.Hour12CloudBurstHighFrequency + " Year";
                            }
                            clsReportValues.Hour12CloudBurstHighValue = GetTableValue(iPointID, strHour, "CloudBurst", "HighVal");
                            break;


                        case "24":
                            clsReportValues.Hour24RainTotal = GetTableValue(iPointID, strHour, "Atlas14", "Value");
                            clsReportValues.Hour24Start = GetTableValue(iPointID, strHour, "Atlas14", "Start");
                            clsReportValues.Hour24End = GetTableValue(iPointID, strHour, "Atlas14", "End");

                            //Atlas14
                            clsReportValues.Hour24AtlasLowFrequency = GetTableValue(iPointID, strHour, "Atlas14", "LowFreq") + " Year";
                            clsReportValues.Hour24AtlasLowValue = GetTableValue(iPointID, strHour, "Atlas14", "LowVal");
                            clsReportValues.Hour24AtlasHighFrequency = GetTableValue(iPointID, strHour, "Atlas14", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour24AtlasHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour24AtlasHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour24AtlasHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour24AtlasHighFrequency = clsReportValues.Hour24AtlasHighFrequency + " Year";
                            }

                            clsReportValues.Hour24AtlasHighValue = GetTableValue(iPointID, strHour, "Atlas14", "HighVal");

                            //Cloudburst
                            clsReportValues.Hour24CloudBurstLowFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "LowFreq") + " Year";
                            clsReportValues.Hour24CloudBurstLowValue = GetTableValue(iPointID, strHour, "CloudBurst", "LowVal");
                            clsReportValues.Hour24CloudBurstHighFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour24CloudBurstHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour24CloudBurstHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour24CloudBurstHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour24CloudBurstHighFrequency = clsReportValues.Hour24CloudBurstHighFrequency + " Year";
                            }
                            clsReportValues.Hour24CloudBurstHighValue = GetTableValue(iPointID, strHour, "CloudBurst", "HighVal");
                            break;


                        case "48":
                            clsReportValues.Hour48RainTotal = GetTableValue(iPointID, strHour, "Atlas14", "Value");
                            clsReportValues.Hour48Start = GetTableValue(iPointID, strHour, "Atlas14", "Start");
                            clsReportValues.Hour48End = GetTableValue(iPointID, strHour, "Atlas14", "End");

                            //Atlas14
                            clsReportValues.Hour48AtlasLowFrequency = GetTableValue(iPointID, strHour, "Atlas14", "LowFreq") + " Year";
                            clsReportValues.Hour48AtlasLowValue = GetTableValue(iPointID, strHour, "Atlas14", "LowVal");
                            clsReportValues.Hour48AtlasHighFrequency = GetTableValue(iPointID, strHour, "Atlas14", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour48AtlasHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour48AtlasHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour48AtlasHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour48AtlasHighFrequency = clsReportValues.Hour48AtlasHighFrequency + " Year";
                            }

                            clsReportValues.Hour48AtlasHighValue = GetTableValue(iPointID, strHour, "Atlas14", "HighVal");

                            //Cloudburst
                            clsReportValues.Hour48CloudBurstLowFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "LowFreq") + " Year";
                            clsReportValues.Hour48CloudBurstLowValue = GetTableValue(iPointID, strHour, "CloudBurst", "LowVal");
                            clsReportValues.Hour48CloudBurstHighFrequency = GetTableValue(iPointID, strHour, "CloudBurst", "HighFreq");
                            Int32.TryParse(clsReportValues.Hour48CloudBurstHighFrequency, out k);
                            if (k > 1000)
                            {
                                clsReportValues.Hour48CloudBurstHighFrequency = "> 1000 Year";
                            }
                            if ((k > 500) && (k < 1000))
                            {
                                clsReportValues.Hour48CloudBurstHighFrequency = "> 500 Year";
                            }
                            if ((k < 500) || (k == 500) || (k == 1000))
                            {
                                clsReportValues.Hour48CloudBurstHighFrequency = clsReportValues.Hour48CloudBurstHighFrequency + " Year";
                            }
                            clsReportValues.Hour48CloudBurstHighValue = GetTableValue(iPointID, strHour, "CloudBurst", "HighVal");
                            break;




                    }


                }





              
            }
            catch (Exception ex)
            {
              
            }

        }


        private string GetTableValue(int iPointID, string strHour, string strStandard, string strFieldName)
        {
            try
            {
                IQueryFilter pq = new QueryFilterClass();
                pq.WhereClause = "Period = '" + strHour + " hr' AND Standard = '" + strStandard + "'";
                ICursor pcurs = _ptable.Search(pq, false);

                IRow prow = pcurs.NextRow();
                string pointid;
                string str = "";


                while (prow != null)
                {
                    pointid = prow.get_Value(prow.Fields.FindField("PointID")).ToString();
                    if (pointid == iPointID.ToString())
                    {
                        str = prow.get_Value(prow.Fields.FindField(strFieldName)).ToString();
                    }
                    prow = pcurs.NextRow();
                }


                pcurs.Flush();

                return str;

            }



            catch
            {
                return "";
            }

        }

        public bool MergePdfFiles(List<string> pdfFiles, string outputPath)
        {


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
                pdfCount = pdfFiles.Count;
                if (pdfCount > 0)
                {
                    //Open the 1st pad using PdfReader object
                    fileName = pdfFiles[f].ToString();
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
                            fileName = pdfFiles[f].ToString();
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
                // logger.LogMessage(ServerLogger.msgType.error, "MergePdfFiles", -1, soe_name + "  " + ex.StackTrace);
            }
            return result;
        }


        private void Cleanup(List<string> lstPDFPages)
        {
            foreach (string strFile in lstPDFPages)
            {
                if(File.Exists(strFile))
                {
                    File.Delete(strFile);
                }

            }
        }



    }

    //class clsclsReportValues
    //{
    //    string strRunDate;
    //    string strRunTime;
    //    string strArea;
    //    string strStartDate;
    //    string strEndDate;
    //    string strAntecedent;

    //    //1 hour 
    //    string str1HourStart;
    //    string str1HourEnd;
    //    string str1HourRainTotal;
    //    string str1HourAtlasLowFrequency;
    //    string str1HourCloudBurstLowFrequency;
    //    string str1HourAtlasLowValue;
    //    string str1HourCloudBurstLowValue;
    //    string str1HourAtlasHighFrequency;
    //    string str1HourCloudBurstHighFrequency;
    //    string str1HourAtlasHighValue;
    //    string str1HourCloudBurstHighValue;


    //    //3 Hour
    //    string str3HourStart;
    //    string str3HourEnd;
    //    string str3HourRainTotal;
    //    string str3HourAtlasLowFrequency;
    //    string str3HourCloudBurstLowFrequency;
    //    string str3HourAtlasLowValue;
    //    string str3HourCloudBurstLowValue;
    //    string str3HourAtlasHighFrequency;
    //    string str3HourCloudBurstHighFrequency;
    //    string str3HourAtlasHighValue;
    //    string str3HourCloudBurstHighValue;

    //    //6 Hour
    //    string str6HourStart;
    //    string str6HourEnd;
    //    string str6HourRainTotal;
    //    string str6HourAtlasLowFrequency;
    //    string str6HourCloudBurstLowFrequency;
    //    string str6HourAtlasLowValue;
    //    string str6HourCloudBurstLowValue;
    //    string str6HourAtlasHighFrequency;
    //    string str6HourCloudBurstHighFrequency;
    //    string str6HourAtlasHighValue;
    //    string str6HourCloudBurstHighValue;


    //    //12 Hour
    //    string str12HourStart;
    //    string str12HourEnd;
    //    string str12HourRainTotal;
    //    string str12HourAtlasLowFrequency;
    //    string str12HourCloudBurstLowFrequency;
    //    string str12HourAtlasLowValue;
    //    string str12HourCloudBurstLowValue;
    //    string str12HourAtlasHighFrequency;
    //    string str12HourCloudBurstHighFrequency;
    //    string str12HourAtlasHighValue;
    //    string str12HourCloudBurstHighValue;

    //    //24 Hour
    //    string str24HourStart;
    //    string str24HourEnd;
    //    string str24HourRainTotal;
    //    string str24HourAtlasLowFrequency;
    //    string str24HourCloudBurstLowFrequency;
    //    string str24HourAtlasLowValue;
    //    string str24HourCloudBurstLowValue;
    //    string str24HourAtlasHighFrequency;
    //    string str24HourCloudBurstHighFrequency;
    //    string str24HourAtlasHighValue;
    //    string str24HourCloudBurstHighValue;


    //    //48 Hour
    //    string str48HourStart;
    //    string str48HourEnd;
    //    string str48HourRainTotal;
    //    string str48HourAtlasLowFrequency;
    //    string str48HourCloudBurstLowFrequency;
    //    string str48HourAtlasLowValue;
    //    string str48HourCloudBurstLowValue;
    //    string str48HourAtlasHighFrequency;
    //    string str48HourCloudBurstHighFrequency;
    //    string str48HourAtlasHighValue;
    //    string str48HourCloudBurstHighValue;



    //    public string SevenDayAntecedent
    //    {
    //        get
    //        {
    //            return strAntecedent;
    //        }
    //        set
    //        {
    //            strAntecedent = value;
    //        }
    //    }

    //    public string RunDate
    //    {
    //        get
    //        {
    //            return strRunDate;
    //        }
    //        set
    //        {

    //            strRunDate = value;

    //        }
    //    }
    //    public string RunTime
    //    {
    //        get
    //        {
    //            return strRunTime;
    //        }
    //        set
    //        {

    //            strRunTime = value;

    //        }
    //    }
    //    public string Area
    //    {
    //        get
    //        {
    //            return strArea;
    //        }
    //        set
    //        {

    //            strArea = value;

    //        }
    //    }
    //    public string StartDate
    //    {
    //        get
    //        {
    //            return strStartDate;
    //        }
    //        set
    //        {

    //            strStartDate = value;

    //        }
    //    }
    //    public string EndDate
    //    {
    //        get
    //        {
    //            return strEndDate;
    //        }
    //        set
    //        {

    //            strEndDate = value;

    //        }
    //    }


    //    #region 1Hour
    //    public string Hour1Start
    //    {
    //        get
    //        {
    //            return str1HourStart;
    //        }
    //        set
    //        {

    //            str1HourStart = value;

    //        }
    //    }
    //    public string Hour1End
    //    {
    //        get
    //        {
    //            return str1HourEnd;
    //        }
    //        set
    //        {

    //            str1HourEnd = value;

    //        }
    //    }
    //    public string Hour1RainTotal
    //    {
    //        get
    //        {
    //            return str1HourRainTotal;
    //        }
    //        set
    //        {

    //            str1HourRainTotal = value;

    //        }
    //    }
    //    public string Hour1AtlasLowFrequency
    //    {
    //        get
    //        {
    //            return str1HourAtlasLowFrequency;
    //        }
    //        set
    //        {

    //            str1HourAtlasLowFrequency = value;

    //        }
    //    }
    //    public string Hour1CloudBurstLowFrequency
    //    {
    //        get
    //        {
    //            return str1HourCloudBurstLowFrequency;
    //        }
    //        set
    //        {

    //            str1HourCloudBurstLowFrequency = value;

    //        }
    //    }
    //    public string Hour1AtlasLowValue
    //    {
    //        get
    //        {
    //            return str1HourAtlasLowValue;
    //        }
    //        set
    //        {

    //            str1HourAtlasLowValue = value;

    //        }
    //    }
    //    public string Hour1CloudBurstLowValue
    //    {
    //        get
    //        {
    //            return str1HourCloudBurstLowValue;
    //        }
    //        set
    //        {

    //            str1HourCloudBurstLowValue = value;

    //        }
    //    }
    //    public string Hour1AtlasHighFrequency
    //    {
    //        get
    //        {
    //            return str1HourAtlasHighFrequency;
    //        }
    //        set
    //        {

    //            str1HourAtlasHighFrequency = value;

    //        }
    //    }
    //    public string Hour1CloudBurstHighFrequency
    //    {
    //        get
    //        {
    //            return str1HourCloudBurstHighFrequency;
    //        }
    //        set
    //        {

    //            str1HourCloudBurstHighFrequency = value;

    //        }
    //    }
    //    public string Hour1AtlasHighValue
    //    {
    //        get
    //        {
    //            return str1HourAtlasHighValue;
    //        }
    //        set
    //        {

    //            str1HourAtlasHighValue = value;

    //        }
    //    }
    //    public string Hour1CloudBurstHighValue
    //    {
    //        get
    //        {
    //            return str1HourCloudBurstHighValue;
    //        }
    //        set
    //        {

    //            str1HourCloudBurstHighValue = value;

    //        }
    //    }
    //    #endregion

    //    #region 3Hour
    //    public string Hour3Start
    //    {
    //        get
    //        {
    //            return str3HourStart;
    //        }
    //        set
    //        {

    //            str3HourStart = value;

    //        }
    //    }
    //    public string Hour3End
    //    {
    //        get
    //        {
    //            return str3HourEnd;
    //        }
    //        set
    //        {

    //            str3HourEnd = value;

    //        }
    //    }
    //    public string Hour3RainTotal
    //    {
    //        get
    //        {
    //            return str3HourRainTotal;
    //        }
    //        set
    //        {

    //            str3HourRainTotal = value;

    //        }
    //    }
    //    public string Hour3AtlasLowFrequency
    //    {
    //        get
    //        {
    //            return str3HourAtlasLowFrequency;
    //        }
    //        set
    //        {

    //            str3HourAtlasLowFrequency = value;

    //        }
    //    }
    //    public string Hour3CloudBurstLowFrequency
    //    {
    //        get
    //        {
    //            return str3HourCloudBurstLowFrequency;
    //        }
    //        set
    //        {

    //            str3HourCloudBurstLowFrequency = value;

    //        }
    //    }
    //    public string Hour3AtlasLowValue
    //    {
    //        get
    //        {
    //            return str3HourAtlasLowValue;
    //        }
    //        set
    //        {

    //            str3HourAtlasLowValue = value;

    //        }
    //    }
    //    public string Hour3CloudBurstLowValue
    //    {
    //        get
    //        {
    //            return str3HourCloudBurstLowValue;
    //        }
    //        set
    //        {

    //            str3HourCloudBurstLowValue = value;

    //        }
    //    }
    //    public string Hour3AtlasHighFrequency
    //    {
    //        get
    //        {
    //            return str3HourAtlasHighFrequency;
    //        }
    //        set
    //        {

    //            str3HourAtlasHighFrequency = value;

    //        }
    //    }
    //    public string Hour3CloudBurstHighFrequency
    //    {
    //        get
    //        {
    //            return str3HourCloudBurstHighFrequency;
    //        }
    //        set
    //        {

    //            str3HourCloudBurstHighFrequency = value;

    //        }
    //    }
    //    public string Hour3AtlasHighValue
    //    {
    //        get
    //        {
    //            return str3HourAtlasHighValue;
    //        }
    //        set
    //        {

    //            str3HourAtlasHighValue = value;

    //        }
    //    }
    //    public string Hour3CloudBurstHighValue
    //    {
    //        get
    //        {
    //            return str3HourCloudBurstHighValue;
    //        }
    //        set
    //        {

    //            str3HourCloudBurstHighValue = value;

    //        }
    //    }
    //    #endregion


    //    #region 6Hour
    //    public string Hour6Start
    //    {
    //        get
    //        {
    //            return str6HourStart;
    //        }
    //        set
    //        {

    //            str6HourStart = value;

    //        }
    //    }
    //    public string Hour6End
    //    {
    //        get
    //        {
    //            return str6HourEnd;
    //        }
    //        set
    //        {

    //            str6HourEnd = value;

    //        }
    //    }
    //    public string Hour6RainTotal
    //    {
    //        get
    //        {
    //            return str6HourRainTotal;
    //        }
    //        set
    //        {

    //            str6HourRainTotal = value;

    //        }
    //    }
    //    public string Hour6AtlasLowFrequency
    //    {
    //        get
    //        {
    //            return str6HourAtlasLowFrequency;
    //        }
    //        set
    //        {

    //            str6HourAtlasLowFrequency = value;

    //        }
    //    }
    //    public string Hour6CloudBurstLowFrequency
    //    {
    //        get
    //        {
    //            return str6HourCloudBurstLowFrequency;
    //        }
    //        set
    //        {

    //            str6HourCloudBurstLowFrequency = value;

    //        }
    //    }
    //    public string Hour6AtlasLowValue
    //    {
    //        get
    //        {
    //            return str6HourAtlasLowValue;
    //        }
    //        set
    //        {

    //            str6HourAtlasLowValue = value;

    //        }
    //    }
    //    public string Hour6CloudBurstLowValue
    //    {
    //        get
    //        {
    //            return str6HourCloudBurstLowValue;
    //        }
    //        set
    //        {

    //            str6HourCloudBurstLowValue = value;

    //        }
    //    }
    //    public string Hour6AtlasHighFrequency
    //    {
    //        get
    //        {
    //            return str6HourAtlasHighFrequency;
    //        }
    //        set
    //        {

    //            str6HourAtlasHighFrequency = value;

    //        }
    //    }
    //    public string Hour6CloudBurstHighFrequency
    //    {
    //        get
    //        {
    //            return str6HourCloudBurstHighFrequency;
    //        }
    //        set
    //        {

    //            str6HourCloudBurstHighFrequency = value;

    //        }
    //    }
    //    public string Hour6AtlasHighValue
    //    {
    //        get
    //        {
    //            return str6HourAtlasHighValue;
    //        }
    //        set
    //        {

    //            str6HourAtlasHighValue = value;

    //        }
    //    }
    //    public string Hour6CloudBurstHighValue
    //    {
    //        get
    //        {
    //            return str6HourCloudBurstHighValue;
    //        }
    //        set
    //        {

    //            str6HourCloudBurstHighValue = value;

    //        }
    //    }
    //    #endregion

    //    #region 12Hour
    //    public string Hour12Start
    //    {
    //        get
    //        {
    //            return str12HourStart;
    //        }
    //        set
    //        {

    //            str12HourStart = value;

    //        }
    //    }
    //    public string Hour12End
    //    {
    //        get
    //        {
    //            return str12HourEnd;
    //        }
    //        set
    //        {

    //            str12HourEnd = value;

    //        }
    //    }
    //    public string Hour12RainTotal
    //    {
    //        get
    //        {
    //            return str12HourRainTotal;
    //        }
    //        set
    //        {

    //            str12HourRainTotal = value;

    //        }
    //    }
    //    public string Hour12AtlasLowFrequency
    //    {
    //        get
    //        {
    //            return str12HourAtlasLowFrequency;
    //        }
    //        set
    //        {

    //            str12HourAtlasLowFrequency = value;

    //        }
    //    }
    //    public string Hour12CloudBurstLowFrequency
    //    {
    //        get
    //        {
    //            return str12HourCloudBurstLowFrequency;
    //        }
    //        set
    //        {

    //            str12HourCloudBurstLowFrequency = value;

    //        }
    //    }
    //    public string Hour12AtlasLowValue
    //    {
    //        get
    //        {
    //            return str12HourAtlasLowValue;
    //        }
    //        set
    //        {

    //            str12HourAtlasLowValue = value;

    //        }
    //    }
    //    public string Hour12CloudBurstLowValue
    //    {
    //        get
    //        {
    //            return str12HourCloudBurstLowValue;
    //        }
    //        set
    //        {

    //            str12HourCloudBurstLowValue = value;

    //        }
    //    }
    //    public string Hour12AtlasHighFrequency
    //    {
    //        get
    //        {
    //            return str12HourAtlasHighFrequency;
    //        }
    //        set
    //        {

    //            str12HourAtlasHighFrequency = value;

    //        }
    //    }
    //    public string Hour12CloudBurstHighFrequency
    //    {
    //        get
    //        {
    //            return str12HourCloudBurstHighFrequency;
    //        }
    //        set
    //        {

    //            str12HourCloudBurstHighFrequency = value;

    //        }
    //    }
    //    public string Hour12AtlasHighValue
    //    {
    //        get
    //        {
    //            return str12HourAtlasHighValue;
    //        }
    //        set
    //        {

    //            str12HourAtlasHighValue = value;

    //        }
    //    }
    //    public string Hour12CloudBurstHighValue
    //    {
    //        get
    //        {
    //            return str12HourCloudBurstHighValue;
    //        }
    //        set
    //        {

    //            str12HourCloudBurstHighValue = value;

    //        }
    //    }
    //    #endregion

    //    #region 24Hour
    //    public string Hour24Start
    //    {
    //        get
    //        {
    //            return str24HourStart;
    //        }
    //        set
    //        {

    //            str24HourStart = value;

    //        }
    //    }
    //    public string Hour24End
    //    {
    //        get
    //        {
    //            return str24HourEnd;
    //        }
    //        set
    //        {

    //            str24HourEnd = value;

    //        }
    //    }
    //    public string Hour24RainTotal
    //    {
    //        get
    //        {
    //            return str24HourRainTotal;
    //        }
    //        set
    //        {

    //            str24HourRainTotal = value;

    //        }
    //    }
    //    public string Hour24AtlasLowFrequency
    //    {
    //        get
    //        {
    //            return str24HourAtlasLowFrequency;
    //        }
    //        set
    //        {

    //            str24HourAtlasLowFrequency = value;

    //        }
    //    }
    //    public string Hour24CloudBurstLowFrequency
    //    {
    //        get
    //        {
    //            return str24HourCloudBurstLowFrequency;
    //        }
    //        set
    //        {

    //            str24HourCloudBurstLowFrequency = value;

    //        }
    //    }
    //    public string Hour24AtlasLowValue
    //    {
    //        get
    //        {
    //            return str24HourAtlasLowValue;
    //        }
    //        set
    //        {

    //            str24HourAtlasLowValue = value;

    //        }
    //    }
    //    public string Hour24CloudBurstLowValue
    //    {
    //        get
    //        {
    //            return str24HourCloudBurstLowValue;
    //        }
    //        set
    //        {

    //            str24HourCloudBurstLowValue = value;

    //        }
    //    }
    //    public string Hour24AtlasHighFrequency
    //    {
    //        get
    //        {
    //            return str24HourAtlasHighFrequency;
    //        }
    //        set
    //        {

    //            str24HourAtlasHighFrequency = value;

    //        }
    //    }
    //    public string Hour24CloudBurstHighFrequency
    //    {
    //        get
    //        {
    //            return str24HourCloudBurstHighFrequency;
    //        }
    //        set
    //        {

    //            str24HourCloudBurstHighFrequency = value;

    //        }
    //    }
    //    public string Hour24AtlasHighValue
    //    {
    //        get
    //        {
    //            return str24HourAtlasHighValue;
    //        }
    //        set
    //        {

    //            str24HourAtlasHighValue = value;

    //        }
    //    }
    //    public string Hour24CloudBurstHighValue
    //    {
    //        get
    //        {
    //            return str24HourCloudBurstHighValue;
    //        }
    //        set
    //        {

    //            str24HourCloudBurstHighValue = value;

    //        }
    //    }
    //    #endregion

    //    #region 48Hour
    //    public string Hour48Start
    //    {
    //        get
    //        {
    //            return str48HourStart;
    //        }
    //        set
    //        {

    //            str48HourStart = value;

    //        }
    //    }
    //    public string Hour48End
    //    {
    //        get
    //        {
    //            return str48HourEnd;
    //        }
    //        set
    //        {

    //            str48HourEnd = value;

    //        }
    //    }
    //    public string Hour48RainTotal
    //    {
    //        get
    //        {
    //            return str48HourRainTotal;
    //        }
    //        set
    //        {

    //            str48HourRainTotal = value;

    //        }
    //    }
    //    public string Hour48AtlasLowFrequency
    //    {
    //        get
    //        {
    //            return str48HourAtlasLowFrequency;
    //        }
    //        set
    //        {

    //            str48HourAtlasLowFrequency = value;

    //        }
    //    }
    //    public string Hour48CloudBurstLowFrequency
    //    {
    //        get
    //        {
    //            return str48HourCloudBurstLowFrequency;
    //        }
    //        set
    //        {

    //            str48HourCloudBurstLowFrequency = value;

    //        }
    //    }
    //    public string Hour48AtlasLowValue
    //    {
    //        get
    //        {
    //            return str48HourAtlasLowValue;
    //        }
    //        set
    //        {

    //            str48HourAtlasLowValue = value;

    //        }
    //    }
    //    public string Hour48CloudBurstLowValue
    //    {
    //        get
    //        {
    //            return str48HourCloudBurstLowValue;
    //        }
    //        set
    //        {

    //            str48HourCloudBurstLowValue = value;

    //        }
    //    }
    //    public string Hour48AtlasHighFrequency
    //    {
    //        get
    //        {
    //            return str48HourAtlasHighFrequency;
    //        }
    //        set
    //        {

    //            str48HourAtlasHighFrequency = value;

    //        }
    //    }
    //    public string Hour48CloudBurstHighFrequency
    //    {
    //        get
    //        {
    //            return str48HourCloudBurstHighFrequency;
    //        }
    //        set
    //        {

    //            str48HourCloudBurstHighFrequency = value;

    //        }
    //    }
    //    public string Hour48AtlasHighValue
    //    {
    //        get
    //        {
    //            return str48HourAtlasHighValue;
    //        }
    //        set
    //        {

    //            str48HourAtlasHighValue = value;

    //        }
    //    }
    //    public string Hour48CloudBurstHighValue
    //    {
    //        get
    //        {
    //            return str48HourCloudBurstHighValue;
    //        }
    //        set
    //        {

    //            str48HourCloudBurstHighValue = value;

    //        }
    //    }
    //    #endregion


    //}

}
