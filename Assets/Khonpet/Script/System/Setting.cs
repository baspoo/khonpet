using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class Setting : MonoBehaviour
{
    public static Setting instance { get { if (m_instance == null) m_instance = FindObjectOfType<Setting>(); return m_instance; } }
    static Setting m_instance;






    public Debugging debug;
    [System.Serializable]
    public class Debugging
    {


        [Header("URL")]
        public URLParameters.Parameters Parameters;
        [Space]

        [Header("AssetBundle")]
        public bool isDebugBundle;
        public AssetBundle PetBundle;


        public bool isDebugAir;
        public AirActivity.AirType AirType;


        public bool isStarDebug;
        public int StarCountDebug;

    }

    public Bundle bundle;
    [System.Serializable]
    public class Bundle
    {
        public ResourcesHandle.LoadType LoadType;
    }



    public Link link;
    [System.Serializable]
    public class Link
    {
        public string facebook;
        public string opensea;
    }



    public Device device;
    [System.Serializable]
    public class Device
    {
        public bool isForceMobile;
        public ScreenType screen;
        public enum ScreenType
        {
            auto,
            portrait,
            wide
        }
    }


    public NFT nft;
    [System.Serializable]
    public class NFT
    {
        public bool IsDummy;
        [TextArea]
        public string MainAsset;
        [TextArea]
        public string OwnerData;
    }

    public Tsv tsv;
    [System.Serializable]
    public class Tsv
    {
        public enum GetTsv { Realtime,ConfigVersion,bySetting }
        public GetTsv getTsv;
        [TextArea]
        public string ConfigTsv;
        [TextArea]
        public string PetTsv;
        [TextArea]
        public string Language;
    }






}









#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(Setting))]
[System.Serializable]
public class SettingUI : Editor
{


    Setting m_page => (Setting)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Adjuest Setting"))
        {
            m_page.debug.isDebugBundle = false;
            m_page.bundle.LoadType = ResourcesHandle.LoadType.CloudFile;
            m_page.device.isForceMobile = false;
            m_page.device.screen = Setting.Device.ScreenType.auto;
            m_page.tsv.getTsv = Setting.Tsv.GetTsv.Realtime;


        }
    }
}
#endif