using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Food
{
    public enum FoodType
    {
        Fish, Meat, Milk, Corn, Cookie, Chocolate, Honey, Vegetable
    }

    public string Name;
    public FoodType Type;
    public Sprite Icon;



    public static void Init( )
    {

    }


}