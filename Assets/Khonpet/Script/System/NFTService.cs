using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NFTService : MonoBehaviour
{

    public string ContractAddress;
    public string Token;


    public Model.OpenSeaAsset MainAsset;
    public Model.OpenSeaOwner OwnerData;

    IEnumerator Start()
    {
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
        }
    }







}
