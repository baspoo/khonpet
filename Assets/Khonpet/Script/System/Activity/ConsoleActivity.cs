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

    public static void OnBegin(Activity activity) 
    {
        if (activity == Activity.Food) OnFood();
        if (activity == Activity.Play) OnPlay();
        if (activity == Activity.Clean) OnClean();
        if (activity == Activity.Sleep) OnSleep();
    }





    static void OnFood( )
    {
        IsActing = true;
        MainmenuPage.instance.main.OnActive(false);
        MainmenuPage.instance.consoleZone.btnFood.StartCooldown();
        FoodPage.instance.OnPlay((food) => {
            MainmenuPage.instance.main.OnActive(true);
            MainmenuPage.instance.starZone.OnAddStar(3);
            IsActing = false;
        });
    }
    static void OnClean()
    {
        IsActing = true;
        MainmenuPage.instance.main.OnActive(false);
        MainmenuPage.instance.consoleZone.btnClean.StartCooldown();
        Clean.instance.OnPlay(() => {
            MainmenuPage.instance.main.OnActive(true);
            MainmenuPage.instance.starZone.OnAddStar(2);
            IsActing = false;
        });
    }

    static void OnSleep()
    {
        IsActing = true;
        MainmenuPage.instance.main.OnActive(false);
        MainmenuPage.instance.consoleZone.btnSleep.StartCooldown();
        Sleep.instance.OnPlay(() => {
            MainmenuPage.instance.main.OnActive(true);
            MainmenuPage.instance.starZone.OnAddStar(5);
            IsActing = false;
        });
    }
    static void OnPlay()
    {
        Play.instance.OnPlay((action)=> {
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
                MainmenuPage.instance.main.OnActive(true);
                if (action == Play.PlayAction.Win)
                {
                    MainmenuPage.instance.starZone.OnAddStar(5);
                }
                if (action == Play.PlayAction.Lose)
                {

                }
                IsActing = false;
            }
        });
    }
}
