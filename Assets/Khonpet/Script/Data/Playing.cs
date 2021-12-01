using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pet 
{
    public class Static 
    {
        public const int MaxStat = 100;
        public const int MaxBalloon = 10;
    }
    public enum StatType
    {
        Relationship, Hungry, Energy, Cleanliness
    }
    public enum Relationship
    {
        Annoying,
        Amiable,
        Friend,
        SoulMate,
        Love
    }
    public class Activity
    {
        public const string EatFood = "EatFood";
        public const string EatHappyFood = "EatHappyFood";

        public const string Play = "Play";
        public const string PlayBall = "PlayBall";
        public const string PlayMemory = "PlayMemory";
        public const string PlayGuess = "PlayGuess";
        public const string PlayDance = "PlayDance";

        public const string Clean = "Clean";

        public const string Sleep = "Sleep";
        public const string StartSleep = "StartSleep";


        public const string GiveStar = "GiveStar";
        public const string Petting = "Petting";
        public const string StayWithMe = "StayWithMe";
        public const string Journey = "Journey";
        public const string Balloon = "Balloon";


    }
}


[System.Serializable]
public class PlayingData
{
    public string NickName;
    public string UserID;
    public long UnixCreatedAt;
    public long UnixLastUpdate;
    public int Language;
    public int StarPoint;
    public int BalloonPoint;

    //Setting
    public bool IsBgm = true;
    public bool IsSfx = true;

    public List<PetPlaying> PetPlayings = new List<PetPlaying>();
    [System.Serializable]
    public class PetPlaying
    {
        public string PetID;
        public long UnixCreatedAt;
        public long UnixLastUpdate;
        public bool Liked;

        public long UnixLastPetting;

        public List<Stat> Stats = new List<Stat>();
        [System.Serializable]
        public class Stat
        {
            public string StatName;
            public int Value;
            public long UnixLastUpdate;
        }

        public List<Equip> Equips = new List<Equip>();
        [System.Serializable]
        public class Equip
        {
            public int Index;
            public bool IsEquipped;
        }

        public List<Activity> Activities = new List<Activity>();
        [System.Serializable]
        public class Activity
        {
            public string ActName;
            public int Value;
            public long UnixLastActive;
        }


        public long UnixQuest;
        public List<QuestPlaying> Quests = new List<QuestPlaying>();
        [System.Serializable]
        public class QuestPlaying
        {
            public string QuestID;
            public long BeginValue;
            public long EndValue;
            public long Value;
            public long UnixCreatedAt;
            public bool IsClaimed;
        }
    }
}











//================================================================
//   PET
//================================================================
public static class PetPlayingTools 
{
    public enum Opt{ Add,Set }
    public static void Liked(this PlayingData.PetPlaying pet) 
    {
        pet.Liked = true;
        Playing.instance.Save();
    }
    public static void Petting(this PlayingData.PetPlaying pet)
    {
        pet.UnixLastPetting = Playing.instance.playingUnix;
        Playing.instance.Save();
    }
    //*****************************************************************
    // Stat
    //*****************************************************************
    public static void AddStat(this PlayingData.PetPlaying pet, Pet.StatType Stat , int Value , Opt Opt )
    {
        Debug.Log($"AddStat {Opt} {Stat} {Value}");
        var stat = pet.FindStat(Stat);
        if (stat == null)
        {
            stat = new PlayingData.PetPlaying.Stat();
            stat.StatName = $"{Stat}";
            stat.Value = Value;
            pet.Stats.Add(stat);
        }
        else
        {
            if (Opt == Opt.Add) stat.Value += Value;
            if (Opt == Opt.Set) stat.Value = Value;
        }

        stat.Value = stat.Value.Max(Pet.Static.MaxStat);
        stat.Value = stat.Value.Min(0);
        stat.UnixLastUpdate = Playing.instance.playingUnix;
        Playing.instance.Save();
    }
    public static PlayingData.PetPlaying.Stat FindStat(this PlayingData.PetPlaying pet, Pet.StatType Stat )
    {
        var StatName = Stat.ToString();
        return pet.Stats.Find(x => x.StatName == StatName);
    }
    //*****************************************************************
    // Equip
    //*****************************************************************
    public static void UpdateEquip(this PlayingData.PetPlaying pet, int index, bool isEquip)
    {
        var equip = pet.FindEquip(index);
        if (equip == null)
        {
            equip = new PlayingData.PetPlaying.Equip();
            equip.Index = index;
            equip.IsEquipped = isEquip;
            pet.Equips.Add(equip);
        }
        else
        {
            equip.IsEquipped = isEquip;
        }
        Playing.instance.Save();
    }
    public static PlayingData.PetPlaying.Equip FindEquip(this PlayingData.PetPlaying pet, int index)
    {
        return pet.Equips.Find(x => x.Index == index);
    }
    //*****************************************************************
    // Activity
    //*****************************************************************
    public static PlayingData.PetPlaying.Activity UpdateActivity(this PlayingData.PetPlaying pet, string ActName, int Value = 0, Opt Opt = Opt.Set)
    {
        var act = pet.Activities.Find(x=>x.ActName == ActName);
        if (act == null)
        {
            act = new PlayingData.PetPlaying.Activity();
            act.ActName = ActName;
            act.Value = Value;
            act.UnixLastActive = Playing.instance.playingUnix;
            pet.Activities.Add(act);
        }
        else 
        {
            if (Opt == Opt.Add) act.Value += Value;
            if (Opt == Opt.Set) act.Value = Value;
            act.UnixLastActive = Playing.instance.playingUnix;
        }
        Playing.instance.Save();
        return act;
    }
    public static void RemoveActivity(this PlayingData.PetPlaying pet, string ActName)
    {
        pet.Activities.RemoveAll(x => x.ActName == ActName);
        Playing.instance.Save();
    }
    public static PlayingData.PetPlaying.Activity FindActivity(this PlayingData.PetPlaying pet, string ActName)
    {
        return pet.Activities.Find(x => x.ActName == ActName);
    }
    //*****************************************************************
    // Quest
    //*****************************************************************
    public static void AddQuest(this PlayingData.PetPlaying pet, List<PlayingData.PetPlaying.QuestPlaying> quests)
    {
        pet.Quests = new List<PlayingData.PetPlaying.QuestPlaying>();
        pet.Quests = quests;
        pet.UnixQuest = Playing.instance.playingUnix;
        Playing.instance.Save();
    }
    public static bool OnClaim(this PlayingData.PetPlaying pet, PlayingData.PetPlaying.QuestPlaying quest )
    {
        if (!quest.IsClaimed)
        {
            quest.IsClaimed = true;
            Playing.instance.Save();
            return true;
        }
        else
            return false;
    }
    public static PlayingData.PetPlaying.QuestPlaying FindQuest(this PlayingData.PetPlaying pet, string QuestID)
    {
        return pet.Quests.Find(x => x.QuestID == QuestID);
    }
}











//================================================================
//   PLAYING
//================================================================
public class Playing : MonoBehaviour
{
    public static Playing instance { get { if (m_instance == null) m_instance = FindObjectOfType<Playing>(); return m_instance; } }
    static Playing m_instance;
    public  PlayingData playingData => m_playing;
    public long playingUnix => Utility.TimeServer.DateTimeToUnixTimeStamp(System.DateTime.Now,true);

    [SerializeField]
    bool ismodify = false;
    [SerializeField]
    PlayingData m_playing;
    PlayingData.PetPlaying m_pet;
    const string m_key = "playing";


    public void Init()
    {
        // load playing
        Load();
    }

    //*****************************************************************
    /// Save
    //*****************************************************************
    public string RawJson => Newtonsoft.Json.JsonConvert.SerializeObject(m_playing);
    public void Save(bool isforce = false)
    {
        if (isforce)
        {
            ismodify = false;

            var unix = playingUnix;
            m_playing.UnixLastUpdate = unix;
            if (m_playing.UnixCreatedAt == 0)
                m_playing.UnixCreatedAt = unix;

            if (m_pet != null) 
            {
                m_pet.UnixLastUpdate = unix;
                if (m_pet.UnixCreatedAt == 0)
                    m_pet.UnixCreatedAt = unix;
            }


            if (!m_playing.UserID.notnull())
                m_playing.UserID = $"{SystemInfo.deviceName}-{SystemInfo.deviceUniqueIdentifier}-{m_playing.UnixCreatedAt}-{Random.RandomRange(11111, 99999)}";


            var json = RawJson;
            PlayerPrefs.SetString(m_key, json);
        }
        else 
        {
            ismodify = true;
        }
    }
    float m_timesave = 1.0f;
    private void Update()
    {
        if (m_playing == null)
            return;

        if (m_timesave > 0)
        {
            m_timesave -= Time.deltaTime;
        }
        else 
        {
            m_timesave = 1.0f;
            if(ismodify)
                Save(true);
        }
    }

    //*****************************************************************
    /// Load
    //*****************************************************************
    public void Load()
    {
        m_playing = GetLocalStore();
    }

    //*****************************************************************
    /// Get LocalStore
    //*****************************************************************
    public PlayingData GetLocalStore()
    {
        var json = PlayerPrefs.GetString(m_key);
        if (!string.IsNullOrEmpty(json)) return Newtonsoft.Json.JsonConvert.DeserializeObject<PlayingData>(json);
        else return new PlayingData();
    }

    //*****************************************************************
    /// Clear
    //*****************************************************************
    public void Clear()
    {
        m_playing = new PlayingData();
        PlayerPrefs.DeleteKey(m_key);
    }

    //*****************************************************************
    /// Update Display Name
    //*****************************************************************
    public void UpdateDisplayName(string displayName)
    {
        m_playing.NickName = displayName;
        Save();
    }
    public void AddStar(int star)
    {
        m_playing.StarPoint += star;
        Save();
    }
    public void AddBalloon(int add)
    {
        m_playing.BalloonPoint += add;
        Save();
    }
    public void ResetBalloon( )
    {
        m_playing.BalloonPoint = 0;
        Save();
    }
    public void Sound(bool? IsBgm, bool? IsSfx)
    {
        if (IsBgm != null) m_playing.IsBgm = (bool)IsBgm;
        if (IsSfx != null) m_playing.IsSfx = (bool)IsSfx;
        Save();
    }


    //*****************************************************************
    /// Find Pet
    //*****************************************************************
    public PlayingData.PetPlaying FindPet(string petID , bool current = false)
    {
        var pet = m_playing.PetPlayings.Find(x => x.PetID == petID);
        if (pet == null)
        {
            // new pet
            pet = new PlayingData.PetPlaying() { PetID = petID };
            m_playing.PetPlayings.Add(pet);
        }
        if (current)
            m_pet = pet;
        return pet;
    }

}

























#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(Playing))]
[System.Serializable]
public class PlayingUI : Editor
{


    Playing plaging => (Playing)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(30);
        if (GUILayout.Button("Load"))
        {
            plaging.Load();
        }
        if (GUILayout.Button("Save"))
        {
            var local = plaging.GetLocalStore();
            if (local.UnixLastUpdate == plaging.playingData.UnixLastUpdate)
                plaging.Save(true);
            else
                Debug.LogError("Can't Save This Playing Data Is <color=red>Not Update.</color>");
        }
        if (GUILayout.Button("Clear"))
        {
            plaging.Clear();
        }
    }
}
#endif