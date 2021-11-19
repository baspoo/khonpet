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


    public class Web 
    {
        public static void GotoUrl(string url) 
        {
            #if UNITY_EDITOR
                Application.OpenURL(url);
            #else
                HtmlCallback.GotoUrl(url);
            #endif
        }
    }


    public class Level 
    {
        double sqrt = 0.4;
        double volume = 6.28;
        public long XP;
        public int CurrentLevel => m_currentLevel +1;
        int m_currentLevel;
        public int xpNextlevel;
        public float Percent;
        public Level (long xp)
        {
            XP = xp;
            m_currentLevel = (int)(sqrt * Mathf.Sqrt(XP));
            //add sone cool text to show you reached a nev level
            xpNextlevel = (int)(volume * (m_currentLevel + 1) * (m_currentLevel + 1));
            var xpstartlevel = (int)(volume * (m_currentLevel) * (m_currentLevel));
            var remain_start = (XP - xpstartlevel);
            var remain_end = (xpNextlevel - xpstartlevel);
            Percent = (float)(remain_start) / (float)(remain_end);

        }
    }



    public class GameObj : MonoBehaviour
    {
        public static GameObject Created(GameObject _page, Transform _parent)
        {
            GameObject p = Instantiate(_page) as GameObject;
            p.transform.parent = _parent;
            p.transform.localPosition = Vector3.zero;
            p.transform.localScale = Vector3.one;
            return p;
        }
        public static List<GameObject> GetAllNode(Transform _tran)
        {
            List<GameObject> m_objs = new List<GameObject>();
            int count = _tran.childCount;
            for (int n = 0; n < count; n++)
            {

                m_objs.Add(_tran.GetChild(n).gameObject);
                m_objs.AddRange(GetAllNode(_tran.GetChild(n)));
            }
            return m_objs;
        }
        public static List<GameObject> GetAllParent(Transform _tran)
        {
            List<GameObject> m_objs = new List<GameObject>();
            int count = _tran.childCount;
            for (int n = 0; n < count; n++)
                m_objs.Add(_tran.GetChild(n).gameObject);
            return m_objs;
        }
    }


}
