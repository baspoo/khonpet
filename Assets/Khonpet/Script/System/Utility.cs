using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility 
{



    public class TimeServer
    {
        public static TimeServer master = new TimeServer();

        long dif;
        public void Init(System.DateTime timebegin)
        {
            //ex1. 50100 - 50000 = 100
            //ex2. 50100 - 50200 = -100
            dif = (DateTimeToUnixTimeStamp(timebegin) - DateTimeToUnixTimeStamp(System.DateTime.Now));
        }
        public static System.DateTime DateTime1970
        {
            get
            {
                return new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            }
        }
        public static long DateTimeToUnixTimeStamp(System.DateTime datetime, bool isLocalTime = false)
        {
            if (isLocalTime) return (long)(datetime.Subtract(DateTime1970.ToLocalTime())).TotalSeconds;
            else return (long)(datetime.Subtract(DateTime1970)).TotalSeconds;
        }
        public System.DateTime Time
        {
            get
            {
                //ex1
                // server - client
                // 50100 - 50000 = 100
                // Dif = 100
                // 100 + 50000 = 50100

                //ex2
                // server - client
                // 50100 - 50200 = -100
                // Dif = -100
                // -100 + 50200 = 50100

                //---- Time Run --------------------
                //Now 50000----->51200

                //ex1
                // Dif = 100
                // 100 + 51200 = 51300
                long t_now = DateTimeToUnixTimeStamp(System.DateTime.Now) + dif;
                System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                return dtDateTime.AddSeconds(t_now);
            }
        }
        public long Unix => DateTimeToUnixTimeStamp(Time, true) * 1000;

        public static System.DateTime TimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if(unixTimeStamp>9999999999)
                    dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            else
                dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }







}
