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
    public string ContractAddress;
    public string TokenId;

    public long Like => FirebaseService.instance.Preset.Like;
    public long Star => FirebaseService.instance.Preset.Star;


    public PetData(GameData raw)
    {
        Enable = System.Convert.ToBoolean(raw.GetValue("Enable"));
        ID = raw.GetValue("ID");
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


    public static PetData FInd(string petID) => PetDatas.Find(x => x.ID == petID);
    public static PetData FInd(string ContractAddress, string TokenId) => PetDatas.Find(x => x.ContractAddress == ContractAddress && x.TokenId == TokenId);
    public static PetData FInd(URLParameters.Parameters parameters) {
        var id = parameters.SearchParams.Get("id");
        if (id.notnull())
        {
            var address = parameters.SearchParams.Get("id").Split('/');
            return FInd(address[0], address[1]);
        }
        else 
        {
            return null;
        }
    } 
   


    public static PetData Current=> m_Current;
    static PetData m_Current;
    public static void SetCurrent(string ContractAddress, string TokenId) 
    {
        m_Current = FInd(ContractAddress, TokenId);
        if(m_Current == null)
            Debug.LogError($"SetCurrent Find Not Found {ContractAddress} {TokenId}");


    }

    public static bool Done => PetDatas.Count!=0;
    public static List<PetData> PetDatas = new List<PetData>();
    public static void Init(System.Action done)
    {
        if (Setting.instance.tsv.getTsv == Setting.Tsv.GetTsv.bySetting)
        {
            //GetBy Setting Editor
            CreatedData(Setting.instance.tsv.PetTsv);
        }
        else 
        {
            //GetBy Internet
            LoaderService.instance.OnLoadTsv(LoaderService.GoogleSpreadsheetsID.pet, (data) =>
            {
                CreatedData(data);
            });
        }
       
        void CreatedData(string data) 
        {
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable())
            {
                PetDatas.Add(new PetData(d));
            }
            Debug.Log("ContentDatas:" + PetDatas.Count);
            done();
        }

    }


}
