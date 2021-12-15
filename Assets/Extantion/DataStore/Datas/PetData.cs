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
[System.Serializable]
public class Feeling
{
    public enum FeelingType
    {
        Super, Happy, Normal, Bad
    }
    public string Name;
    public FeelingType Type;
    public Sprite Icon;
}
public class PetData
{

    
    public bool Enable;
    public string ID;
    public string Name;
    public string Description;
    public string Air;
    public string Kind;
    public string Rarity;
    public int vBundle;
    public int[] LvUnlocks;
    public Dictionary<Food.FoodType, Feeling.FeelingType> Foods = new Dictionary<Food.FoodType, Feeling.FeelingType>();
    public Dictionary<string, object> Meta = new Dictionary<string, object>();
    public string ContractAddress;
    public string TokenId;




    public static PetActivity.PetInspector PetInspector => Current.petInspector;
    public PetActivity.PetInspector petInspector
    {
        get
        {
            if (m_Inspector == null)
            {
                m_Inspector = new PetActivity.PetInspector(this);
                QuestActivity.InitQuest(m_Inspector);
            }
            return m_Inspector;
        }
    }
    PetActivity.PetInspector m_Inspector;





    public PetData(GameData raw)
    {
        Enable = System.Convert.ToBoolean(raw.GetValue("Enable"));
        ID = raw.GetValue("ID");
        Name = raw.GetValue("Name");
        Description = raw.GetValue("Description");
        Kind = raw.GetValue("Kind");
        Air = raw.GetValue("Air");
        Rarity = raw.GetValue("Rarity");
        vBundle = raw.GetValue("vBundle").ToInt();
        ContractAddress = raw.GetValue("ContractAddress");
        TokenId = raw.GetValue("TokenId");
        Meta = raw.GetValue("Meta").DeserializeObject<Dictionary<string, object>>();
        LvUnlocks = raw.GetValue("LvUnlocks").DeserializeObject<int[]>();

        AddFood(Food.FoodType.Fish);
        AddFood(Food.FoodType.Meat);
        AddFood(Food.FoodType.Milk);
        AddFood(Food.FoodType.Corn);
        AddFood(Food.FoodType.Cookie);
        AddFood(Food.FoodType.Chocolate);
        AddFood(Food.FoodType.Honey);
        AddFood(Food.FoodType.Vegetable);
        void AddFood(Food.FoodType food) 
        { 
            Foods.Add(food, (Feeling.FeelingType)raw.GetValue(food.ToString()).ToEnum(Feeling.FeelingType.Normal)); 
        }
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

        if (ContractAddress.notnull() && TokenId.notnull())
        {
            //** Find By ContractAddress & TokenId
            m_Current = FInd(ContractAddress, TokenId);
        }
        else 
        {
            //** Find By petID
            m_Current = FInd(ContractAddress);
            if (m_Current != null) 
            {
                Information.instance.ContractAddress = m_Current.ContractAddress;
                Information.instance.TokenId = m_Current.TokenId;
            }
        }

       


        if(m_Current == null) 
        { 
            Debug.LogError($"SetCurrent Find Not Found {ContractAddress} {TokenId}");
            PopupPage.instance.message.Open("Find Not Found!", Language.Get("find_not_found_pet") ,false,true).HideBtnClose();
            InterfaceRoot.instance.Loading(false);
        }

    }

    public static bool Done => PetDatas.Count!=0;
    public static List<PetData> PetDatas = new List<PetData>();
    public static void Init(System.Action done)
    {
        //if (Setting.instance.tsv.getTsv == Setting.Tsv.GetTsv.bySetting)
        //{
        //    //GetBy Setting Editor
        //    CreatedData(Setting.instance.tsv.PetTsv);
        //}
        //else 
        //{
        //    //GetBy Internet
        //    LoaderService.instance.OnGetDatabase("petdata.tsv", (data) =>
        //    {
        //        CreatedData(data);
        //    });
        //    //LoaderService.instance.OnLoadTsv(LoaderService.GoogleSpreadsheetsID.pet, (data) =>
        //    //{
        //    //    CreatedData(data);
        //    //});
        //}


        LoaderService.instance.OnGetDatabase("petdata.tsv", (data) =>
        {
            CreatedData(data);
        });
        void CreatedData(string data) 
        {
            var table = GameDataTable.ReadData(data);
            foreach (var d in table.GetTable())
            {
                PetDatas.Add(new PetData(d));
            }
            //Debug.Log("ContentDatas:" + PetDatas.Count);
            done();
        }

    }


}
