using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rarity
{
    public const string UltraRare = "Ultra Rare";
    public const string Rare = "Rare";
    public const string Uncommond = "Uncommond";
    public const string Commond = "Commond";
}

public class PetData
{
    public enum Feeling
    {
        Super, Happy, Normal, Bad
    }

    public bool Enable;
    public string ID;
    public string Name;
    public string Description;
    public string Kind;
    public string Rarity;
    public string Url_Image;
    public string Url_Bundle;
    public Dictionary<Food.FoodType, Feeling> Foods = new Dictionary<Food.FoodType, Feeling>();
    public Dictionary<string, object> Config = new Dictionary<string, object>();
    public string ContractAddress ;
    public string TokenId;

    public PetData(GameData raw)
    {
        Enable = System.Convert.ToBoolean(raw.GetValue("Enable"));
        ID = raw.GetValue("Enable");
        Name = raw.GetValue("Name");
        Description = raw.GetValue("Description");
        Kind = raw.GetValue("Kind");
        Rarity = raw.GetValue("Rarity");
        Url_Image = raw.GetValue("Url_Image");
        Url_Bundle = raw.GetValue("Url_Bundle");
        ContractAddress = raw.GetValue("ContractAddress");
        TokenId = raw.GetValue("TokenId");
        Config = raw.GetValue("Config").DeserializeObject<Dictionary<string, object>>();

        AddFood(Food.FoodType.Fish);
        AddFood(Food.FoodType.Meat);
        AddFood(Food.FoodType.Milk);
        AddFood(Food.FoodType.Corn);
        AddFood(Food.FoodType.Cookie);
        AddFood(Food.FoodType.Chocolate);
        AddFood(Food.FoodType.Honey);
        AddFood(Food.FoodType.Vegetable);
        void AddFood(Food.FoodType food) { Foods.Add(food, (Feeling)raw.GetValue(food.ToString()).ToEnum(Feeling.Normal)); }
    }






    public static bool Done => PetDatas.Count!=0;
    public static List<PetData> PetDatas = new List<PetData>();
    public static void Init(System.Action done)
    {
        LoaderService.instance.OnLoadTsv("0",(data) =>
        {
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable())
            {
                PetDatas.Add(new PetData(d));
            }
            Debug.Log("ContentDatas:" + PetDatas.Count);
            done();
        });
    }


}
