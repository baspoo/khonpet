using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class PetActivity 
{

    public static bool IsReady => m_petObj != null;
    static PetObj m_petObj;
    public static void Init( )
    {
        ResourcesHandle.Load(PetData.Current.ID,"pet", ResourcesHandle.FileType.prefab, (obj) => {
            if (obj != null)
            {
                m_petObj = GameObject.Instantiate((GameObject)obj, World.instance.PetPosition).GetComponent<PetObj>();
                m_petObj.transform.localPosition = Vector3.zero;
                m_petObj.transform.localScale = Vector3.one;
                m_petObj.Init(PetData.Current);
            }
            else
            {
                Debug.LogError($"petobj = null!");
                InterfaceRoot.instance.Error("Not Instantiate!", Language.Get("not_instantiate_assets"));
            }
        });

    }

























    public  class PetInspector
    {
        PetObj petObj => PetObj.Current;
        PetData m_pet;
        PlayingData.PetPlaying m_petplaying;

        public PetInspector(PetData pet) 
        {
            this.m_pet = pet;
            this.m_petplaying = Playing.instance.FindPet(m_pet.ID,true);
        }



        public PetData PetData => m_pet;
        public PlayingData.PetPlaying PetPlaying => m_petplaying;
        public long Like => FirebaseService.instance.Preset.Like;
        public long Star => (Setting.instance.debug.isStarDebug) ? Setting.instance.debug.StarCountDebug : FirebaseService.instance.Preset.Star;
        public Utility.Level Lv => new Utility.Level(Star);
        public bool Liked => m_petplaying.Liked;
        public bool IsBoring => m_petplaying.UnixLastPetting.IsTimeout(Config.Data.Petting.BoringTime_Min);
        public bool IsLikeAir => PetData.Air == AirActivity.GetAirData().airName;
        public List<PlayingData.PetPlaying.QuestPlaying> Quests => m_petplaying.Quests;

        public bool IsNeedFood => this.GetStat(Pet.StatType.Hungry) < (Pet.Static.MaxStat * 0.9);
        public bool IsNeedEnergy => this.GetStat(Pet.StatType.Energy) < (Pet.Static.MaxStat * 0.2);
        public bool IsNeedClean => this.GetStat(Pet.StatType.Cleanliness) < (Pet.Static.MaxStat * 0.5);

        public Food.FoodType FoodFav => m_pet.Foods.First(x => x.Value == Feeling.FeelingType.Super).Key;
        public Food.FoodType FoodBad => m_pet.Foods.First(x => x.Value == Feeling.FeelingType.Bad).Key;
        public bool IsActing => ConsoleActivity.IsActing;

        public bool IsSleeping => this.GetActivity(Pet.Activity.StartSleep) != null;


        public Store.Relationship Relationship 
        {
            get {
                /*
                20
                40
                60
                80
                100
                 */
                var stat = this.GetStat(Pet.StatType.Relationship);
                foreach (var r in Store.instance.Relationships) 
                {
                    if (stat < r.Value) 
                    {
                        return r;
                    }
                }
                return Store.instance.Relationships[0];
            }
        }
  
        public string AirName => m_petplaying.Air.AirName;
        public bool AirTimeOut => m_petplaying.Air.UnixAir.IsTimeout(Config.Data.Air.SeasonChangeDuration_Min);


        public List<FirebaseService.Pet.TopScore.Score> JourneyScore => FirebaseService.instance.Preset.TopScore.journey;
        public PlayingData.PetPlaying.JourneyData Journey => m_petplaying.Journey;
    }


    public static void OnPetting(this PetInspector pet)
    {
        Conversation.Petting(pet.IsBoring, pet.IsLikeAir);
        if (pet.IsBoring) 
        { 
            pet.AddStar(1);
            pet.AddActivity(Pet.Activity.Petting, 1);
            pet.AddRelationship(pet.IsLikeAir? Random.RandomRange(1,3) : 1 );
            pet.PetPlaying.Petting();
            Chat.instance.Add(Chat.ChatCode.petting);
        }
    }


    public static void AddStar(this PetInspector pet , int star )
    {
        if (star == 0)
            return;

        FirebaseService.instance.Preset.ClientStar += star;
        FirebaseService.instance.AddValue( FirebaseService.ValueKey.star , star );
        pet.AddActivity(Pet.Activity.GiveStar, star);
        Playing.instance.AddStar(star);
        MainmenuPage.instance.starZone.OnAddStar(star);
        Chat.instance.Add(Chat.ChatCode.star);
        FirebaseService.instance.UpdateUserData();
    }

    public static void LvUp(this PetInspector pet)
    {
        PetObj.Current.OnUpdatePetObj();
        Chat.instance.Add(Chat.ChatCode.lvup);
    }


    public static void AddLike(this PetInspector pet)
    {
        FirebaseService.instance.Preset.ClientLike++;
        FirebaseService.instance.AddValue(FirebaseService.ValueKey.like, 1);
        pet.PetPlaying.Liked();
        pet.AddRelationship(10);
        Chat.instance.Add(Chat.ChatCode.like);
        Conversation.Like( );
    }

    public static void UpdateAir(this PetInspector pet , string airName)
    {
        pet.PetPlaying.UpdateAir(airName);
    }


    public static void AddRelationship(this PetInspector pet , int point = 1)
    {
        pet.AddStat( Pet.StatType.Relationship , point);

    }


    public static void OnFoodComplete(this PetInspector pet, Food.FoodType type)
    {
        var feeling = pet.PetData.Foods[type];
        var star = 0;
        var foodpoint = Config.Data.Eat.Hungry;
        var energy = Config.Data.Eat.Energy;

        switch (feeling)
        {
            case Feeling.FeelingType.Super:
                Chat.instance.Add(Chat.ChatCode.food);
                pet.AddActivity(Pet.Activity.EatHappyFood);
                star = 5;
                break;
            case Feeling.FeelingType.Happy:
                star = 3; 
                break;
            case Feeling.FeelingType.Normal:
                star = 2; 
                break;
            case Feeling.FeelingType.Bad:
                star = 0;
                break;
            default:
                break;
        }


        var current = pet.GetStat(Pet.StatType.Hungry);
        bool unfull = pet.OnGiveStarUnFullStat(Pet.StatType.Hungry, star);
        if (unfull)
        {
            pet.AddStat(Pet.StatType.Hungry, foodpoint);
            pet.AddStat(Pet.StatType.Energy, energy);
            if (feeling == Feeling.FeelingType.Super)
            {
                pet.AddRelationship(1);
            }
        }

        Conversation.EatFood(type, feeling, current , !unfull);
        pet.AddActivity(Pet.Activity.EatFood);
    }
    public static void OnPlayComplete(this PetInspector pet , Play.PlayType play , bool win )
    {
        var playData = Store.instance.FindPlay(play);
        pet.AddStar(win? playData.Star : 0);
        pet.AddStat(Pet.StatType.Cleanliness, playData.Cleanliness);
        pet.AddStat(Pet.StatType.Energy, playData.Energy);

        pet.AddActivity(Pet.Activity.Play);
        switch (play) {
            case Play.PlayType.Ball:pet.AddActivity(Pet.Activity.PlayBall); break;
            case Play.PlayType.Memory: pet.AddActivity(Pet.Activity.PlayMemory); break;
            case Play.PlayType.Guess: pet.AddActivity(Pet.Activity.PlayGuess); break;
            case Play.PlayType.Dance: pet.AddActivity(Pet.Activity.PlayDance); break;
        }
        Conversation.Play(play, win);
    }
    public static void OnCleanComplete(this PetInspector pet )
    {
        var star = (Pet.Static.MaxStat-pet.GetStat(Pet.StatType.Cleanliness)) / 20;
        bool unfull = pet.OnGiveStarUnFullStat(Pet.StatType.Cleanliness, star);
        if (unfull)
            pet.AddActivity(Pet.Activity.Clean, star);
        pet.AddStat(Pet.StatType.Cleanliness, 100);
        Conversation.Clean(star,!unfull);
    }
    public static void OnSleepStart(this PetInspector pet)
    {
        pet.SetActivity( Pet.Activity.StartSleep, pet.GetStat(Pet.StatType.Energy));
    }
    public static void OnSleepComplete(this PetInspector pet)
    {

        // (+100)
        // 30 (+100) = 130p 
        // 130p - 100st = 30des
        // (+100) -30des = 70!

        // (+71)
        // 30 (+71) = 101p
        // 101p - 100st = 1des
        // (+71) -1des = 70!

        // (+20)
        // 30 (+20) = 50p 
        // 50p - 100st = -50des ~0
        // (+20) -0des = 20!


        // (+19)
        // 30 (+19) = 49p 
        // 49p - 100st = -51des ~0
        // (+19) -0des = 19!

        // (+1)
        // 30 (+1) = 31p 
        // 31p - 100st = -69des ~0
        // (+1) -0des = 1!


        // (+0)
        // 30 (+0) = 30p 
        // 30p - 100st = -70des ~0
        // (+0) -0des = 0!

        var energy = pet.CalActivityDurationTime(Pet.Activity.StartSleep,1).Max(Pet.Static.MaxStat);
        var nowEnergy = pet.GetStat(Pet.StatType.Energy);
        var awsP = energy + nowEnergy;
        var des = (awsP - Pet.Static.MaxStat).Min(0);
        var realEnergy = energy - des;
        var star = realEnergy / 20;


        //Debug.Log($"energy : {energy}");
        //Debug.Log($"nowEnergy : {nowEnergy}");
        //Debug.Log($"awsP : {awsP}");
        //Debug.Log($"des : {des}");
        //Debug.Log($"realEnergy : {realEnergy}");
        //Debug.Log($"star : {star}");

        bool unfull = pet.OnGiveStarUnFullStat(Pet.StatType.Energy, star);
        if (unfull)
            pet.AddActivity(Pet.Activity.Sleep, star);
        pet.AddStat(Pet.StatType.Energy, realEnergy );
        pet.RemoveActivity(Pet.Activity.StartSleep);
        Conversation.Sleep(star, energy , !unfull);
    }

    public static void OnJourneyComplete(this PetInspector pet, int score)
    {

        if (score == 0)
            return;

        int star = 0;
        int max = Config.Data.Journey.PointOfRange;
        //old 18000
        //new 25000
        // 25000-18000 = 7000
        // 18000 % 5000 = 3000
        // 7000+3000 = 2 star
        var resume = score - pet.Journey.DailyScore;
        var mod = pet.Journey.DailyScore % max;
        resume += mod;
        if (resume > 0) 
        {
            star = (int)resume / max;
            star = star.Max(9);
        }
        FirebaseService.instance.TopScoreVerify(score);
        pet.AddStar(star);


        pet.AddActivity(Pet.Activity.Journey, 1);


        //New High Score..
        var high = pet.PetPlaying.UpdateJourney(score, star);
        Conversation.JourneyTopScore(high, score);
        if (high || star!=0) 
        {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.LikeLove);
        }
    }


}









public static class StatUtility 
{
    [System.Serializable]
    public class StatData
    {
        public Pet.StatType stat;
        public string description => Language.Get($"{stat}");
        public int reduceDuration => Config.Data.StatReduce.Find(x => x.Stat == $"{stat}" ).Reduce_Min;
    }
    public static void AddStat(this PetActivity.PetInspector inspector, Pet.StatType Stat, int Value)
    {
        var current = inspector.GetStat(Stat);
        inspector.PetPlaying.AddStat(Stat, (current + Value).Max(Pet.Static.MaxStat) , PetPlayingTools.Opt.Set);
    }
    public static void UpdateStat( this PetActivity.PetInspector inspector , Pet.StatType Stat , int Value ) 
    {
        inspector.PetPlaying.AddStat(Stat, Value, PetPlayingTools.Opt.Set);
    }
    public static int GetStat(this PetActivity.PetInspector inspector, Pet.StatType Stat)
    {
        var statPlaying = inspector.PetPlaying.FindStat(Stat);
        var statData = Store.instance.FindStat(Stat);
        if (statPlaying != null) return CalStat(statPlaying.Value, statPlaying.UnixLastUpdate, statData.reduceDuration );
        else return 0;
    }


    public static bool OnGiveStarUnFullStat(this PetActivity.PetInspector pet, Pet.StatType stat, int star)
    {
        if (!pet.IsStatFull(stat))
        {
            pet.AddStar(star);
            return true;
        }
        else
        {
            PetObj.Current.talking.bubble.OnEmo(Talking.Bubble.EmoType.Full, 2.0f);
            return false;
        }
    }
    public static bool IsStatFull(this PetActivity.PetInspector inspector, Pet.StatType Stat)
    {
        var statVal = inspector.GetStat(Stat);
        return (statVal > Pet.Static.MaxStat * 0.9);
    }
    public static bool IsHighDemandData(this PetActivity.PetInspector inspector, Pet.StatType Stat)
    {
        var statVal = inspector.GetStat(Stat);
        return (statVal <= Pet.Static.MaxStat * 0.3);
    }


    

    public static int CalStat(int currentValue ,long lastupdate, int reduce)
    {
        //currentValue = 100;
        //lastupdate = Utility.TimeServer.DateTimeToUnixTimeStamp(System.DateTime.Now.AddMinutes(-1500),true);


        
        var now = System.DateTime.Now;
        var last = Utility.TimeServer.TimeStampToDateTime(lastupdate);
        var timespan = now-last;
        var point = timespan.TotalMinutes / (float)reduce;
        currentValue -= (int)point;
        if (currentValue < 0) currentValue = 0;

        //Debug.Log($"timespan {timespan.TotalMinutes}");
        //Debug.Log($"point {point}");
        //Debug.Log($"currentValue {currentValue}");





        //int val = currentValue;
        //var now = Utility.TimeServer.DateTimeToUnixTimeStamp(System.DateTime.Now);
        //var remain = now - currentValue;
        //var datetime = Utility.TimeServer.TimeStampToDateTime(remain);
        //Debug.Log($"{datetime.main}");

        return currentValue;
    }
}











public static class ActivityUtility
{
    [System.Serializable]
    public class StatData
    {
        public Pet.StatType stat;
        [TextArea]
        public string description;
        public int reduceDuration;
    }
    public static void AddActivity(this PetActivity.PetInspector inspector, string ActName , int Value = 1)
    {
        MainmenuPage.instance.submenuZone.UpdateNotif();
        inspector.PetPlaying.UpdateActivity(ActName, Value , PetPlayingTools.Opt.Add);
    }
    public static void SetActivity(this PetActivity.PetInspector inspector, string ActName , int Value = 0)
    {
        inspector.PetPlaying.UpdateActivity(ActName, Value, PetPlayingTools.Opt.Set);
    }
    public static PlayingData.PetPlaying.Activity GetActivity(this PetActivity.PetInspector inspector, string Act)
    {
        return inspector.PetPlaying.FindActivity(Act);
    }
    public static int GetActivityValue(this PetActivity.PetInspector inspector, string Act)
    {
        int value = 0;
        var act = inspector.PetPlaying.FindActivity(Act);
        if (act != null)
            return act.Value;
        else
            return 0;
    }
    public static void RemoveActivity(this PetActivity.PetInspector inspector, string ActName)
    {
        inspector.PetPlaying.RemoveActivity(ActName);
    }
    public static int CalActivityDurationTime(this PetActivity.PetInspector inspector, string ActName , int min )
    {
        var act = inspector.GetActivity(ActName);
        var now = System.DateTime.Now;
        var last = Utility.TimeServer.TimeStampToDateTime(act.UnixLastActive);
        var timespan = now - last;
        var point = timespan.TotalMinutes / (float)min;
        return ((int)point);
    }
}






public static class Achievement
{
    public enum RewardType
    {
        Costume, Animation
    }
    [System.Serializable]
    public class AchievementData
    {
        public int Index;
        public int Level => PetData.Current.LvUnlocks[Index];
        public string Name;
        public Sprite Icon;
        public RewardType RewardType;
        public AnimationClip Clip;
        public Transform Costume;
        public bool IsUnlock(int lv) => lv >= Level;
    }
    public static void AdjuestCostume(this PetObj pet , bool? force = null , int? onlyIndex = null)
    {
        var lv = PetData.PetInspector.Lv.CurrentLevel;
        pet.achievements.ForEach(ahv => {
            if (ahv.RewardType == RewardType.Costume)
            {
                if(force == null) 
                {
                    bool isunlock = ahv.IsUnlock(lv);
                    if (isunlock) 
                    { 
                        ahv.Costume.gameObject.SetActive(PetData.PetInspector.IsEquip(ahv.Index));
                    }
                    else
                        ahv.Costume.gameObject.SetActive(false);
                }
                else
                    ahv.Costume.gameObject.SetActive((bool)force);
            }
        });
    }
    public static void AdjuestAnimation(this PetObj pet)
    {
        var lv = PetData.PetInspector.Lv.CurrentLevel;
        pet.achievements.ForEach(ahv => {
            if (ahv.RewardType == RewardType.Animation && ahv.IsUnlock(lv))
            {
                if (!pet.anim.SubIdle.Contains(ahv.Clip)) 
                {
                    pet.anim.SubIdle.Add(ahv.Clip);
                }
            }
        });
    }
    public static void OnEquip(this PetActivity.PetInspector inspector, int index , bool value)
    {
        inspector.PetPlaying.UpdateEquip(index, value);
        PetObj.Current.AdjuestCostume();
    }
    public static bool IsEquip(this PetActivity.PetInspector inspector, int index)
    {
        var isEquip = inspector.PetPlaying.FindEquip(index);
        if (isEquip != null)
        {
            return isEquip.IsEquipped;
        }
        else
            return true;
    }
}
