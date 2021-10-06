using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;





public class GetRaw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
   

   
        //string address = "0xa7d8d9ef8d8ce8992df33d8b8cf4aebabd5bd270";
        //string token_id = "131000037";
        //string url = $"https://api.opensea.io/api/v1/asset/{address}/{token_id}/";


        string owner = "0xf4b4a58974524e183c275f3c6ea895bc2368e738";
        string url = "https://api.opensea.io/api/v1/assets?owner={owner}&order_direction=desc&offset=0&limit=50";
        StartCoroutine(load(url));
    }


    IEnumerator load(string url) {

        WWW www = new WWW(url);
        yield return www;
        if (www.isDone)
        {
            if (www.error == null)
            {
               
                //Model.OpenSea opensea = JsonConvert.DeserializeObject<Model.OpenSea>(www.text);
               // Debug.Log(opensea.name);
               // Debug.Log(opensea.description);
               // Debug.Log(opensea.image_thumbnail_url);
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
        else 
        {
            Debug.LogError("lose");
        }
    
    }



    // Update is called once per frame
    void Update()
    {
        
    }












}
