using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StormFrequencyCalculator
{

    public static class clsReportValues
    {
        static string strRunDate;
        static string strRunTime;
        static string strArea;
        static string strStartDate;
        static string strEndDate;
        static string strAntecedent;
        static string strPeriodTotal;
        static Dictionary<string, string> dicAntecedent = new Dictionary<string, string>();

        //1 hour 
        static string str1HourStart;
        static string str1HourEnd;
        static string str1HourRainTotal;
        static string str1HourAtlasLowFrequency;
        static string str1HourCloudBurstLowFrequency;
        static string str1HourAtlasLowValue;
        static string str1HourCloudBurstLowValue;
        static string str1HourAtlasHighFrequency;
        static string str1HourCloudBurstHighFrequency;
        static string str1HourAtlasHighValue;
        static string str1HourCloudBurstHighValue;


        //3 Hour
        static string str3HourStart;
        static string str3HourEnd;
        static string str3HourRainTotal;
        static string str3HourAtlasLowFrequency;
        static string str3HourCloudBurstLowFrequency;
        static string str3HourAtlasLowValue;
        static string str3HourCloudBurstLowValue;
        static string str3HourAtlasHighFrequency;
        static string str3HourCloudBurstHighFrequency;
        static string str3HourAtlasHighValue;
        static string str3HourCloudBurstHighValue;

        //6 Hour
        static string str6HourStart;
        static string str6HourEnd;
        static string str6HourRainTotal;
        static string str6HourAtlasLowFrequency;
        static string str6HourCloudBurstLowFrequency;
        static string str6HourAtlasLowValue;
        static string str6HourCloudBurstLowValue;
        static string str6HourAtlasHighFrequency;
        static string str6HourCloudBurstHighFrequency;
        static string str6HourAtlasHighValue;
        static string str6HourCloudBurstHighValue;


        //12 Hour
        static string str12HourStart;
        static string str12HourEnd;
        static string str12HourRainTotal;
        static string str12HourAtlasLowFrequency;
        static string str12HourCloudBurstLowFrequency;
        static string str12HourAtlasLowValue;
        static string str12HourCloudBurstLowValue;
        static string str12HourAtlasHighFrequency;
        static string str12HourCloudBurstHighFrequency;
        static string str12HourAtlasHighValue;
        static string str12HourCloudBurstHighValue;

        //24 Hour
        static string str24HourStart;
        static string str24HourEnd;
        static string str24HourRainTotal;
        static string str24HourAtlasLowFrequency;
        static string str24HourCloudBurstLowFrequency;
        static string str24HourAtlasLowValue;
        static string str24HourCloudBurstLowValue;
        static string str24HourAtlasHighFrequency;
        static string str24HourCloudBurstHighFrequency;
        static string str24HourAtlasHighValue;
        static string str24HourCloudBurstHighValue;


        //48 Hour
        static string str48HourStart;
        static string str48HourEnd;
        static string str48HourRainTotal;
        static string str48HourAtlasLowFrequency;
        static string str48HourCloudBurstLowFrequency;
        static string str48HourAtlasLowValue;
        static string str48HourCloudBurstLowValue;
        static string str48HourAtlasHighFrequency;
        static string str48HourCloudBurstHighFrequency;
        static string str48HourAtlasHighValue;
        static string str48HourCloudBurstHighValue;



        public static Dictionary<string, string> AntedecentList 
        { 
            get 
            { 
                return dicAntecedent; 
            } 
        }

        public static string SevenDayAntecedent
        {
            get
            {
                return strAntecedent;
            }
            set
            {
                strAntecedent = value;
            }
        }
        public static string PeriodTotal
        {
            get
            {
                return strPeriodTotal;
            }
            set
            {
                strPeriodTotal = value;
            }
        }

        public static string RunDate
        {
            get
            {
                return strRunDate;
            }
            set
            {

                strRunDate = value;

            }
        }
        public static string RunTime
        {
            get
            {
                return strRunTime;
            }
            set
            {

                strRunTime = value;

            }
        }
        public static string Area
        {
            get
            {
                return strArea;
            }
            set
            {

                strArea = value;

            }
        }
        public static string StartDate
        {
            get
            {
                return strStartDate;
            }
            set
            {

                strStartDate = value;

            }
        }
        public static string EndDate
        {
            get
            {
                return strEndDate;
            }
            set
            {

                strEndDate = value;

            }
        }


        #region 1Hour
        public static string Hour1Start
        {
            get
            {
                return str1HourStart;
            }
            set
            {

                str1HourStart = value;

            }
        }
        public static string Hour1End
        {
            get
            {
                return str1HourEnd;
            }
            set
            {

                str1HourEnd = value;

            }
        }
        public static string Hour1RainTotal
        {
            get
            {
                return str1HourRainTotal;
            }
            set
            {

                str1HourRainTotal = value;

            }
        }
        public static string Hour1AtlasLowFrequency
        {
            get
            {
                return str1HourAtlasLowFrequency;
            }
            set
            {

                str1HourAtlasLowFrequency = value;

            }
        }
        public static string Hour1CloudBurstLowFrequency
        {
            get
            {
                return str1HourCloudBurstLowFrequency;
            }
            set
            {

                str1HourCloudBurstLowFrequency = value;

            }
        }
        public static string Hour1AtlasLowValue
        {
            get
            {
                return str1HourAtlasLowValue;
            }
            set
            {

                str1HourAtlasLowValue = value;

            }
        }
        public static string Hour1CloudBurstLowValue
        {
            get
            {
                return str1HourCloudBurstLowValue;
            }
            set
            {

                str1HourCloudBurstLowValue = value;

            }
        }
        public static string Hour1AtlasHighFrequency
        {
            get
            {
                return str1HourAtlasHighFrequency;
            }
            set
            {

                str1HourAtlasHighFrequency = value;

            }
        }
        public static string Hour1CloudBurstHighFrequency
        {
            get
            {
                return str1HourCloudBurstHighFrequency;
            }
            set
            {

                str1HourCloudBurstHighFrequency = value;

            }
        }
        public static string Hour1AtlasHighValue
        {
            get
            {
                return str1HourAtlasHighValue;
            }
            set
            {

                str1HourAtlasHighValue = value;

            }
        }
        public static string Hour1CloudBurstHighValue
        {
            get
            {
                return str1HourCloudBurstHighValue;
            }
            set
            {

                str1HourCloudBurstHighValue = value;

            }
        }
        #endregion

        #region 3Hour
        public static string Hour3Start
        {
            get
            {
                return str3HourStart;
            }
            set
            {

                str3HourStart = value;

            }
        }
        public static string Hour3End
        {
            get
            {
                return str3HourEnd;
            }
            set
            {

                str3HourEnd = value;

            }
        }
        public static string Hour3RainTotal
        {
            get
            {
                return str3HourRainTotal;
            }
            set
            {

                str3HourRainTotal = value;

            }
        }
        public static string Hour3AtlasLowFrequency
        {
            get
            {
                return str3HourAtlasLowFrequency;
            }
            set
            {

                str3HourAtlasLowFrequency = value;

            }
        }
        public static string Hour3CloudBurstLowFrequency
        {
            get
            {
                return str3HourCloudBurstLowFrequency;
            }
            set
            {

                str3HourCloudBurstLowFrequency = value;

            }
        }
        public static string Hour3AtlasLowValue
        {
            get
            {
                return str3HourAtlasLowValue;
            }
            set
            {

                str3HourAtlasLowValue = value;

            }
        }
        public static string Hour3CloudBurstLowValue
        {
            get
            {
                return str3HourCloudBurstLowValue;
            }
            set
            {

                str3HourCloudBurstLowValue = value;

            }
        }
        public static string Hour3AtlasHighFrequency
        {
            get
            {
                return str3HourAtlasHighFrequency;
            }
            set
            {

                str3HourAtlasHighFrequency = value;

            }
        }
        public static string Hour3CloudBurstHighFrequency
        {
            get
            {
                return str3HourCloudBurstHighFrequency;
            }
            set
            {

                str3HourCloudBurstHighFrequency = value;

            }
        }
        public static string Hour3AtlasHighValue
        {
            get
            {
                return str3HourAtlasHighValue;
            }
            set
            {

                str3HourAtlasHighValue = value;

            }
        }
        public static string Hour3CloudBurstHighValue
        {
            get
            {
                return str3HourCloudBurstHighValue;
            }
            set
            {

                str3HourCloudBurstHighValue = value;

            }
        }
        #endregion


        #region 6Hour
        public static string Hour6Start
        {
            get
            {
                return str6HourStart;
            }
            set
            {

                str6HourStart = value;

            }
        }
        public static string Hour6End
        {
            get
            {
                return str6HourEnd;
            }
            set
            {

                str6HourEnd = value;

            }
        }
        public static string Hour6RainTotal
        {
            get
            {
                return str6HourRainTotal;
            }
            set
            {

                str6HourRainTotal = value;

            }
        }
        public static string Hour6AtlasLowFrequency
        {
            get
            {
                return str6HourAtlasLowFrequency;
            }
            set
            {

                str6HourAtlasLowFrequency = value;

            }
        }
        public static string Hour6CloudBurstLowFrequency
        {
            get
            {
                return str6HourCloudBurstLowFrequency;
            }
            set
            {

                str6HourCloudBurstLowFrequency = value;

            }
        }
        public static string Hour6AtlasLowValue
        {
            get
            {
                return str6HourAtlasLowValue;
            }
            set
            {

                str6HourAtlasLowValue = value;

            }
        }
        public static string Hour6CloudBurstLowValue
        {
            get
            {
                return str6HourCloudBurstLowValue;
            }
            set
            {

                str6HourCloudBurstLowValue = value;

            }
        }
        public static string Hour6AtlasHighFrequency
        {
            get
            {
                return str6HourAtlasHighFrequency;
            }
            set
            {

                str6HourAtlasHighFrequency = value;

            }
        }
        public static string Hour6CloudBurstHighFrequency
        {
            get
            {
                return str6HourCloudBurstHighFrequency;
            }
            set
            {

                str6HourCloudBurstHighFrequency = value;

            }
        }
        public static string Hour6AtlasHighValue
        {
            get
            {
                return str6HourAtlasHighValue;
            }
            set
            {

                str6HourAtlasHighValue = value;

            }
        }
        public static string Hour6CloudBurstHighValue
        {
            get
            {
                return str6HourCloudBurstHighValue;
            }
            set
            {

                str6HourCloudBurstHighValue = value;

            }
        }
        #endregion

        #region 12Hour
        public static string Hour12Start
        {
            get
            {
                return str12HourStart;
            }
            set
            {

                str12HourStart = value;

            }
        }
        public static string Hour12End
        {
            get
            {
                return str12HourEnd;
            }
            set
            {

                str12HourEnd = value;

            }
        }
        public static string Hour12RainTotal
        {
            get
            {
                return str12HourRainTotal;
            }
            set
            {

                str12HourRainTotal = value;

            }
        }
        public static string Hour12AtlasLowFrequency
        {
            get
            {
                return str12HourAtlasLowFrequency;
            }
            set
            {

                str12HourAtlasLowFrequency = value;

            }
        }
        public static string Hour12CloudBurstLowFrequency
        {
            get
            {
                return str12HourCloudBurstLowFrequency;
            }
            set
            {

                str12HourCloudBurstLowFrequency = value;

            }
        }
        public static string Hour12AtlasLowValue
        {
            get
            {
                return str12HourAtlasLowValue;
            }
            set
            {

                str12HourAtlasLowValue = value;

            }
        }
        public static string Hour12CloudBurstLowValue
        {
            get
            {
                return str12HourCloudBurstLowValue;
            }
            set
            {

                str12HourCloudBurstLowValue = value;

            }
        }
        public static string Hour12AtlasHighFrequency
        {
            get
            {
                return str12HourAtlasHighFrequency;
            }
            set
            {

                str12HourAtlasHighFrequency = value;

            }
        }
        public static string Hour12CloudBurstHighFrequency
        {
            get
            {
                return str12HourCloudBurstHighFrequency;
            }
            set
            {

                str12HourCloudBurstHighFrequency = value;

            }
        }
        public static string Hour12AtlasHighValue
        {
            get
            {
                return str12HourAtlasHighValue;
            }
            set
            {

                str12HourAtlasHighValue = value;

            }
        }
        public static string Hour12CloudBurstHighValue
        {
            get
            {
                return str12HourCloudBurstHighValue;
            }
            set
            {

                str12HourCloudBurstHighValue = value;

            }
        }
        #endregion

        #region 24Hour
        public static string Hour24Start
        {
            get
            {
                return str24HourStart;
            }
            set
            {

                str24HourStart = value;

            }
        }
        public static string Hour24End
        {
            get
            {
                return str24HourEnd;
            }
            set
            {

                str24HourEnd = value;

            }
        }
        public static string Hour24RainTotal
        {
            get
            {
                return str24HourRainTotal;
            }
            set
            {

                str24HourRainTotal = value;

            }
        }
        public static string Hour24AtlasLowFrequency
        {
            get
            {
                return str24HourAtlasLowFrequency;
            }
            set
            {

                str24HourAtlasLowFrequency = value;

            }
        }
        public static string Hour24CloudBurstLowFrequency
        {
            get
            {
                return str24HourCloudBurstLowFrequency;
            }
            set
            {

                str24HourCloudBurstLowFrequency = value;

            }
        }
        public static string Hour24AtlasLowValue
        {
            get
            {
                return str24HourAtlasLowValue;
            }
            set
            {

                str24HourAtlasLowValue = value;

            }
        }
        public static string Hour24CloudBurstLowValue
        {
            get
            {
                return str24HourCloudBurstLowValue;
            }
            set
            {

                str24HourCloudBurstLowValue = value;

            }
        }
        public static string Hour24AtlasHighFrequency
        {
            get
            {
                return str24HourAtlasHighFrequency;
            }
            set
            {

                str24HourAtlasHighFrequency = value;

            }
        }
        public static string Hour24CloudBurstHighFrequency
        {
            get
            {
                return str24HourCloudBurstHighFrequency;
            }
            set
            {

                str24HourCloudBurstHighFrequency = value;

            }
        }
        public static string Hour24AtlasHighValue
        {
            get
            {
                return str24HourAtlasHighValue;
            }
            set
            {

                str24HourAtlasHighValue = value;

            }
        }
        public static string Hour24CloudBurstHighValue
        {
            get
            {
                return str24HourCloudBurstHighValue;
            }
            set
            {

                str24HourCloudBurstHighValue = value;

            }
        }
        #endregion

        #region 48Hour
        public static string Hour48Start
        {
            get
            {
                return str48HourStart;
            }
            set
            {

                str48HourStart = value;

            }
        }
        public static string Hour48End
        {
            get
            {
                return str48HourEnd;
            }
            set
            {

                str48HourEnd = value;

            }
        }
        public static string Hour48RainTotal
        {
            get
            {
                return str48HourRainTotal;
            }
            set
            {

                str48HourRainTotal = value;

            }
        }
        public static string Hour48AtlasLowFrequency
        {
            get
            {
                return str48HourAtlasLowFrequency;
            }
            set
            {

                str48HourAtlasLowFrequency = value;

            }
        }
        public static string Hour48CloudBurstLowFrequency
        {
            get
            {
                return str48HourCloudBurstLowFrequency;
            }
            set
            {

                str48HourCloudBurstLowFrequency = value;

            }
        }
        public static string Hour48AtlasLowValue
        {
            get
            {
                return str48HourAtlasLowValue;
            }
            set
            {

                str48HourAtlasLowValue = value;

            }
        }
        public static string Hour48CloudBurstLowValue
        {
            get
            {
                return str48HourCloudBurstLowValue;
            }
            set
            {

                str48HourCloudBurstLowValue = value;

            }
        }
        public static string Hour48AtlasHighFrequency
        {
            get
            {
                return str48HourAtlasHighFrequency;
            }
            set
            {

                str48HourAtlasHighFrequency = value;

            }
        }
        public static string Hour48CloudBurstHighFrequency
        {
            get
            {
                return str48HourCloudBurstHighFrequency;
            }
            set
            {

                str48HourCloudBurstHighFrequency = value;

            }
        }
        public static string Hour48AtlasHighValue
        {
            get
            {
                return str48HourAtlasHighValue;
            }
            set
            {

                str48HourAtlasHighValue = value;

            }
        }
        public static string Hour48CloudBurstHighValue
        {
            get
            {
                return str48HourCloudBurstHighValue;
            }
            set
            {

                str48HourCloudBurstHighValue = value;

            }
        }
        #endregion


    }
}
