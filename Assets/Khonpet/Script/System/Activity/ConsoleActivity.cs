using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleActivity 
{
    public enum Activity 
    {
        Food,Play,Clean,Sleep
    }
    public static bool IsActing = false;
    public static void Init() {
        IsActing = false;
        FoodPage.instance.Init();
        Clean.instance.Init();
        Sleep.instance.Init();
    }





    public static void OnPetting( )
    {
        if (!IsActing)
        {
            PetObj.Current.talking.bubble.OnEmo( Talking.Bubble.EmoType.Love,2.0f );
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Petting);
            PetData.PetInspector.OnPetting();
        }
    }




    public static void OnBegin(Activity activity) 
    {
        if (activity == Activity.Food) OnFood();
        if (activity == Activity.Play) OnPlay();
        if (activity == Activity.Clean) OnClean();
        if (activity == Activity.Sleep) OnSleep();
    }
    public static void OnEnding( )
    {
        MainmenuPage.instance.main.OnActive(true);
        IsActing = false;
    }




    static void OnFood( )
    {
        IsActing = true;
        MainmenuPage.instance.main.OnActive(false);
        MainmenuPage.instance.consoleZone.btnFood.StartCooldown();
        FoodPage.instance.OnPlay((food) => {
            PetData.PetInspector.OnFoodComplete(food);
            OnEnding();
        });
    }
    static void OnClean()
    {
        IsActing = true;
        MainmenuPage.instance.main.OnActive(false);
        MainmenuPage.instance.consoleZone.btnClean.StartCooldown();
        Clean.instance.OnPlay(() => {
            PetData.PetInspector.OnCleanComplete();
            OnEnding();
        });
    }

    static void OnSleep()
    {
        IsActing = true;
        MainmenuPage.instance.main.OnActive(false);
        MainmenuPage.instance.consoleZone.btnSleep.StartCooldown();

        PetData.PetInspector.OnSleepStart();
        Sleep.instance.OnPlay();
    }
    static void OnPlay()
    {
        Play.instance.OnPlay((playType,action)=> {
            if (action == Play.PlayAction.None)
            {

            }
            else if (action == Play.PlayAction.Playing) 
            {
                MainmenuPage.instance.main.OnActive(false);
                MainmenuPage.instance.consoleZone.btnPlay.StartCooldown();
            }
            else
            {
                PetData.PetInspector.OnPlayComplete(playType,action == Play.PlayAction.Win);
                OnEnding();
            }
        });
    }
}
