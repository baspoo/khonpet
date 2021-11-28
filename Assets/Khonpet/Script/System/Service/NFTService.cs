using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NFTService : MonoBehaviour
{
    public static NFTService instance { get { if (m_instance == null) m_instance = FindObjectOfType<NFTService>(); return m_instance; } }
    static NFTService m_instance;

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
    }
    public void SetupPreset()
    {
        Preset = new PresetData();
        Preset.ContractAddress = ContractAddress;
        Preset.Token = Token;
        Preset.OwnerName = MainAsset.owner.user.username;
        Preset.OwnerAddress = MainAsset.owner.address;
        Preset.OwnerProfileImgUrl = MainAsset.owner.profile_img_url;
        Preset.PetImageUrl = MainAsset.image_url;
    }







    public bool IsDone { get; private set; }

    public Model.OpenSeaAsset MainAsset;
    public Model.OpenSeaOwner OwnerData;

    public IEnumerator Init(string contractAddress, string token)
    {
        IsDone = false;
        ContractAddress = contractAddress;
        Token = token;


        if (Setting.instance.nft.IsDummy) 
        {
            IsDone = true;
            MainAsset = "NFT-MainAsset".GetByLocal().DeserializeObject<Model.OpenSeaAsset>();
            OwnerData = "NFT-OwnerData".GetByLocal().DeserializeObject<Model.OpenSeaOwner>();
            SetupPreset();
            yield break;
        }

        var url = $"https://api.opensea.io/api/v1/asset/{ContractAddress}/{Token}/";
        Debug.Log(url);
        UnityWebRequest uwr =  UnityWebRequest.Get(url);
        yield return uwr.Send();

        if (uwr.error != null)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            var json = uwr.downloadHandler.text;
            MainAsset = json.DeserializeObject<Model.OpenSeaAsset>();
            #if UNITY_EDITOR
            json.SaveToLocal("NFT-MainAsset");
            #endif
            StartCoroutine(Owner(MainAsset.owner.address, ContractAddress));
        }

    }


    IEnumerator Owner(string owner , string contractaddress = null , int begin = 0, int end = 20)
    {
        var url = $"https://api.opensea.io/api/v1/assets?owner={owner}"; 
        if (!string.IsNullOrEmpty(contractaddress)) 
            url += $"&asset_contract_address={contractaddress}";
        url += $"&order_direction=desc&offset={begin}&limit={end}";
        Debug.Log(url);
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
            json.SaveToLocal("NFT-OwnerData");
            #endif
        }
        SetupPreset();
        IsDone = true;
    }







}
