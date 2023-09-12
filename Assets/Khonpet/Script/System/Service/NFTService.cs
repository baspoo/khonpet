using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NFTService : MonoBehaviour
{
    public static NFTService instance { get { if (m_instance == null) m_instance = FindObjectOfType<NFTService>(); return m_instance; } }
    static NFTService m_instance;
    public bool IsIgnore;
    string ContractAddress;
    string Token;

    public PresetData Preset;
    [System.Serializable]
    public class PresetData
    {
        public string ContractAddress;
        public string Token;
        public string OwnerName;
        public string OwnerAddress;
        public string OwnerProfileImgUrl;
        public string PetImageUrl;
        public string ImageUrl;
    }
    [System.Serializable]
    public class CollectionData
    {
        public string Name;
        public string Description;
        public string ContractAddress;
        public string Token;
        public string ImageUrl;
    }
    public List<CollectionData> KhonpetOwner;
    public List<CollectionData> OtherOwner;

    public void SetupPreset(Model.OpenSeaAsset.UserData owner)
    {
        Preset = new PresetData();
        Preset.ContractAddress = ContractAddress;
        Preset.Token = Token;
        Preset.OwnerName = owner.user.username;
        Preset.OwnerAddress = owner.address;
        Preset.OwnerProfileImgUrl = owner.profile_img_url;
        Preset.PetImageUrl = MainAsset.image_url;

        foreach (var assest in OwnerData.assets) 
        {
            NFTService.CollectionData collection = new CollectionData();
            collection.ContractAddress = assest.creator.address;
            collection.Token = assest.token_id;
            collection.ImageUrl = assest.image_preview_url;
            collection.Name = assest.name;
            collection.Description = assest.description;
            if (collection.ContractAddress == ContractAddress)
            {
                KhonpetOwner.Add(collection);
            }
            else 
            {
                OtherOwner.Add(collection);
            }
        }
        Debug.Log($"KhonpetOwner : {KhonpetOwner.Count}");
        Debug.Log($"OtherOwner : {OtherOwner.Count}");
    }
    public void SetupPresetUnkwon()
    {
        Preset = new PresetData();
        Preset.ContractAddress = ContractAddress;
        Preset.Token = Token;
        Preset.OwnerName = "Unknown";
        Preset.OwnerAddress = "Unknown";
        Preset.OwnerProfileImgUrl = null;
        Preset.PetImageUrl = null;
        IsDone = true;
    }






    public bool IsDone { get; private set; }

    public Model.OpenSeaAsset MainAsset;
    public Model.OpenSeaOwner OwnerData;


    public Model.OpenSeaAsset.UserData FindOwner(Model.OpenSeaAsset asset) {
        if (asset == null)
            Debug.LogError("asset null");
        if (asset.owner != null && asset.owner.address != "0x0000000000000000000000000000000000000000")
        {
                return asset.owner;
        }
        else 
        {
            if (asset.top_ownerships != null && asset.top_ownerships.Count > 0)
            {
                return asset.top_ownerships[0].owner;
            }
            else 
            {
                return null;
            }
        }
        
    }

    public IEnumerator Init(string contractAddress, string token)
    {
        IsDone = false;
        ContractAddress = contractAddress;
        Token = token;

        if (IsIgnore)
        {
            SetupPresetUnkwon();
            yield break;
        }
 


        if (Setting.instance.nft.IsDummy) 
        {
            IsDone = true;
            MainAsset = "NFT-MainAsset".GetByLocal().DeserializeObject<Model.OpenSeaAsset>();
            OwnerData = "NFT-OwnerData".GetByLocal().DeserializeObject<Model.OpenSeaOwner>();

            var userOwner = FindOwner(MainAsset);
            if (userOwner != null) SetupPreset(userOwner);
            else SetupPresetUnkwon();
            yield break;
        }

        var url = $"https://api.opensea.io/api/v1/asset/{ContractAddress}/{Token}/";
        Logger.Log(url);
        UnityWebRequest uwr =  UnityWebRequest.Get(url);
        yield return uwr.Send();

        if (uwr.error != null)
        {
            Debug.LogError(uwr.error);
            SetupPresetUnkwon();
        }
        else
        {
            var json = uwr.downloadHandler.text;
            MainAsset = json.DeserializeObject<Model.OpenSeaAsset>();
            #if UNITY_EDITOR
            json.SaveToLocal("NFT-MainAsset");
#endif

            var userOwner = FindOwner(MainAsset);
            if (userOwner != null) StartCoroutine(Owner(userOwner, ContractAddress));
            else SetupPresetUnkwon();

           
        }

    }


    IEnumerator Owner(Model.OpenSeaAsset.UserData owner , string contractaddress = null , int begin = 0, int end = 20)
    {
        var url = $"https://api.opensea.io/api/v1/assets?owner={owner.address}"; 
        if (!string.IsNullOrEmpty(contractaddress)) 
            url += $"&asset_contract_address={contractaddress}";
        url += $"&order_direction=desc&offset={begin}&limit={end}";
        Logger.Log(url);
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.Send();

        if (uwr.error != null)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            var json = uwr.downloadHandler.text;
            OwnerData = json.DeserializeObject<Model.OpenSeaOwner>();
            #if UNITY_EDITOR
            Debug.Log(json);
            json.SaveToLocal("NFT-OwnerData");
            #endif
        }
        SetupPreset(owner);
        IsDone = true;
    }







}
