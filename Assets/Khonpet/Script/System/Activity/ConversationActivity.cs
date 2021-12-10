using System.Collections;
using System.Collections.Generic;
using UnityEngine;









public class Conversation
{
    static PetObj petObj => PetObj.Current;
    static PetActivity.PetInspector inspector => PetData.PetInspector;
    static PlayingData.PetPlaying petPlaying => inspector.PetPlaying;




    public static void Welcome()
    {
        var message = $"";
        Talk(message);
    }

    public static void Like()
    {
        var message = $"";
        Talk(message);
    }
    public static void Petting(bool boring, bool airlike)
    {
        var message = $"";
        Talk(message);
    }

    public static void EatFood(Food.FoodType type , Feeling.FeelingType feeling, int currentpoint, bool full)
    {
        var message = $"";
        Talk(message);
    }
    public static void Play(Play.PlayType play, bool win)
    {
        var message = $"";
        Talk(message);
    }
    public static void Sleep(int star , int point, bool full)
    {
        var message = $"";
        Talk(message);
    }
    public static void Clean(int star , bool full )
    {
        var message = $"";
        Talk(message);
    }
    public static void JourneyTopScore(bool high, int score)
    {
        var message = $"{Language.Get("journey_newhighscore")} {score}";
        Talk(message);
    }



   









    static void Talk(string message)
    {
        petObj.talking.petTalk.ShowText(message);
    }



}

