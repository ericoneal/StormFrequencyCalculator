using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using Microsoft.Win32;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Output;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Framework;

namespace StormFrequencyCalculator
{
    class clsExport
    {

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






        IMapDocument mapdoc;
        public string MakeLocationMap(IFeature pFeature, IMapDocument mapdoc)
        {
            //RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\\LOJICReg\\MSD");
            //string strMXD = regkey.GetValue("SupportFiles").ToString() + @"\AddinsSupportFiles\StormFrequencyCalculator\LocationMap.mxd";


            //File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\LocationMap.mxd", StormFrequencyCalculator.Properties.Resources.LocationMap);
            //string strMXD = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\LocationMap.mxd";

            //mapdoc = new MapDocumentClass();
            //mapdoc.Open(strMXD);


            mapdoc.ActiveView.Extent = mapdoc.ActiveView.FullExtent;


            IStyleGallery SG = new StyleGalleryClass();
            IStyleGalleryItem pSI = new StyleGalleryItemClass();
            IStyleGalleryStorage pSGS;
            IEnumStyleGalleryItem pItems;
            IMarkerElement pMarkerElement = new MarkerElementClass();
            pItems = new EnumStyleGalleryItemClass();
            pSGS = (IStyleGalleryStorage)SG;
            string pStylePath;
            pStylePath = pSGS.DefaultStylePath + "ESRI.Style";
            pItems = SG.get_Items("Marker Symbols", pStylePath, "Default");

            pItems.Reset();
            pSI = pItems.Next();

            IMarkerSymbol markerSymbol = null;

            while (pSI != null)
            {
                if (pSI.Name == "Star 3")
                {
                    //pMarkerElement.Symbol = pSI.Item as IMarkerSymbol;
                    markerSymbol = pSI.Item as IMarkerSymbol;
                    markerSymbol.Size = 50;
                    break;
                }
                else
                {
                    pSI = pItems.Next();
                }
            }


            IPoint pPoint = new PointClass();
            pPoint = pFeature.Shape as IPoint;


            IElement pelement;
            pelement = pMarkerElement as IElement;
            pelement.Geometry = pPoint;

            pMarkerElement.Symbol = markerSymbol;

            IGraphicsContainer pgraphics = mapdoc.ActiveView.FocusMap as IGraphicsContainer;
            pgraphics.DeleteAllElements();
            pgraphics.AddElement(pelement, 0);


            mapdoc.ActiveView.Activate(GetDesktopWindow());
            mapdoc.SetActiveView(mapdoc.ActiveView);
            mapdoc.ActiveView.Refresh();


            string strOutFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".jpg");
            ExportJPG(strOutFile, 144, mapdoc);


            return strOutFile;

        }




        public void ExportJPG(string sExportPath, short lDPI, IMapDocument mapdoc)
        {
            try
            {
                IExporter pExporter = new JpegExporterClass();
                IEnvelope pPixelEnv = new EnvelopeClass();

                IEnvelope penv = new EnvelopeClass();
                penv.PutCoords(0.0, 0.0, 11, 11);

                pPixelEnv.PutCoords(0, 0, lDPI * penv.UpperRight.X, lDPI * penv.UpperRight.Y);

                pExporter.PixelBounds = pPixelEnv;
                pExporter.Resolution = lDPI;
                pExporter.ExportFileName = sExportPath;
                pExporter.ClipToGraphicExtent = false;

                tagRECT tExpRect;
                tExpRect.left = Convert.ToInt32(pExporter.PixelBounds.LowerLeft.X);
                tExpRect.bottom = Convert.ToInt32(pExporter.PixelBounds.UpperRight.Y);
                tExpRect.right = Convert.ToInt32(pExporter.PixelBounds.UpperRight.X);
                tExpRect.top = Convert.ToInt32(pExporter.PixelBounds.LowerLeft.Y);

                int hDc = pExporter.StartExporting();

                mapdoc.ActiveView.Output(hDc, lDPI, ref tExpRect, null, null);
                pExporter.FinishExporting();
                hDc = 0;
            }

            catch (Exception ex)
            {

            }
        }


      


    }
}
