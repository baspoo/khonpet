using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Config
{

    [System.Serializable]
    public class ConfigData
    {
        public TimeData Time;
        [System.Serializable]
        public class TimeData
        {
            public int BoringTime_Min;
            public float[] BalloonTime_Sec;
        }
        public List<StatReduceData> StatReduce;
        [System.Serializable]
        public class StatReduceData
        {
            public string Stat;
            public int Reduce_Min;
        }
        public List<RelationshipData> Relationships;
        [System.Serializable]
        public class RelationshipData
        {
            public string Type;
            public int Value;
        }
        public List<PlayData> PlayDatas;
        [System.Serializable]
        public class PlayData
        {
            public string PlayType;
            public int Lv;
            public int Energy;
            public int Cleanliness;
            public int Star;
        }
        public List<Quest> Quests;
        [System.Serializable]
        public class Quest
        {
            public string QuestID;
            public int Count;
            public int QuestStarReward;
        }
        public JourneyData Journey;
        [System.Serializable]
        public class JourneyData
        {
            public int PointOfRange;
            public int MaxCombo = 10;
            public int StartLive=5; 
            public int BaseScore=100;
            public int LowScore=50;
            public float SpeedStart = 1.0f;
            public float SpeedPlus = 0.1f;
        }
    }




    public static ConfigData Data;
    public static bool Done = false;
    public static void Init(System.Action done)
    {
        if (Done)
        {
            done();
            return;
        }
        LoaderService.instance.OnGetDatabase("config.json", (data) =>
        {
            Data = data.DeserializeObject<ConfigData>();
            Done = true;
            done();
        });
    }
}






public class Language
{

    public static bool Done = false;
    public static Dictionary<string, string[]> Languages = new Dictionary<string, string[]>();
    public static void Init(System.Action done)
    {
        if (Done) 
        {
            done();
            return;
        }
        LoaderService.instance.OnGetDatabase("language.tsv", (data) =>
        {
            setup(data);
        });


        void setup(string data) 
        {
            Debug.Log("data: " + data);
            var table = GameDataTable.ReadData(data);
            Debug.Log("table: " + table.GetTable().Count);
            foreach (var d in table.GetTable())
            {
                Debug.Log("d: " + d.DataLists.Count);
                Languages.Add(d.GetIndex(0), new string[2] { d.GetIndex(1), d.GetIndex(2) });
            }
            Debug.Log("Languages: " + Languages.Count);
            Done = true;
            done();
        }
    }

    public enum LanguageType { En=0,Th=1 }

    static int languageIndex => Playing.instance.playingData.Language;
    public static LanguageType languageType => (LanguageType)languageIndex;

    public static string Get(string key) 
    {
        if (Languages.ContainsKey(key)) 
        {
            return Languages[key][languageIndex];
        }
        else return key;
    } 

}