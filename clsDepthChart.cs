using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StormFrequencyCalculator
{

    class clsDepthChart
    {

       public List<DepthChart> lst1HourAtlas14, lst3HourAtlas14, lst6HourAtlas14, lst12HourAtlas14, lst24HourAtlas14, lst48HourAtlas14;
       public List<DepthChart> lst1HourCloudBurst, lst3HourCloudBurst, lst6HourCloudBurst, lst12HourCloudBurst, lst24HourCloudBurst, lst48HourCloudBurst;


       public clsDepthChart()
       {
           
           lst1HourAtlas14 = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 1.16, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 1.16 , HighVal = 1.39, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 1.39 , HighVal = 1.71, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 1.71 , HighVal = 1.96, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 1.96 , HighVal = 2.3, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 2.3 , HighVal = 2.57, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 2.57 , HighVal = 2.85, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 2.85 , HighVal = 3.13, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 200, LowVal = 3.13 , HighVal = 3.52, lowYear = 200, highYear = 500},
            new DepthChart { FregNum = 500, LowVal = 3.52 , HighVal = 3.83, lowYear = 500, highYear = 1000},
            new DepthChart { FregNum = 1000, LowVal = 3.83 , HighVal = 100, lowYear = 1000, highYear = 1000000}            
            };

           lst3HourAtlas14 = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 1.51, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 1.51 , HighVal = 1.81, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 1.81, HighVal = 2.23, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 2.23 , HighVal = 2.58, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 2.58 , HighVal = 3.06, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 3.06 , HighVal = 3.46, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 3.46 , HighVal = 3.89, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 3.89 , HighVal = 4.33, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 200, LowVal = 4.33 , HighVal = 4.96, lowYear = 200, highYear = 500},
            new DepthChart { FregNum = 500, LowVal = 4.96 , HighVal = 5.48, lowYear = 500, highYear = 1000},
            new DepthChart { FregNum = 1000, LowVal = 5.48 , HighVal = 100, lowYear = 1000, highYear = 1000000}            
            };

           lst6HourAtlas14 = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 1.84, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 1.84 , HighVal = 2.21, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 2.21, HighVal = 2.72, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 2.72 , HighVal = 3.15, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 3.15 , HighVal = 3.77, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 3.77 , HighVal = 4.28, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 4.28 , HighVal = 4.83, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 4.83 , HighVal = 5.41, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 200, LowVal = 5.41 , HighVal = 6.24, lowYear = 200, highYear = 500},
            new DepthChart { FregNum = 500, LowVal = 6.24 , HighVal = 6.93, lowYear = 500, highYear = 1000},
            new DepthChart { FregNum = 1000, LowVal = 6.93 , HighVal = 100, lowYear = 1000, highYear = 1000000}            
            };

           lst12HourAtlas14 = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 2.19, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 2.19 , HighVal = 2.62, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 2.62, HighVal = 3.23, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 3.23 , HighVal = 3.74, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 3.74 , HighVal = 4.46, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 4.46 , HighVal = 5.05, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 5.05 , HighVal = 5.7, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 5.7 , HighVal = 6.38, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 200, LowVal = 6.38 , HighVal = 7.35, lowYear = 200, highYear = 500},
            new DepthChart { FregNum = 500, LowVal = 7.35 , HighVal = 8.15, lowYear = 500, highYear = 1000},
            new DepthChart { FregNum = 1000, LowVal = 8.15 , HighVal = 100, lowYear = 1000, highYear = 1000000}            
            };

           lst24HourAtlas14 = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 2.61, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 2.61 , HighVal = 3.13, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 3.13, HighVal = 3.89, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 3.89 , HighVal = 4.52, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 4.52 , HighVal = 5.43, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 5.43 , HighVal = 6.19, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 6.19 , HighVal = 7.01, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 7.01 , HighVal = 7.91, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 200, LowVal = 7.91 , HighVal = 9.2, lowYear = 200, highYear = 500},
            new DepthChart { FregNum = 500, LowVal = 9.2 , HighVal = 10.28, lowYear = 500, highYear = 1000},
            new DepthChart { FregNum = 1000, LowVal = 10.28 , HighVal = 100, lowYear = 1000, highYear = 1000000}            
            };


           lst48HourAtlas14 = new List<DepthChart> { 
           new DepthChart { FregNum = 0, LowVal = 0, HighVal = 3.09, lowYear = 0, highYear = 1},
           new DepthChart { FregNum = 1, LowVal = 3.09 , HighVal = 3.71, lowYear = 1, highYear = 2},
           new DepthChart { FregNum = 2, LowVal = 3.71, HighVal = 4.58, lowYear = 2, highYear = 3},
           new DepthChart { FregNum = 5, LowVal = 4.58 , HighVal = 5.3, lowYear = 5, highYear = 10},
           new DepthChart { FregNum = 10, LowVal = 5.3 , HighVal = 6.33, lowYear = 10, highYear = 25},
           new DepthChart { FregNum = 25, LowVal = 6.33 , HighVal = 7.17, lowYear = 25, highYear = 50},
           new DepthChart { FregNum = 50, LowVal = 7.17 , HighVal = 8.08, lowYear = 50, highYear = 100},
           new DepthChart { FregNum = 100, LowVal = 9.04 , HighVal = 7.91, lowYear = 100, highYear = 200},
           new DepthChart { FregNum = 200, LowVal = 7.91 , HighVal = 9.2, lowYear = 200, highYear = 500},
           new DepthChart { FregNum = 500, LowVal = 9.2 , HighVal = 10.28, lowYear = 500, highYear = 1000},
           new DepthChart { FregNum = 1000, LowVal = 10.28 , HighVal = 100, lowYear = 1000, highYear = 1000000}            
           };


           ////////////////////////


           lst1HourCloudBurst  = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 1.15, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 1.15 , HighVal = 1.38, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 1.38, HighVal = 1.7, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 1.7 , HighVal = 1.96, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 1.96 , HighVal = 2.3, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 2.3 , HighVal = 2.57, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 2.57 , HighVal = 2.85, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 2.85 , HighVal = 3.53, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 500, LowVal = 3.53 , HighVal = 100, lowYear = 200, highYear = 500}
            };



           lst3HourCloudBurst = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 1.52, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 1.52 , HighVal = 1.82, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 1.82, HighVal = 2.25, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 2.25 , HighVal = 2.6, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 2.6 , HighVal = 3.08, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 3.08 , HighVal = 3.47, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 3.47 , HighVal = 3.87, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 3.87 , HighVal = 4.88, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 500, LowVal = 4.88 , HighVal = 100, lowYear = 200, highYear = 500}
            };


           lst6HourCloudBurst = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 1.81, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 1.81 , HighVal = 2.17, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 2.17, HighVal = 2.69, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 2.69 , HighVal = 3.12, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 3.12 , HighVal = 3.71, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 3.71 , HighVal = 4.19, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 4.19 , HighVal = 4.7, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 4.7 , HighVal = 5.99, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 500, LowVal = 5.99 , HighVal = 100, lowYear = 200, highYear = 500}
            };

           lst12HourCloudBurst = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 2.16, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 2.16 , HighVal = 2.59, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 2.59, HighVal = 3.21, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 3.21 , HighVal = 3.73, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 3.73 , HighVal = 4.46, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 4.46 , HighVal = 5.06, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 5.06 , HighVal = 5.7, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 5.7 , HighVal = 7.35, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 500, LowVal = 7.35 , HighVal = 100, lowYear = 200, highYear = 500}
            };

           lst24HourCloudBurst = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 2.58, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 2.58 , HighVal = 3.09, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 3.09, HighVal = 3.84, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 3.84 , HighVal = 4.46, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 4.46 , HighVal = 5.36, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 5.36 , HighVal = 6.11, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 6.11 , HighVal = 6.91, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 6.91 , HighVal = 9.02, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 500, LowVal = 9.02 , HighVal = 100, lowYear = 200, highYear = 500}
            };


           lst48HourCloudBurst = new List<DepthChart> { 
            new DepthChart { FregNum = 0, LowVal = 0, HighVal = 3.08, lowYear = 0, highYear = 1},
            new DepthChart { FregNum = 1, LowVal = 3.08 , HighVal = 3.68, lowYear = 1, highYear = 2},
            new DepthChart { FregNum = 2, LowVal = 3.68, HighVal = 4.59, lowYear = 2, highYear = 5},
            new DepthChart { FregNum = 5, LowVal = 4.59 , HighVal = 5.34, lowYear = 5, highYear = 10},
            new DepthChart { FregNum = 10, LowVal = 5.34 , HighVal = 6.45, lowYear = 10, highYear = 25},
            new DepthChart { FregNum = 25, LowVal = 6.45 , HighVal = 7.38, lowYear = 25, highYear = 50},
            new DepthChart { FregNum = 50, LowVal = 7.38 , HighVal = 8.38, lowYear = 50, highYear = 100},
            new DepthChart { FregNum = 100, LowVal = 8.38 , HighVal = 11.07, lowYear = 100, highYear = 200},
            new DepthChart { FregNum = 500, LowVal = 11.07, HighVal = 100, lowYear = 200, highYear = 500}
            };

       }
    }

   public class DepthChart
    {
        public int FregNum;
        public double LowVal;
        public double HighVal;
        public int lowYear;
        public int highYear;

    }
}
