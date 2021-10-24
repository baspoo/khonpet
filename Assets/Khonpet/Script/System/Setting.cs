using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }






}
