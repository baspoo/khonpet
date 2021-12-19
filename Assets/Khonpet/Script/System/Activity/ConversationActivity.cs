using System.Collections;
using System.Collections.Generic;
using UnityEngine;









public class Conversation
{
    static PetObj petObj => PetObj.Current;
    static PetActivity.PetInspector inspector => PetData.PetInspector;
    static PlayingData.PetPlaying petPlaying => inspector.PetPlaying;



    static int hour = 60;
    static int day = hour*24;


    public static void Welcome()
    {
        var message = "";
        var act = inspector.GetActivity(Pet.Activity.Welcome);
        if (act == null || act.Value == 0)
        {
            message = "welcome_firsttime";
        }
        else 
        {
            var time =  act.UnixLastActive.ToDateTime();
            if (time.IsTimeout(day * 7))
            {
                message = "welcome_verylongtime";
            }
            else if (time.IsTimeout(day * 2))
            {
                message = "welcome_longtime";
            }
            else if (time.IsTimeout(hour*16))
            {
                message = "welcome_day";
            }
            else if (time.IsTimeout(30))
            {
                message = "welcome_30min";
            }
        }

       
        inspector.AddActivity(Pet.Activity.Welcome);
        Talk("welcome", message);
    }

    public static void Like()
    {
        var message = $"like_me";
        Talk("like", message);
    }


    public static void PetBehaviour( int relation ) 
    {
       
        List<string> msg = new List<string>();
        if (msg.Count == 0 && inspector.IsHighDemandData(Pet.StatType.Hungry))
        {
            msg.AddRange(GetMessageList("petting", "petting_needfood", relation));
        }
        if (msg.Count == 0 && inspector.IsHighDemandData(Pet.StatType.Energy))
        {
            msg.AddRange(GetMessageList("petting", "petting_needsleep", relation));
        }
        if (msg.Count == 0 && inspector.IsHighDemandData(Pet.StatType.Cleanliness))
        {
            msg.AddRange(GetMessageList("petting", "petting_needclean", relation));
        }
        if (msg.Count == 0)
            msg.AddRange(GetMessageList("behaviour", "behaviour_talking", relation));


        if (msg.Count > 0)
        {
            PushMessage(msg[Random.RandomRange(0, msg.Count)]);
        }
    }


    static PettingStatic pettingStatic;
    public class PettingStatic {
        public int count;
        public int rushcount;
        public System.DateTime lastpetting;
        public PlayingData.PetPlaying.Activity act;
    }
    public static void Petting(bool boring, bool likeair)
    {
        if (pettingStatic == null)
        {
            pettingStatic = new PettingStatic();
            var act = inspector.GetActivity(Pet.Activity.Petting);

            if (act != null)
            {
                pettingStatic.act = act;
                pettingStatic.lastpetting = pettingStatic.act.UnixLastActive.ToDateTime();
            }
            else 
            {
                pettingStatic.lastpetting = System.DateTime.Now;
            }
        }





        if (boring)
        {
            var message = $"petting_boring";
            Talk("petting", message);
        }
        else 
        {
            List<string> msg = new List<string>();
            pettingStatic.rushcount++;
            var relation = PetData.PetInspector.Relationship.Relation;

            if (pettingStatic.lastpetting.IsTimeoutSec(3))
            {

                Debug.Log($"Hungry {inspector.IsHighDemandData(Pet.StatType.Hungry)}  {msg.Count}");

                if (msg.Count == 0 && inspector.IsHighDemandData(Pet.StatType.Hungry)) 
                {
                    msg.AddRange(GetMessageList("petting", "petting_needfood", relation));
                }

                Debug.Log($"Energy {inspector.IsHighDemandData(Pet.StatType.Energy)}  {msg.Count}");

                if (msg.Count == 0 && inspector.IsHighDemandData(Pet.StatType.Energy))
                {
                    msg.AddRange(GetMessageList("petting", "petting_needsleep", relation));
                }
                Debug.Log($"Cleanliness {inspector.IsHighDemandData(Pet.StatType.Cleanliness)}  {msg.Count}");

                if (msg.Count == 0 && inspector.IsHighDemandData(Pet.StatType.Cleanliness))
                {
                    msg.AddRange(GetMessageList("petting", "petting_needclean", relation));
                }

                msg.AddRange(GetMessageList("petting", "petting_normal", relation));
                if (likeair)
                    msg.AddRange(GetMessageList("petting", "petting_likeair", relation));


            }
            else 
            {
                if (pettingStatic.rushcount == 5) 
                {
                    msg.AddRange(GetMessageList("petting", "petting_spam", relation));
                }
            }










            if (msg.Count > 0)
            {
                pettingStatic.rushcount = 0;
                PushMessage(msg[Random.RandomRange(0, msg.Count)]);
            }
        }




        pettingStatic.lastpetting = System.DateTime.Now;
    }

    public static void EatFood(Food.FoodType type , Feeling.FeelingType feeling, int currentpoint, bool overenough)
    {
        var message = $"";
        if (overenough)
        {
            message = "eat_full";
        }
        else
        {
            if (feeling == Feeling.FeelingType.Bad) 
            {
                message = "eat_bad";
            }
            if (feeling == Feeling.FeelingType.Super)
            {
                message = "eat_super";
            }
            else 
            {
                if (currentpoint < 50)
                {
                    message = "eat_again";
                }
                else
                {
                    message = "eat_eatting";
                }
            }
        }
        Talk("eat", message);
    }
    public static void Play(Play.PlayType play, bool win)
    {
        var message = $"";
        if (win)
        {
            message = "play_win";
        }
        else
        {
            message = "play_lose";
        }
        Talk("play", message);
    }
    public static void Sleep(int star , int min, bool overenough)
    {
        var message = "";
        if (overenough)
        {
            message = "sleep_full";
        }
        else
        {
           if(min<1)
                message = "sleep_fastawake";
           else
                message = "sleep_awake";
        }
        Talk("sleep", message);
    }
    public static void Clean(int star , bool overenough)
    {
        var message = "";
        if (overenough)
        {
            message = "clean_full";
        }
        else
        {
            message = "clean_cleanup";
        }
        Talk("clean", message);
    }
    public static void JourneyTopScore(bool high, int score)
    {
        var message = $"{Language.Get("journey_newhighscore")} {score}";
        PushMessage(message);
    }





























    static List<string> GetMessageList(string tag,string key , int relation = -1)
    {
        if(relation==-1) relation = PetData.PetInspector.Relationship.Relation;
        var message = Language.GetTag(tag, key , relation);
        if (message != null && message.Count > 0)
            return message;
        else
            return new List<string>();
    }


    static void Talk(string tag ,string key)
    {
        if (key.notnull()) 
        {
            var message = GetMessageList(tag, key);
            if (message.Count > 0) 
            {
                var msg = message[Random.RandomRange(0, message.Count)];
                PushMessage(msg);
            }
        }
    }
    static void PushMessage(string message)
    {
        if (message.notnull())
            petObj.talking.petTalk.ShowText(Language.Override(message));
    }


}

