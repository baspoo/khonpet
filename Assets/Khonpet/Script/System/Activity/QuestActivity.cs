using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class QuestActivity
{
    public enum QuestType
    {
        //Main 
        EatFood, EatHappyFood , 
        PlayBall , PlayMemory, PlayGuess, PlayDance ,
        Clean,
        Sleeping,
        Journey, // <--- not yet


        //Other 
        Petting,
        GiveStar, 
        Balloon  // <--- not yet
    }
    public enum Group
    {
        None, Eat, Play
    }
    [System.Serializable]
    public class QuestData
    {
        public string QuestID;
        public string Description => Language.Get(QuestID);
        public QuestType QuestType;
        public Group Group;
        public Sprite Icon;
        public int QuestLevel;
        public int Count;
        public int QuestStarReward;
        public bool IsUnlock(int lv) => lv >= QuestLevel;
    }

    static List<QuestData> GetQuest(int count = 3)
    {
        List<QuestData> output = new List<QuestData>();
        List<QuestData> quests = GetCanDoingQuest();
        if (count >= quests.Count)
        {
            output = quests;
        }
        else 
        {
            while (output.Count < count)
            {
                var q = quests[quests.Count.Random()];
                if (!output.Contains(q))
                {
                    if (q.Group == Group.None || output.Find(x => x.Group == q.Group) == null) 
                    {
                        output.Add(q);
                        quests.Remove(q);
                    }
                }
            }
        }
        return output;
    }
    static List<QuestData> GetCanDoingQuest()  {
        List<QuestData> quests = new List<QuestData>();
        var level = PetData.PetInspector.Lv;
        foreach (var quest in Store.instance.Quests) 
        {
            if (quest.IsUnlock(level.CurrentLevel)) 
            {
                switch (quest.QuestType)
                {
                    case QuestType.EatFood:
                    case QuestType.EatHappyFood:
                    case QuestType.Clean:
                    case QuestType.Sleeping:
                    case QuestType.PlayBall:
                    case QuestType.Petting:
                    case QuestType.GiveStar:
                        quests.Add(quest);
                        break;
                    case QuestType.PlayMemory:
                        if (Store.instance.FindPlay(Play.PlayType.Memory).IsActive(level.CurrentLevel))
                            quests.Add(quest);
                        break;
                    case QuestType.PlayGuess:
                        if (Store.instance.FindPlay(Play.PlayType.Guess).IsActive(level.CurrentLevel))
                            quests.Add(quest);
                        break;
                    case QuestType.PlayDance:
                        if (Store.instance.FindPlay(Play.PlayType.Dance).IsActive(level.CurrentLevel))
                            quests.Add(quest);
                        break;
                    case QuestType.Journey:
                        break;
                    case QuestType.Balloon:
                        break;
                    default:
                        break;
                }
            }
        }
        return quests;
    }
    static string GetActivityKey(QuestType quest) {
        switch (quest)
        {
            case QuestType.EatFood: 
                return Pet.Activity.EatFood;
            case QuestType.EatHappyFood:
                return Pet.Activity.EatHappyFood;
            case QuestType.Clean:
                return Pet.Activity.Clean;
            case QuestType.Sleeping:
                return Pet.Activity.Sleep;
            case QuestType.Petting:
                return Pet.Activity.Petting;
            case QuestType.GiveStar:
                return Pet.Activity.GiveStar;
            case QuestType.PlayBall:
                return Pet.Activity.PlayBall;
            case QuestType.PlayMemory:
                return Pet.Activity.PlayMemory;
            case QuestType.PlayGuess:
                return Pet.Activity.PlayGuess;
            case QuestType.PlayDance:
                return Pet.Activity.PlayDance;
            case QuestType.Journey:
                return Pet.Activity.Journey;
            case QuestType.Balloon:
                return Pet.Activity.Balloon;
            default:
                break;
        }
        return null;
    }
    static PlayingData.PetPlaying.QuestPlaying ConvertToQuestPlaying(QuestData quest) 
    {
        PlayingData.PetPlaying.QuestPlaying questPlaying = new PlayingData.PetPlaying.QuestPlaying();
        questPlaying.QuestID = quest.QuestID;
        questPlaying.UnixCreatedAt = Playing.instance.playingUnix;
        questPlaying.BeginValue = PetData.PetInspector.GetActivityValue(GetActivityKey(quest.QuestType));
        questPlaying.EndValue = quest.Count;
        return questPlaying;
    }
    static void RefreshQuest(this PetActivity.PetInspector pet)
    {
        var quests = new List<PlayingData.PetPlaying.QuestPlaying>();
        foreach (var q in GetQuest())
        {
            quests.Add(ConvertToQuestPlaying(q));
        }
        pet.PetPlaying.AddQuest(quests);
    }






    public static void OnClaim(this PlayingData.PetPlaying.QuestPlaying quest )
    {
        if (PetData.PetInspector.PetPlaying.OnClaim(quest)) 
        {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.LikeLove);
            PetData.PetInspector.AddStar(quest.GetQuestData().QuestStarReward);
        }
    }
    public static bool IsHaveQuestCanClaim(this PetActivity.PetInspector pet)
    {
        foreach (var q in pet.Quests) 
        {
            if (q.IsDone() && !q.IsClaimed)
                return true;
        }
        return false;
    }
    public static QuestActivity.QuestData GetQuestData(this PlayingData.PetPlaying.QuestPlaying quest)
    {
        return Store.instance.FindQuest(quest.QuestID);
    }
    public static int[] GetProgress(this PlayingData.PetPlaying.QuestPlaying quest)
    {
        int[] output = new int[2];

        var questData = Store.instance.FindQuest(quest.QuestID);
        var currentValue = PetData.PetInspector.GetActivityValue(GetActivityKey(questData.QuestType));
        output[0] = currentValue - (int)quest.BeginValue;
        output[1] = (int)quest.EndValue;
        return output;
    }
    public static bool IsDone(this PlayingData.PetPlaying.QuestPlaying quest)
    {
        int[] output = quest.GetProgress();
        return output[0] >= output[1];
    }
    public static float GetPercent(this PlayingData.PetPlaying.QuestPlaying quest)
    {
        int[] output = quest.GetProgress();
        return (float)output[0].Max(output[1]) /(float)output[1];
    }

    public static void InitQuest(this PetActivity.PetInspector pet) {
        var now = System.DateTime.Now;
        var last = pet.PetPlaying.UnixQuest.ToDateTime();

        if (pet.PetPlaying.Quests.Count == 0) 
        {
            pet.RefreshQuest();
        }
        if (now.Day != last.Day) 
        {
            pet.RefreshQuest();
        }
    }







}
