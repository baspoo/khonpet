
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
        public PettingData Petting;
        [System.Serializable]
        public class PettingData
        {
            public int BoringTime_Min;
            public float PetTalkDuration_Sec;
            public int SkinHighDemandPercent;
            public int[] SpamDuration_Sec;
            public int SpamCount;
        }
        public List<BehaviourData> Behaviours;
        [System.Serializable]
        public class BehaviourData
        {
            public int Rate;
            public int Percent;
            public int[] TalkStayTime_Sec;
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
        public string tag;
        public string relation;
        public int percent;
        public string exclusive;
    }

    public static List<LanguageData> Languages = new List<LanguageData>();
    static Dictionary<string, List<LanguageData>> Conversations = new Dictionary<string, List<LanguageData>>();
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
                var val = new LanguageData()
                {
                    key = d.GetIndex(0),
                    tag = d.GetIndex(1),
                    relation = d.GetIndex(2),
                    percent = d.GetIndex(3).ToInt(),
                    messages = new string[] { d.GetIndex(4), d.GetIndex(5) },
                    exclusive = d.GetIndex(6)
                };


                if (string.IsNullOrEmpty(val.tag))
                {
                    // Languages
                    Languages.Add(val);
                }
                else 
                {
                    //Conversations
                    if (!Conversations.ContainsKey(val.tag))
                    {
                        Conversations.Add(val.tag, new List<LanguageData>());
                    }
                    Conversations[val.tag].Add(val);
                }

            }

            Logger.Log($"Languages:{Languages.Count}    Conversations:{Conversations.Count}");


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
            return language.getmessage;
        }
        else return key;
    }
    public static List<string> GetTag( string tag , string key , int relation)
    {
        string str_relation = relation.ToString();
        List<string> let = new List<string>();
        if (Conversations.ContainsKey(tag))
        {
            var languages = Conversations[tag];
            foreach (var find in languages)
            {

                var msg = find.getmessage;


                if (find.key == key && 
                    !string.IsNullOrEmpty(msg) && msg != "-" &&
                    (string.IsNullOrEmpty(find.relation) || find.relation == "0" || find.relation.Contains(str_relation)) &&
                    (find.percent == 0 || find.percent.IsPercent()) &&
                    (string.IsNullOrEmpty(find.exclusive) || find.exclusive == "-" || find.exclusive == PetData.Current.ID))

                {

                    let.Add(msg);
                }

            }
            Logger.Log($"tag.count = {languages.Count} / filter = {let.Count}");
        }

        return let;
    }

    public static string Override(string message)
    {
        message = message.Replace("{foodfav}", Get(PetData.PetInspector.FoodFav.ToString()));
        message = message.Replace("{foodbad}", Get(PetData.PetInspector.FoodBad.ToString()));
        message = message.Replace("{name}", Get(PetData.Current.Name));
        message = message.Replace("{username}", Get(Playing.instance.playingData.NickName));
        return message;
    }


}