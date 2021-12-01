using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{

    public static Store instance { get { if (m_instance == null) m_instance = FindObjectOfType<Store>(); return m_instance;  } }
    static Store m_instance;



    public PetAsset Pet;
    [System.Serializable]
    public class PetAsset 
    {
        public RuntimeAnimatorController animatorController;
    }


    //AIR
    public List<AirActivity.AirData> AirDatas;
    public AirActivity.AirData FindAirData(string name) => AirDatas.Find(x=>x.airName == name);
    public AirActivity.AirData FindAirData(AirActivity.AirType type) => AirDatas.Find(x => x.airType == type);



    //FOOD
    public List<Food> Foods;
    public Food FindFood(string name) => Foods.Find(x => x.Name == name);
    public Food FindFood(Food.FoodType type) => Foods.Find(x => x.Type == type);



    //FEELING
    public List<Feeling> Feelings;
    public Feeling FindFeeling(string name) => Feelings.Find(x => x.Name == name);
    public Feeling FindFeeling(Feeling.FeelingType type) => Feelings.Find(x => x.Type == type);


    //PLAY
    public List<PlayAsset> Plays;
    [System.Serializable]
    public class PlayAsset
    {
        public Play.PlayType Type;
        public GameObject Root;
        public int Lv=> playData.Lv;
        public int Energy => playData.Energy;
        public int Cleanliness => playData.Cleanliness;
        public int Star => playData.Star;
        public bool IsActive (int lv) => (lv >= this.Lv);
        Config.ConfigData.PlayData playData
        {
            get
            {
                if (m_playData == null)
                    m_playData = Config.Data.PlayDatas.Find(x => x.PlayType == Type.ToString());
                return m_playData;
            }
        }
        Config.ConfigData.PlayData m_playData;
    }
    public PlayAsset FindPlay(Play.PlayType type) => Plays.Find(x => x.Type == type);


    //PLAY
    public List<Relationship> Relationships;
    [System.Serializable]
    public class Relationship
    {
        public Pet.Relationship Type;
        public string Name => Language.Get($"{Type}");
        public Sprite Icon;
        public int Value => Config.Data.Relationships.Find(x=>x.Type == Name).Value;
    }
    public Relationship FindRelationship(Pet.Relationship type) => Relationships.Find(x => x.Type == type);

    //STAT
    public List<StatUtility.StatData> Stats;
    public StatUtility.StatData FindStat(Pet.StatType stat) => Stats.Find(x => x.stat == stat);


    //QUEST
    public List<QuestActivity.QuestData> Quests;
    public QuestActivity.QuestData FindQuest(string QuestID) => Quests.Find(x => x.QuestID == QuestID);
}
