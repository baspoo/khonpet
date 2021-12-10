
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




public class Config
{

    [System.Serializable]
    public class ConfigData
    {
        public AirData Air;
        [System.Serializable]
        public class AirData
        {
            public long SeasonChangeDuration_Min;
        }
        public BalloonData Balloon;
        [System.Serializable]
        public class BalloonData
        {
            public bool Active;
            public float[] Duration_Sec;
        }
        public TimeData Time;
        [System.Serializable]
        public class TimeData
        {
            public int BoringTime_Min;
        }
        public EatData Eat;
        [System.Serializable]
        public class EatData
        {
            public int Hungry;
            public int Energy;
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


    public class LanguageData {
        public string key;
        public string[] messages;
        public string getmessage => messages[languageIndex];
        public List<string> tag;
    }

    public static List<LanguageData> Languages = new List<LanguageData>();
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
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable())
            {
                Languages.Add(new LanguageData()
                {
                    key = d.GetIndex(0),
                    tag = d.GetIndex(1).Split(',').ToList(),
                    messages = new string[] { d.GetIndex(2) , d.GetIndex(3) }
                }) ;
            }
            Done = true;
            done();
        }
    }

    public enum LanguageType { En=0,Th=1 }

    static int languageIndex => Playing.instance.playingData.Language;
    public static LanguageType languageType => (LanguageType)languageIndex;

    public static string Get(string key) 
    {
        var language = Languages.Find(x=>x.key == key);
        if (language!=null) 
        {
            return language.messages[languageIndex];
        }
        else return key;
    }
    public static List<LanguageData> GetTag( List<string> tag)
    {

        List<LanguageData> let = new List<LanguageData>();
        foreach (var find in Languages) {


            bool iscan = true;
            foreach (var t in tag)
            {
                if(!find.tag.Contains(t))
                    iscan = false;
            }
            if (iscan)
                let.Add(find);
        }
        return let;
    }




}