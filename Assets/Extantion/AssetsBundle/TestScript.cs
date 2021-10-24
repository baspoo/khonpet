using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public AssetsBundleHandle assetsBundleHandle;
    void Start()
    {


        // var img = (Texture) ResourcesHandle.Load("imgs","1", ResourcesHandle.FileType.png);

        //var img = (Texture)ResourcesHandle.LoadAll("imgs","")[0];

        assetsBundleHandle = AssetsBundleHandle.instance;

      

        //assetsBundleHandle.OnClear();
        var bundle = "imgs";


        string url = "";
        #if UNITY_EDITOR
        url = $"{AssetsBundlePath.FullPathOutputByType(UnityEditor.BuildTarget.StandaloneWindows)}/{bundle}";
        #endif

        url = $"http://junk.onemoby.com/thelastbug/assetsbundle/{bundle}";


        assetsBundleHandle.OnClear();
        assetsBundleHandle.OnDownloadAssets(bundle, url ,(done)=> {

            var file = $"{AssetsBundlePath.pathInput}/imgs/1.png";



            //assetsBundleHandle.OnLoadAssetAsync(bundle, file, (asset) =>
            //{
            //    GetComponent<Renderer>().material.mainTexture = (Texture)asset;
            //});



            //var items = ResourcesHandle.LoadAllEditor( "imgs", "");
            //GetComponent<Renderer>().material.mainTexture = (Texture)items[1];

            //GetComponent<Renderer>().material.mainTexture = (Texture)assetsBundleHandle.OnLoadAsset( bundle , file);

            GetComponent<Renderer>().material.mainTexture = (Texture)ResourcesHandle.Load(bundle , "1", ResourcesHandle.FileType.png);
        });


    }

    public float downloading;
    // Update is called once per frame
    void Update()
    {
        if(assetsBundleHandle!=null)
            downloading = assetsBundleHandle.progress;
    }
}
